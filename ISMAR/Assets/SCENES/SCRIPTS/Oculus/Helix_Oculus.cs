using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Helix_Oculus : MonoBehaviour
{
    // helix 
    public static int numberOfHelixesHighlighted = 0;

    // parameters of the helix 
    public float currentRadianAngle = 0f;
    private float previousRadianAngle = 0f;

    // constants of the helix
    private float pitch = .7f;
    private float radius = .1f;
    private float verticalProportionalDistanceBetweenAlbums = .9f;
    private int numberOfObjectsPerRevolution = 6;
    private float maximumAngle;

    // Spotify data 
    private int numberOfObjects;
    private MySpotify.SpotifyData spotifyData;

    private SpotifyAlbums_Oculus albumsSpotifyScript;
    private SpotifyPlaylists_Oculus playlistsSpotifyScript;
    public HelixGlow_Oculus glowScript;

    // quads in the helix
    private GameObject[] quads;

    void Update()
    { 
        if (previousRadianAngle != currentRadianAngle && Mathf.Abs(currentRadianAngle) < maximumAngle)
        {
            UpdateQuadsPositions();
        }
        else if (currentRadianAngle >= maximumAngle)
        {
            currentRadianAngle = maximumAngle;
        }
        else if (currentRadianAngle <= -maximumAngle)
        {
            currentRadianAngle = -maximumAngle;
        }
        previousRadianAngle = currentRadianAngle;
    }

    public void InitializeSpotifyHelix(MySpotify.SpotifyData data, float helixRadius)
    {
        radius = helixRadius;
        spotifyData = data;

        if (spotifyData.type.Equals("artist"))
        {
            albumsSpotifyScript = GameObject.FindGameObjectWithTag("InstructionSign").GetComponent<SpotifyAlbums_Oculus>();
            StartSpotifyAlbumsSearch();
        }

        if (spotifyData.type.Equals("category"))
        {
            playlistsSpotifyScript = GameObject.FindGameObjectWithTag("InstructionSign").GetComponent<SpotifyPlaylists_Oculus>();
            StartSpotifyPlaylistsSearch();
        }

        glowScript = gameObject.AddComponent<HelixGlow_Oculus>();
        gameObject.layer = LayerMask.NameToLayer("Interactible");
    }

    private void SetMaximumAngle()
    {
        maximumAngle = numberOfObjects * 2f * Mathf.PI / numberOfObjectsPerRevolution;
    }

    private void StartSpotifyAlbumsSearch()
    {
        albumsSpotifyScript.StartSpotifySearch(spotifyData.ID, this);
    }

    private void StartSpotifyPlaylistsSearch()
    {
        playlistsSpotifyScript.StartSpotifySearch(spotifyData.ID, this);
    }

    // display the helix 
    public void DisplaySpotifyObjects(List<string> ids, List<Texture2D> textures, string objectType, List<string> previewURLs = null)
    {
        numberOfObjects = textures.Count;
        quads = new GameObject[numberOfObjects];

        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject myObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

            // set the transform 
            myObject.transform.parent = transform;
            float yScale = pitch * verticalProportionalDistanceBetweenAlbums / (float)numberOfObjectsPerRevolution;
            float txtDimensionProportion = textures[i].width / (float)textures[i].height;
            myObject.transform.localScale = new Vector3(yScale * txtDimensionProportion, yScale, 1f);

            // make object interactible 
            InteractibleObject_Oculus interactibleScript = myObject.AddComponent<InteractibleObject_Oculus>();
            interactibleScript.idInHelix = i - numberOfObjects / 2;
            interactibleScript.objectType = objectType;
            interactibleScript.previewURL = previewURLs[i]; 
            myObject.GetComponent<MeshRenderer>().material.mainTexture = textures[i];

            SpotifySongs songsScript = myObject.AddComponent<SpotifySongs>();
            songsScript.objectID = ids[i];
            songsScript.objectType = objectType;

            // create copy on back of quad 
            GameObject copyOfMyObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            copyOfMyObject.transform.SetParent(myObject.transform, false);
            copyOfMyObject.transform.localScale = new Vector3(1f, 1f, 1f);
            copyOfMyObject.transform.position = myObject.transform.position;
            copyOfMyObject.transform.rotation = myObject.transform.rotation;
            copyOfMyObject.transform.Rotate(180 * myObject.transform.up);
            copyOfMyObject.transform.position += 0.001f * myObject.transform.forward;
            copyOfMyObject.GetComponent<MeshRenderer>().material.mainTexture = textures[i];
            interactibleScript.objectCopy = copyOfMyObject;

            quads[i] = myObject;
        }

        SetMaximumAngle();
        UpdateQuadsPositions();
        float halfHeightAccordingToCylinder = pitch * numberOfObjects / numberOfObjectsPerRevolution / 2;
        glowScript.InitializeGlow(radius, halfHeightAccordingToCylinder);
    }

    private void UpdateQuadsPositions()
    {
        for (int i = 0; i < quads.Length; i++)
        {
            InteractibleObject_Helix albumScript = quads[i].GetComponent<InteractibleObject_Helix>();
            if (!albumScript.hasBeenGrabbed)
            {
                float albumAngle = 2f * Mathf.PI * albumScript.idInHelix / (float)numberOfObjectsPerRevolution + currentRadianAngle;
                quads[i].transform.localPosition = ComputeHelixCoordinates(albumAngle);
                quads[i].transform.localRotation = ComputeHelixQuaternion(albumAngle);
            }
        }
    }

    private Vector3 ComputeHelixCoordinates(float theta)
    {
        float z = -radius * Mathf.Cos(theta);
        float x = radius * Mathf.Sin(theta);
        float y = pitch / (2f * Mathf.PI) * theta;

        Vector3 relativePosition = new Vector3(x, y, z);
        return relativePosition;
    }

    private Quaternion ComputeHelixQuaternion(float theta)
    {
        Vector3 outVector = new Vector3(radius * Mathf.Sin(theta), 0f, -radius * Mathf.Cos(theta));
        return Quaternion.LookRotation(-outVector);
    }
}
