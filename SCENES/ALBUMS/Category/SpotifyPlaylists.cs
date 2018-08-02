using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpotifyPlaylists : MonoBehaviour
{

    // Spotify API 
    string client_id = "a235b5a294f64861b17afaaf2e0752c6"; // client id
    string client_secret = "1e15336c55c44efe8c2954f0c465cb00"; // client secret
    private string accessToken = "";
    private string tokenType = "";

    // playlists 
    private List<string> playlistIDs;
    private List<Texture2D> playlistTextures; 

    [HideInInspector]
    public Helix helixScript; 

    public void StartSpotifySearch(string categoryID, Helix script)
    {
        ResetVariables();
        helixScript = script;
        StartCoroutine(SearchForPlaylists(categoryID));

    }

    private void ResetVariables()
    {
        accessToken = "";
        tokenType = "";
        playlistIDs = new List<string>();
        playlistTextures = new List<Texture2D>(); 
    }

    public int maxNumberOfPlaylists = 20;

    // client credentials flow 
    IEnumerator SearchForPlaylists(string categoryID)
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
        string categoryPlaylistsURL = "https://api.spotify.com/v1/browse/categories/" + categoryID + "/playlists";
        WWWForm categoryPlaylistsForm = new WWWForm();
        Dictionary<string, string> categoryPlaylistsHeaders = categoryPlaylistsForm.headers;
        categoryPlaylistsHeaders["Authorization"] = tokenType + " " + accessToken;

        WWW categoryPlaylistsWWW = new WWW(categoryPlaylistsURL, null, categoryPlaylistsHeaders);
        yield return categoryPlaylistsWWW;

        string encodedStringCategoryPlaylists = categoryPlaylistsWWW.text;
        JSONObject jCategoryPlaylists = new JSONObject(encodedStringCategoryPlaylists);
        accessData(jCategoryPlaylists);

        int numberOfPlaylists = jCategoryPlaylists["playlists"].Count;

        Debug.Log("Number of Playlists :" + numberOfPlaylists);

        string categoryPlaylistTextureURL;
        WWW wwwcategoryPlaylistTexture;

        for (int i = 0; i < numberOfPlaylists; i++)
        {

            playlistIDs.Add(jCategoryPlaylists["playlists"]["items"][i]["id"].str);
            categoryPlaylistTextureURL = jCategoryPlaylists["playlists"]["items"][i]["images"][0]["url"].str;
            wwwcategoryPlaylistTexture = new WWW(categoryPlaylistTextureURL);
            yield return wwwcategoryPlaylistTexture;
            playlistTextures.Add(wwwcategoryPlaylistTexture.texture);
        }

        string objectType = "playlist"; 
        helixScript.DisplaySpotifyObjects(playlistIDs, playlistTextures, objectType); 
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