using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpotifyTracks : MonoBehaviour {

    string client_id = "a235b5a294f64861b17afaaf2e0752c6"; // client id
    string client_secret = "1e15336c55c44efe8c2954f0c465cb00"; // client secret

    private string accessToken = "";
    private string tokenType = "";

    private List<string> trackIDs;
    private List<string> trackNames;
    private List<string> trackPreviewURLs;

    [HideInInspector]
    public SpotifySongs objectScript;

    public void StartSpotifySearch(string objectID, string objectType, SpotifySongs script)
    {
        ResetVariables();
        objectScript = script;
        StartCoroutine(SearchForTracks(objectID, objectType));
    }

    private void ResetVariables()
    {
        accessToken = "";
        tokenType = "";
        trackIDs = new List<string>();
        trackNames = new List<string>();
        trackPreviewURLs = new List<string>();
    }

    // client credentials flow 
    IEnumerator SearchForTracks(string objectID, string objectType)
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

        // get tracks 
        string tracksSearchURL = "";

        if (objectType.Equals("album"))
        {
            tracksSearchURL = "https://api.spotify.com/v1/albums/" + objectID + "/tracks";
        }

        if (objectType.Equals("playlist"))
        {
            tracksSearchURL = "https://api.spotify.com/v1/browse/featured-playlists/" + objectID + "/tracks";
        }

        WWWForm tracksForm = new WWWForm();
        Dictionary<string, string> tracksHeaders = tracksForm.headers;
        tracksHeaders["Authorization"] = tokenType + " " + accessToken;

        WWW tracksWWW = new WWW(tracksSearchURL, null, tracksHeaders); 
        yield return tracksWWW;

        string encodedStringTracks = tracksWWW.text;
        JSONObject jTracks = new JSONObject(encodedStringTracks);
        accessData(jTracks);


        int numberOfTracks = jTracks["items"].Count;
        Debug.Log(numberOfTracks);

        for (int i = 0; i < numberOfTracks; i++)
        {
            trackIDs.Add(jTracks["items"][i]["id"].str);
            trackNames.Add(jTracks["items"][i]["name"].str);
            trackPreviewURLs.Add(jTracks["items"][i]["preview_url"].str);
        }

        objectScript.ProcessSpotifyData(trackIDs, trackNames, trackPreviewURLs);
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
