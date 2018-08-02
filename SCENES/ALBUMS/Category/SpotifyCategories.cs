using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotifyCategories : MonoBehaviour {

    SpotifyGenerator_Helix spotify; 

    // Spotify API 
    string client_id = "a235b5a294f64861b17afaaf2e0752c6"; // client id
    string client_secret = "1e15336c55c44efe8c2954f0c465cb00"; // client secret
    private string accessToken = "";
    private string tokenType = "";

    // categories 
    private List<string> categoryIDs = new List<string>();
    private List<string> categoryNames = new List<string>();
    private List<Texture2D> categoryTextures = new List<Texture2D>();

    int maxNumberOfCategories = 20; 

    void Start()
    {
        ResetVariables();
        StartCoroutine(GetCategories());
        spotify = gameObject.AddComponent<SpotifyGenerator_Helix>(); 
    }

    private void ResetVariables()
    {
        accessToken = "";
        tokenType = "";
    }

    // client credentials flow 
    IEnumerator GetCategories()
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

        // get categories 
        string categoriesUrl = "https://api.spotify.com/v1/browse/categories?country=US"; 
        WWWForm categoriesForm = new WWWForm();
        Dictionary<string, string> categoriesHeaders = categoriesForm.headers;
        categoriesHeaders["Authorization"] = tokenType + " " + accessToken;

        WWW categoriesWWW = new WWW(categoriesUrl, null, categoriesHeaders);
        yield return categoriesWWW;

        string encodedStringCategory = categoriesWWW.text;
        JSONObject jCategory = new JSONObject(encodedStringCategory);
        accessData(jCategory);

        int numberOfCategories = jCategory["categories"].Count;

        Debug.Log("Number of Categories :" + numberOfCategories);

        string categoryTextureURL;
        WWW wwwcategoryTexture;

        for (int i = 0; i < numberOfCategories; i++)
        {
            categoryIDs.Add(jCategory["categories"]["items"][i]["id"].str);
            categoryNames.Add(jCategory["categories"]["items"][i]["name"].str);
            categoryTextureURL = jCategory["categories"]["items"][i]["icons"][0]["url"].str;
            wwwcategoryTexture = new WWW(categoryTextureURL);
            yield return wwwcategoryTexture;
            categoryTextures.Add(wwwcategoryTexture.texture);
        }

        spotify.processSpotifyCategoryData(categoryIDs, categoryNames, categoryTextures); 
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
