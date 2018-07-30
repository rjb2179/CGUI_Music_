using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MercuryMessaging; 

// Get Token and Token Type, Send Message 
public class GetToken : MonoBehaviour {

    MmRelayNode _myRelayNode;
    
    string client_id = "a235b5a294f64861b17afaaf2e0752c6"; // client id
    string client_secret = "1e15336c55c44efe8c2954f0c465cb00"; // client secret 

    private string accessToken = "";
    private string tokenType = "";

    void Start()
    {
        _myRelayNode = GetComponent<MmRelayNode>();
        StartCoroutine(GetSpotifyToken()); 
    }

    // Client Credentials Flow 
    IEnumerator GetSpotifyToken()
    {
        // Request Authorization 
        string authUrl = "https://accounts.spotify.com/api/token";
        WWWForm authForm = new WWWForm();

        // Request Body Parameter 
        authForm.AddField("grant_type", "client_credentials");
        byte[] rawData = authForm.data;

        Dictionary<string, string> authHeaders = authForm.headers;
        authHeaders["Authorization"] = "Basic " + System.Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(client_id + ":" + client_secret));

        // URL 
        WWW authWww = new WWW(authUrl, rawData, authHeaders);
        yield return authWww;

        // Get Token and Token Type 
        string encodedStringAuth = authWww.text;
        JSONObject j = new JSONObject(encodedStringAuth);
        accessData(j);

        accessToken = j.list[0].str;
        tokenType = j.list[1].str;

        // MmMethod, string, MmMetadataBlock
        _myRelayNode.MmInvoke(MmMethod.MessageString, accessToken, 
            new MmMetadataBlock(MmLevelFilter.Child, MmActiveFilter.All));
        _myRelayNode.MmInvoke(MmMethod.MessageString, tokenType,
           new MmMetadataBlock(MmLevelFilter.Child, MmActiveFilter.All));
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
