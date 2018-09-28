using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpotifyAlbums_Oculus : MonoBehaviour
{ 
    string client_id = "a235b5a294f64861b17afaaf2e0752c6"; // client id
    string client_secret = "1e15336c55c44efe8c2954f0c465cb00"; // client secret

    private string accessToken = "";
    private string tokenType = "";

    private List<string> albumIDs = new List<string>();
    private List<Texture2D> albumTextures = new List<Texture2D>();

    public int maxNumberOfAlbums = 20;

    [HideInInspector]
    public Helix_Oculus helixScript;

    public void StartSpotifySearch(string artistID, Helix_Oculus script)
    {
        ResetVariables();
        helixScript = script;
        StartCoroutine(SearchForAlbums(artistID));
    }

    private void ResetVariables()
    {
        accessToken = "";
        tokenType = "";
        albumIDs = new List<string>();
        albumTextures = new List<Texture2D>();
    }

    // Use this for initialization
    IEnumerator SearchForAlbums(string artistID)
    {

        // Use client credentials flow - get token and then use it
        string authUrl = "https://accounts.spotify.com/api/token";
        WWWForm authForm = new WWWForm();

        authForm.AddField("grant_type", "client_credentials");
        byte[] rawData = authForm.data;

        Dictionary<string, string> authHeaders = authForm.headers;
        authHeaders["Authorization"] = "Basic " + System.Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(client_id + ":" + client_secret));

        WWW authWww = new WWW(authUrl, rawData, authHeaders);
        // StartCoroutine(WaitForRequestPost(authWww));
        yield return authWww;

        string encodedStringAuth = authWww.text;
        JSONObject j = new JSONObject(encodedStringAuth);
        accessData(j);

        // Keep the access token (access_token) and the token type
        accessToken = j.list[0].str;
        tokenType = j.list[1].str;

        string albumSearchURL = "https://api.spotify.com/v1/artists/" + artistID + "/albums?album_type=album&market=US&limit=" + maxNumberOfAlbums;
        WWWForm albumForm = new WWWForm();
        Dictionary<string, string> albumHeaders = albumForm.headers;
        albumHeaders["Authorization"] = tokenType + " " + accessToken;

        WWW albumWWW = new WWW(albumSearchURL, null, albumHeaders);
        // StartCoroutine(WaitForRequestPost(artistWWW));
        yield return albumWWW;

        string encodedStringAlbums = albumWWW.text;
        JSONObject jAlbums = new JSONObject(encodedStringAlbums);
        accessData(jAlbums);

        string albumTextureURL;
        WWW wwwAlbumTexture;

        for (int i = 0; i < jAlbums["items"].Count; i++)
        {
            albumIDs.Add(jAlbums["items"][i]["id"].str);
            albumTextureURL = jAlbums["items"][i]["images"][0]["url"].str;
            wwwAlbumTexture = new WWW(albumTextureURL);
            yield return wwwAlbumTexture;
            albumTextures.Add(wwwAlbumTexture.texture);
        }

        string objectType = "album";
        helixScript.DisplaySpotifyObjects(albumIDs, albumTextures, objectType);
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
