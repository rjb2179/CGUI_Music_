using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;

public class GRID_TouchableCircleObject : MonoBehaviour
{
    private bool unfolded = false;
    private bool isInteractible = true;

    // data retrieved from spotify
    public MySpotify.SpotifyData spotifyData;

    // Leap provider 
    public LeapXRServiceProvider provider;

    // pinch detectors 
    public MyPinchDetector rightPinchDetector;
    public MyPinchDetector leftPinchDetector;

    // curved grid 
    private GameObject curvedGrid; 

    public float touchDistance = 0.1f;
    public float glowDistance = 0.1f;
    private float interactibleDelayInSeconds = 2f;
    private float helixOffset = .25f;

    Material highlightMaterial;
    Material defaultMaterial;

    private void Start()
    {
        AddPinchDetection();
        highlightMaterial = Resources.Load("Materials/AdditiveRim/AdditiveYellowShader", typeof(Material)) as Material;
        defaultMaterial = gameObject.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {

        if (provider == null)
        {
            return;
        }

        Frame currentFrame = provider.CurrentFrame;

        if (currentFrame == null)
        {
            return;
        }

        foreach (Hand h in currentFrame.Hands)
        {
            float distanceToAlbum = Vector3.Distance(transform.position, LeapUtils.convertVectorToVector3(h.PalmPosition));

            if (distanceToAlbum < glowDistance)
            {
                gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }

            if (isInteractible)
            {
                if (distanceToAlbum < touchDistance)
                {
                    unfolded = !unfolded;

                    if (unfolded)
                    {
                        curvedGrid = GenerateCurvedGrid();
                    }
                    else
                    {
                        DestroyImmediate(curvedGrid);
                    }

                    isInteractible = false;
                    StartCoroutine(EnableInteractibilityAfterDelay(interactibleDelayInSeconds));
                }
            }
        }
    }

    public IEnumerator EnableInteractibilityAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isInteractible = true;
    }

    private GameObject GenerateCurvedGrid()
    {
        GameObject curvedGrid = new GameObject("Helix " + spotifyData.name);

        SetGridTransform(curvedGrid);

        // add curved grid script 
        CurvedGrid script = curvedGrid.AddComponent<CurvedGrid>();
        script.InitializeCurvedGrid(provider, leftPinchDetector, rightPinchDetector, spotifyData);

        // add curved grid rotation script 

        return curvedGrid;
    }

    // set the position and rotation of an artistHelix game object
    private void SetGridTransform(GameObject helix)
    {

        GameObject GO = GameObject.Find("Leap Rig");
        helix.transform.position = GO.transform.position + (helixOffset * GO.transform.forward);
        
        // helix.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        
        helix.transform.parent = GO.transform;
    }

    private void AddPinchDetection()
    {
        GameObject pinchManager = GameObject.FindGameObjectWithTag("PinchManager");

        leftPinchDetector = pinchManager.AddComponent<MyPinchDetector>();
        leftPinchDetector.leftHand = true;
        leftPinchDetector.provider = provider;

        rightPinchDetector = pinchManager.AddComponent<MyPinchDetector>();
        rightPinchDetector.leftHand = false;
        rightPinchDetector.provider = provider;
    }
}
