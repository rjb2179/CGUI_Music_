using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MySpotify 
{
    public struct SpotifyTrack
    {
        public string id;
        public string name;
        public string previewURL;

        public SpotifyTrack(string trackId, string trackName, string trackPreviewURL)
        {
            id = trackId;
            name = trackName;
            previewURL = trackPreviewURL;
        }
    }
}

public class SpotifySongs : MonoBehaviour {
   
    // spotify ID of the object 
    public string objectID;

    // type of the object 
    public string objectType; 

    // spotify tracks 
    private SpotifyTracks tracksSpotifyScript;
    private List<MySpotify.SpotifyTrack> tracksList = new List<MySpotify.SpotifyTrack>();

    public void StartSpotifySearch()
    {
        tracksSpotifyScript = GameObject.FindGameObjectWithTag("InstructionSign").GetComponent<SpotifyTracks>();
        tracksSpotifyScript.StartSpotifySearch(objectID, objectType, this);
    }

    public void ProcessSpotifyData(List<string> ids, List<string> names, List<string> urls)
    {
        for (int i = 0; i < ids.Count;i++)
        {
            tracksList.Add(new MySpotify.SpotifyTrack(ids[i], names[i], urls[i]));
        } 

        foreach (MySpotify.SpotifyTrack track in tracksList)
        {
            Debug.Log(track.name); 
        }

        // AddSongsAtTheBack() 
    }

    /* 
    
    private void AddSongsAtTheBack()
    {
        List<string> songsList = new List<string>();
        for (int i = 0; i < tracksList.Count; i++)
        {
            songsList.Add((i + 1) + ". " + tracksList[i].name);
        }

        childSongsManager = GameObject.FindGameObjectWithTag("InstructionSign").GetComponent<TextAlbumInstancer>().InstantiateTextAlbum(
        songsList, transform.position + 0.01f * transform.forward, transform.rotation);

        childSongsManager.transform.parent = transform;
        childSongsManager.transform.localScale = Vector3.one;
        childSongsManager.SetLocalRotationsToZero();
        childSongsManager.transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
        childSongsManager.transform.localPosition += 0.42f * transform.up;

        int numberOfSongs = songsList.Count;

        if (numberOfSongs <= 8)
        {
            float ratio = 6f / numberOfSongs;
            foreach (GameObject go in childSongsManager.Albums)
            {
                Transform colliderTransform = go.GetComponentInChildren<BoxCollider>().transform;
                colliderTransform.localScale = new Vector3(colliderTransform.localScale.x, colliderTransform.localScale.x / numberOfSongs, colliderTransform.localScale.z);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, ratio * go.transform.localPosition.y, go.transform.localPosition.z);
                Text txt = go.GetComponentInChildren<Text>();
                txt.fontSize = (int)(txt.fontSize * ratio);
            }
        }
        else
        {
            int halfNumber = (numberOfSongs + 1) / 2;
            float ratio = 6f / halfNumber;

            for (int j = 0; j < halfNumber; j++)
            {
                GameObject go = childSongsManager.Albums[j];
                Transform colliderTransform = go.GetComponentInChildren<BoxCollider>().transform;
                colliderTransform.localScale = new Vector3(colliderTransform.localScale.x / 2, colliderTransform.localScale.x / halfNumber, colliderTransform.localScale.z);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x - 0.24f, ratio * go.transform.localPosition.y, go.transform.localPosition.z);
                Text txt = go.GetComponentInChildren<Text>();
                txt.fontSize = (int)(txt.fontSize * ratio / 2f);
            }

            float offset = ratio * childSongsManager.Albums[halfNumber].transform.localPosition.y;

            for (int j = halfNumber; j < numberOfSongs; j++)
            {
                GameObject go = childSongsManager.Albums[j];
                Transform colliderTransform = go.GetComponentInChildren<BoxCollider>().transform;
                colliderTransform.localScale = new Vector3(colliderTransform.localScale.x / 2, colliderTransform.localScale.x / halfNumber, colliderTransform.localScale.z);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x + 0.24f, ratio * go.transform.localPosition.y - offset, go.transform.localPosition.z);
                Text txt = go.GetComponentInChildren<Text>();
                txt.fontSize = (int)(txt.fontSize * ratio / 2f);
            }
        }
    }

    */ 
}
