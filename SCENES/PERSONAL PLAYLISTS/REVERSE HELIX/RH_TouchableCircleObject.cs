using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;

public class RH_TouchableCircleObject : MonoBehaviour
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

    public float touchDistance = 0.1f;
    public float glowDistance = 0.1f;
    private float interactibleDelayInSeconds = 2f;
    private float helixOffset = .25f;

    Material highlightMaterial;
    Material defaultMaterial;

    public GameObject reverseHelix; 

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
                        reverseHelix = GenerateReverseHelix();
                    }
                    else
                    {
                        DestroyImmediate(reverseHelix);
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

    private GameObject GenerateReverseHelix()
    {
        GameObject reverseHelix = new GameObject("reverseHelix " + spotifyData.name);

        // add helix of albums script 
        ReverseHelix script = reverseHelix.AddComponent<ReverseHelix>();
        script.InitializeReverseHelix(provider, leftPinchDetector, rightPinchDetector,
            spotifyData);

        return reverseHelix; 
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
