using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;

public class ReverseHelix : MonoBehaviour
{
    // Leap provider 
    public LeapXRServiceProvider provider;

    // pinch detectors 
    private MyPinchDetector leftPinchDetector;
    private MyPinchDetector rightPinchDetector;

    // parameters of the helix 
    public float currentRadianAngle = 0f;
    private float previousRadianAngle = 0f;

    // constants of the helix
    private float pitch = .7f;
    private float radius = 0.5f;
    private float verticalProportionalDistanceBetweenAlbums = 0.9f;
    private int numberOfObjectsPerRevolution = 6;
    private float maximumAngle;

    private int numberOfObjects;
    private MySpotify.SpotifyData spotifyData;
    private RH_MySpotifyPlaylistsTracks mySpotifyPlaylistsTracksScript;

    // quads in the reverse helix 
    private GameObject[] quads;
    private GameObject reverseHelix; 

    void Update()
    { 

            if (provider == null)
            {
                return;
            }

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

    public void InitializeReverseHelix(LeapXRServiceProvider prov, MyPinchDetector left, MyPinchDetector right, 
        MySpotify.SpotifyData data)
    {
        provider = prov;
        leftPinchDetector = left;
        rightPinchDetector = right;
        spotifyData = data;

        reverseHelix = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        SetTransform();
        // SetMaterial(); 

        mySpotifyPlaylistsTracksScript = GameObject.FindGameObjectWithTag("InstructionSign").GetComponent<RH_MySpotifyPlaylistsTracks>();
        StartMySpotifyPlaylistsTracksSearch();
    }

    private void StartMySpotifyPlaylistsTracksSearch()
    {
        mySpotifyPlaylistsTracksScript.StartSpotifySearch(spotifyData.ID, this);
    }

    public void DisplaySpotifyObjects(List<string> ids, List<Texture2D> textures, string objectType, List<string> previewURLs = null)
    {
        numberOfObjects = textures.Count;
        quads = new GameObject[numberOfObjects];

        for (int i = 0; i < numberOfObjects; i++)
        {
            GameObject myObject = GameObject.CreatePrimitive(PrimitiveType.Quad);

            // set the transform and localScale 
            myObject.transform.parent = reverseHelix.transform;
            myObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            myObject.GetComponent<MeshRenderer>().material.mainTexture = textures[i];

            RH_InteractibleObject interactibleScript = myObject.AddComponent<RH_InteractibleObject>();
            if (i % 2 == 0)
            {
                interactibleScript.idInHelix = i / 2;
            } else
            {
                interactibleScript.idInHelix = -1 * (i / 2 + 1);
            }
            interactibleScript.leftPinchDetector = leftPinchDetector;
            interactibleScript.rightPinchDetector = rightPinchDetector;
            interactibleScript.objectType = objectType;
            interactibleScript.previewURL = previewURLs[i];

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
    }

    private void SetTransform()
    {
        reverseHelix.transform.position = GameObject.Find("Leap Rig").transform.position + (1f * transform.forward); 
        reverseHelix.transform.localScale = new Vector3(1, 1, 1);
        reverseHelix.transform.rotation = transform.rotation;
        reverseHelix.transform.parent = transform; 
    }

    private void SetMaterial()
    {
        reverseHelix.GetComponent<MeshRenderer>().material = Resources.Load("Materials/AdditiveRim/AdditiveGreenShader", typeof(Material)) as Material;
    }

    private void SetMaximumAngle()
    {
        maximumAngle = numberOfObjects * 2f * Mathf.PI / numberOfObjectsPerRevolution;
    }

    private void UpdateQuadsPositions()
    {
        for (int i = 0; i < quads.Length; i++)
        {
            RH_InteractibleObject albumScript = quads[i].GetComponent<RH_InteractibleObject>();
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
