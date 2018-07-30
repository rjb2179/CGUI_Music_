using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;

public class CurvedGrid : MonoBehaviour
{
    // Leap provider 
    public LeapXRServiceProvider provider;

    // pinch detectors 
    private MyPinchDetector leftPinchDetector;
    private MyPinchDetector rightPinchDetector;

    // ??? 
    private int numberOfObjects;
    private MySpotify.SpotifyData spotifyData;
    private GRID_MySpotifyPlaylistsTracks mySpotifyPlaylistsTracksScript;

    // quads in the grid 
    private GameObject[] quads;

    // helix glow script 
    public HelixGlow glowScript;

    void Update()
    {
        if (provider == null)
        {
            return;
        }
        
        // Need my own version of UpdateQuadPositions() 
    }

    public void InitializeCurvedGrid(LeapXRServiceProvider prov, MyPinchDetector left, MyPinchDetector right, 
        MySpotify.SpotifyData data)
    {
        provider = prov;
        leftPinchDetector = left;
        rightPinchDetector = right;
        spotifyData = data;

        mySpotifyPlaylistsTracksScript = GameObject.FindGameObjectWithTag("InstructionSign").GetComponent<GRID_MySpotifyPlaylistsTracks>();
        StartMySpotifyPlaylistsTracksSearch();
    }

    private void StartMySpotifyPlaylistsTracksSearch()
    {
        mySpotifyPlaylistsTracksScript.StartSpotifySearch(spotifyData.ID, this);
    }

    // display the curved grid 
    public void DisplaySpotifyObjects(List<string> ids, List<Texture2D> textures, string objectType, List<string> previewURLs = null)
    {
        numberOfObjects = textures.Count;
        quads = new GameObject[numberOfObjects];

        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject myObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

            // set the transform and localScale 
            myObject.transform.parent = transform;
            myObject.transform.localScale = new Vector3(0.3f, 0.1f, 0.1f);
    
            myObject.GetComponent<MeshRenderer>().material.mainTexture = textures[i];

            quads[i] = myObject;
        }

        UpdateQuadsPositions(); 
    }

    private void UpdateQuadsPositions()
    {
        for (int i = 0; i < quads.Length; i++)
        {
            quads[i].transform.localPosition = new Vector3(0, i/10, 0);
        }
    }
}
