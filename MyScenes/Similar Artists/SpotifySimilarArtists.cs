using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpotifySimilarArtists : MonoBehaviour {

    SpotifyGenerator spotify;

    // Spotify API 
    string client_id = "a235b5a294f64861b17afaaf2e0752c6"; // client id
    string client_secret = "1e15336c55c44efe8c2954f0c465cb00"; // client secret
    private string accessToken = "";
    private string tokenType = "";

    [HideInInspector]
    public string[] artistName;
 
    private string artistSpotifyID;
    private string artistSpotifyName;
    private Texture2D artistTexture;

    private List<string> artistIDs = new List<string>();
    private List<string> artistNames = new List<string>();
    private List<Texture2D> artistTextures = new List<Texture2D>();

    void Start()
    {
        ResetVariables();
        artistName = new string[1];
        artistName[0] = "Kendrick Lamar";
        StartCoroutine(SearchForArtistAndRelated());
        spotify = gameObject.AddComponent<SpotifyGenerator>(); 
    }

    private void ResetVariables()
    {
        accessToken = "";
        tokenType = ""; 
    }

    // client credentials flow 
    IEnumerator SearchForArtistAndRelated () {

        string authUrl = "https://accounts.spotify.com/api/token";
        WWWForm authForm = new WWWForm();

        authForm.AddField("grant_type", "client_credentials");
        byte[] rawData = authForm.data;

        Dictionary<string,string> authHeaders = authForm.headers;
        authHeaders["Authorization"] = "Basic " + System.Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(client_id + ":" + client_secret));

        WWW authWww = new WWW(authUrl, rawData, authHeaders);
        yield return authWww;

        string encodedStringAuth = authWww.text;
        JSONObject j = new JSONObject(encodedStringAuth);
        accessData(j);

        accessToken = j.list[0].str;
        tokenType = j.list[1].str;

        string artistSearchURL = "https://api.spotify.com/v1/search?q=";
        for (int i = 0; i < artistName.Length; i++)
        {
            artistSearchURL += artistName[i];
            if (i != artistName.Length - 1)
                artistSearchURL += "%20";
        }

        artistSearchURL += "&type=artist&limit=1";
        WWWForm artistForm = new WWWForm();
        Dictionary<string, string> artistHeaders = artistForm.headers;
        artistHeaders["Authorization"] = tokenType + " " + accessToken;

        WWW artistWWW = new WWW(artistSearchURL, null, artistHeaders);
        yield return artistWWW;

        string encodedStringArtist = artistWWW.text;
        JSONObject jArtist = new JSONObject(encodedStringArtist);
        accessData(jArtist);

        artistSpotifyName = jArtist["artists"]["items"][0]["name"].str;
        artistSpotifyID = jArtist["artists"]["items"][0]["id"].str;
        string artistImageURL = jArtist["artists"]["items"][0]["images"][0]["url"].str;
        WWW wwwArtistTexture = new WWW(artistImageURL);
        yield return wwwArtistTexture;
        artistTexture = wwwArtistTexture.texture;

        artistIDs.Add(artistSpotifyID);
        artistNames.Add(artistSpotifyName);
        artistTextures.Add(artistTexture); 

        // similar artists 
        string relatedArtistURL = "https://api.spotify.com/v1/artists/" + artistSpotifyID + "/related-artists";

        WWWForm relatedArtistForm = new WWWForm();
        Dictionary<string, string> relatedArtistHeaders = relatedArtistForm.headers;
        relatedArtistHeaders["Authorization"] = tokenType + " " + accessToken;

        WWW relatedArtistWWW = new WWW(relatedArtistURL, null, relatedArtistHeaders);
        yield return relatedArtistWWW;

        string encodedStringRelatedArtist = relatedArtistWWW.text;
        JSONObject jRelatedArtist = new JSONObject(encodedStringRelatedArtist);
        accessData(jRelatedArtist);

        int numberOfSimilarArtists = jRelatedArtist["artists"].Count;
        string relatedArtistTextureURL;
        WWW wwwrelatedArtistTexture;

        for (int i = 0; i < numberOfSimilarArtists; i++)
        {

            artistIDs.Add(jRelatedArtist["artists"][i]["id"].str);
            artistNames.Add(jRelatedArtist["artists"][i]["name"].str);

            relatedArtistTextureURL = jRelatedArtist["artists"][i]["images"][0]["url"].str;
            wwwrelatedArtistTexture = new WWW(relatedArtistTextureURL);
            yield return wwwrelatedArtistTexture;
            artistTextures.Add(wwwrelatedArtistTexture.texture);
        }

        spotify.processSpotifyArtistData(artistIDs, artistNames, artistTextures);

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