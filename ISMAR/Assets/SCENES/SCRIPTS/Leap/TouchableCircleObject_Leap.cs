using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;

public class TouchableCircleObject_Leap : MonoBehaviour
{ 
    private bool unfolded = false;
    private bool isInteractible = true;

    // data retrieved from spotify
    public MySpotify.SpotifyData spotifyData;

    // Leap 
    public LeapXRServiceProvider provider;
    public MyPinchDetector rightPinchDetector;
    public MyPinchDetector leftPinchDetector;

    // helix 
    private GameObject helix; 

    public float touchDistance = 0.1f;
    public float glowDistance = 0.1f; 
    private float interactibleDelayInSeconds = 2f;
    private float helixOffset = .5f;

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

        /* if (provider == null)
        {
            return;
        }

        Frame currentFrame = provider.CurrentFrame;

        if (currentFrame == null)
        {
            return;
        } */

        if (Input.GetKeyDown("1"))
        {
            gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
        }
        else
        {
            if (!unfolded)
            {
                gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
        }

        if (isInteractible)
        {
            if (Input.GetKeyDown("2"))
            {
                unfolded = !unfolded;

                if (unfolded)
                {
                    helix = GenerateHelix();
                    gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                }
                else
                {
                    DestroyImmediate(helix);
                    gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
                }

                isInteractible = false;
                StartCoroutine(EnableInteractibilityAfterDelay(interactibleDelayInSeconds));
            }
        }


        /* foreach (Hand h in currentFrame.Hands)
        {

        Debug.Log(h);

        float distanceToAlbum = Vector3.Distance(transform.position, LeapUtils.convertVectorToVector3(h.PalmPosition));

        if (distanceToAlbum < glowDistance)
        {
            gameObject.GetComponent<MeshRenderer>().material = highlightMaterial; 
        }
        else
        {
            if (!unfolded)
            {
                gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
        }

        if (isInteractible)
        {
            if (distanceToAlbum < touchDistance)
            {
                unfolded = !unfolded;

                if (unfolded)
                {
                    helix = GenerateHelix();
                    gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
                }
                else
                {
                    DestroyImmediate(helix);
                    gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
                }

                isInteractible = false;
                StartCoroutine(EnableInteractibilityAfterDelay(interactibleDelayInSeconds));
            }
        }
    }
} 

        */ 
    } 

    public IEnumerator EnableInteractibilityAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        isInteractible = true;
    }

    private GameObject GenerateHelix()
    {
        GameObject helix = new GameObject("Helix " + spotifyData.name);

        SetHelixTransform(helix);

        // add helix of albums script 
        Helix script = helix.AddComponent<Helix>();
        script.InitializeSpotifyHelixFakeScreen(provider, leftPinchDetector, rightPinchDetector, 
            spotifyData, this.transform, transform.localScale.x * 0.8f);

        // add helix of rotation script 
        HelixRotation rotationScript = helix.AddComponent<HelixRotation>();
        rotationScript.InitializeScript(script, provider);

        return helix;
    }

    // set the position and rotation of an artistHelix game object
    private void SetHelixTransform(GameObject helix)
    {
        // For Helix in the middle 
        GameObject GO = GameObject.Find("STONES");
        helix.transform.position = GO.transform.position + (helixOffset * GO.transform.forward);
        helix.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        helix.transform.parent = GO.transform; 

        /* helix.transform.position = this.transform.position + (helixOffset * - this.transform.forward);
        helix.transform.rotation = Quaternion.LookRotation(transform.up, transform.up);
        helix.transform.parent = this.transform; */ 
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