using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RH_MySpotifyPlaylistsTracks : MonoBehaviour
{
    // Spotify API 
    string client_id = "a235b5a294f64861b17afaaf2e0752c6"; // client id
    string client_secret = "1e15336c55c44efe8c2954f0c465cb00"; // client secret
    private string accessToken = "";
    private string tokenType = "";

    string myUserID = "12156650693";

    // playlists 
    private List<string> trackIDs;
    private List<string> trackPreviewURLs;
    private List<Texture2D> trackTextures;

    [HideInInspector]
    public ReverseHelix reverseHelixScript;

    public void StartSpotifySearch(string playlistID, ReverseHelix script)
    {
        ResetVariables();
        reverseHelixScript = script;
        StartCoroutine(SearchForPlaylists(myUserID, playlistID));

    }

    private void ResetVariables()
    {
        accessToken = "";
        tokenType = "";
        trackIDs = new List<string>();
        trackPreviewURLs = new List<string>();
        trackTextures = new List<Texture2D>();
    }

    public int maxNumberOfPlaylists = 20;

    // client credentials flow 
    IEnumerator SearchForPlaylists(string userID, string playlistID)
    {
        // get access token 
        string authUrl = "https://accounts.spotify.com/api/token";
        WWWForm authForm = new WWWForm();

        authForm.AddField("grant_type", "client_credentials");
        byte[] rawData = authForm.data;

        Dictionary<string, string> authHeaders = authForm.headers;
        authHeaders["Authorization"] = "Basic " + System.Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(client_id + ":" + client_secret));

        WWW authWww = new WWW(authUrl, rawData, authHeaders);
        yield return authWww;

        string encodedStringAuth = authWww.text;
        JSONObject j = new JSONObject(encodedStringAuth);
        accessData(j);

        accessToken = j.list[0].str;
        tokenType = j.list[1].str;

        // get playlists 
        string myPlaylistsTracksURL = "https://api.spotify.com/v1/users/" + myUserID + "/playlists/" + playlistID + "/tracks";
        WWWForm myPlaylistsTracksForm = new WWWForm();
        Dictionary<string, string> myPlaylistsTracksHeaders = myPlaylistsTracksForm.headers;
        myPlaylistsTracksHeaders["Authorization"] = tokenType + " " + accessToken;

        WWW myPlaylistsTracksWWW = new WWW(myPlaylistsTracksURL, null, myPlaylistsTracksHeaders);
        yield return myPlaylistsTracksWWW;

        string encodedStringPlaylists = myPlaylistsTracksWWW.text;
        JSONObject jPlaylistsTracks = new JSONObject(encodedStringPlaylists);
        accessData(jPlaylistsTracks);

        int numberOfTracks = jPlaylistsTracks["items"].Count;
        Debug.Log("Number of Tracks :" + numberOfTracks);

        string myPlaylistTrackTextureURL;
        WWW myPlaylistTrackTexture;

        for (int i = 0; i < numberOfTracks; i++)
        {
            trackIDs.Add(jPlaylistsTracks["items"][i]["track"]["id"].str);
            trackPreviewURLs.Add(jPlaylistsTracks["items"][i]["track"]["preview_url"].str);
            myPlaylistTrackTextureURL = jPlaylistsTracks["items"][i]["track"]["album"]["images"][0]["url"].str;
            myPlaylistTrackTexture = new WWW(myPlaylistTrackTextureURL);
            yield return myPlaylistTrackTexture;
            trackTextures.Add(myPlaylistTrackTexture.texture);
        }

        string objectType = "track";
        reverseHelixScript.DisplaySpotifyObjects(trackIDs, trackTextures, objectType, trackPreviewURLs);
    }

    void accessData(JSONObject obj)
    {
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int i = 0; i < obj.list.Count; i++)
                {
                    string key = (string)obj.keys[i];
                    JSONObject j = (JSONObject)obj.list[i];
                    accessData(j);
                }
                break;
            case JSONObject.Type.ARRAY:
                foreach (JSONObject j in obj.list)
                {
                    accessData(j);
                }
                break;
            case JSONObject.Type.STRING:
                break;
            case JSONObject.Type.NUMBER:
                break;
            case JSONObject.Type.BOOL:
                break;
            case JSONObject.Type.NULL:
                break;

        }
    }
}
