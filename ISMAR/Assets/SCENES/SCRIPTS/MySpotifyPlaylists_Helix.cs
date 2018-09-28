using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySpotifyPlaylists_Helix : MonoBehaviour
{ 
    SpotifyGenerator spotify;

    // Spotify API 
    string client_id = "a235b5a294f64861b17afaaf2e0752c6"; // client id
    string client_secret = "1e15336c55c44efe8c2954f0c465cb00"; // client secret
    private string accessToken = "";
    private string tokenType = "";

    string myUserID = "12156650693"; 

    // categories 
    private List<string> playlistIDs;
    private List<string> playlistNames;
    private List<Texture2D> playlistTextures;

    void Start()
    {
        ResetVariables();
        StartCoroutine(GetMyPlaylists());
        spotify = gameObject.AddComponent<SpotifyGenerator>();
    }

    private void ResetVariables()
    {
        accessToken = "";
        tokenType = "";
        playlistIDs = new List<string>();
        playlistNames = new List<string>();
        playlistTextures = new List<Texture2D>();
    }

    // client credentials flow 
    IEnumerator GetMyPlaylists()
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

        // get my playlists
        string myPlaylistsUrl = "https://api.spotify.com/v1/users/" + myUserID + "/playlists?limit=10"; 
        WWWForm myPlaylistsForm = new WWWForm();
        Dictionary<string, string> myPlaylistsHeaders = myPlaylistsForm.headers;
        myPlaylistsHeaders["Authorization"] = tokenType + " " + accessToken;

        WWW myPlaylistsWWW = new WWW(myPlaylistsUrl, null, myPlaylistsHeaders);
        yield return myPlaylistsWWW;

        string encodedStringPlaylists = myPlaylistsWWW.text;
        JSONObject jCategory = new JSONObject(encodedStringPlaylists);
        accessData(jCategory);

        int numberOfPlaylists = jCategory["items"].Count;
        Debug.Log("Number of Playlists :" + numberOfPlaylists);

        string categoryTextureURL;
        WWW wwwcategoryTexture;

        for (int i = 0; i < numberOfPlaylists; i++)
        {
            playlistIDs.Add(jCategory["items"][i]["id"].str);
            playlistNames.Add(jCategory["items"][i]["name"].str);
            categoryTextureURL = jCategory["items"][i]["images"][0]["url"].str;
            wwwcategoryTexture = new WWW(categoryTextureURL);
            yield return wwwcategoryTexture;
            playlistTextures.Add(wwwcategoryTexture.texture);
        }

        spotify.processSpotifyPlaylistData(playlistIDs, playlistNames, playlistTextures);
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
