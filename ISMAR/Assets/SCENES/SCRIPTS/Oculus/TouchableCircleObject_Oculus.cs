using UnityEngine;
using System.Collections;

public class TouchableCircleObject_Oculus : MonoBehaviour
{ 
    // data from Spotify
    public MySpotify.SpotifyData spotifyData;

    // helix 
    private GameObject helix;

    // floats 
    public float touchDistance = 0.1f;
    public float glowDistance = 0.1f;
    private float interactibleDelayInSeconds = 2f;
    private float helixOffset = .5f;

    // bools 
    private bool unfolded = false;
    private bool isInteractible = true;

    // mats 
    Material highlightMaterial;
    Material defaultMaterial;

    private void Start()
    {
        Debug.Log(UnityEngine.Input.GetJoystickNames());
        LoadMats(); 
    }

    private void LoadMats()
    {
        highlightMaterial = Resources.Load("Materials/AdditiveRim/AdditiveYellowShader", typeof(Material)) as Material;
        defaultMaterial = gameObject.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {

        OVRInput.Update();

        Vector3 LTouchPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Vector3 RTouchPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        float distanceToObjectLTouch = Vector3.Distance(transform.position, LTouchPos);
        float distanceToObjectRTouch = Vector3.Distance(transform.position, RTouchPos);

        if (distanceToObjectLTouch < glowDistance || distanceToObjectRTouch < glowDistance)
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
            if (distanceToObjectLTouch < touchDistance || distanceToObjectRTouch < touchDistance)
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
        Helix_Oculus script = helix.AddComponent<Helix_Oculus>();
        script.InitializeSpotifyHelix(spotifyData, transform.localScale.x * 0.8f);

        // add helix of rotation script 
        HelixRotation_Oculus rotationScript = helix.AddComponent<HelixRotation_Oculus>();
        rotationScript.InitializeScript(script); 

        return helix;
    } 

    // set the position and rotation of an artistHelix game object
    private void SetHelixTransform(GameObject helix)
    {
        // helix in the middle 
        GameObject GO = GameObject.Find("STONES");
        helix.transform.position = GO.transform.position + (helixOffset * GO.transform.forward);
        helix.transform.rotation = Quaternion.LookRotation(transform.forward, transform.up);
        helix.transform.parent = GO.transform;

        /* helix.transform.position = this.transform.position + (helixOffset * - this.transform.forward); 
        helix.transform.rotation = Quaternion.LookRotation(transform.up, transform.up);
        helix.transform.parent = this.transform; */
    }
} 
