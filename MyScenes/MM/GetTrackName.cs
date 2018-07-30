using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MercuryMessaging;

public class GetTrackName : MmBaseResponder { 

    private List<string> trackName = new List<string>();
    string accessToken = "";
    string tokenType = "";
    int count = 0;

    protected override void ReceivedMessage(MmMessageString message)
    {
        if (accessToken.Equals(""))
        {
            accessToken = message.value;
        }
        else
        {
            tokenType = message.value;
        }
    }

    public override void Update()
    {
        if (!(accessToken.Equals("")) && count == 0)
        {
            StartCoroutine(GetSpotifyTrackID("7ycBtnsMtyVbbwTfJwRjSP"));
            count = 1;
        }
    }

    IEnumerator GetSpotifyTrackID(string albumID)
    {
        string tracksSearchURL = "https://api.spotify.com/v1/albums/" + albumID + "/tracks";
        WWWForm tracksForm = new WWWForm();
        Dictionary<string, string> tracksHeaders = tracksForm.headers;
        tracksHeaders["Authorization"] = tokenType + " " + accessToken;

        // load data from www stream  
        WWW tracksWWW = new WWW(tracksSearchURL, null, tracksHeaders);
        yield return tracksWWW;

        string encodedStringTracks = tracksWWW.text;
        JSONObject jTracks = new JSONObject(encodedStringTracks);
        accessData(jTracks);

        for (int i = 0; i < jTracks["items"].Count; i++)
        {
            trackName.Add(jTracks["items"][i]["name"].str);
        }

        foreach (string Name in trackName)
        {
            Debug.Log(Name);
        }
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
                    // Debug.Log(key);
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
                // Debug.Log(obj.str);
                break;
            case JSONObject.Type.NUMBER:
                // Debug.Log(obj.n);
                break;
            case JSONObject.Type.BOOL:
                // Debug.Log(obj.b);
                break;
            case JSONObject.Type.NULL:
                // Debug.Log("NULL");
                break;

        }
    }
}
