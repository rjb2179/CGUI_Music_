using UnityEngine;
using System.Collections;
using NAudio;
using NAudio.Wave;

public class InteractibleObject_Oculus : MonoBehaviour
{
    // game objects 
    public GameObject objectCopy;
    public GameObject hasPrevURL;

    // audio source 
    public AudioSource source;

    // mats 
    public Material greenHighlightMaterial;
    public Material redHighlightMaterial;

    private Vector3 initialScale;
    private Vector3 originalPos;
    private Vector3 LTouchPos;
    private Vector3 RTouchPos;
    private Quaternion originalRot;
    private Transform parent;

    // strings 
    public string objectType;
    public string previewURL;

    // ints 
    public int idInHelix;

    public enum RotationMethod
    {
        None,
        Single,
        Axis,
        Full
    }
    private RotationMethod oneHandedRotationMethod = RotationMethod.Axis;

    // bools 
    public bool hasBeenGrabbed = false;
    private bool scalingEnabled = true;
    private bool wasPinching = false;
    private bool isPinching = false;
    private bool initialPinch = true;
    private bool firstLeftPinch = true;
    private bool lightSwitch = true;
    private bool wasScaling = false;
    private bool isScaling = false;
    private bool scalingBlocking = false;
    private bool hasSourceClip = false;
    private bool play = true;

    // floats 
    private float distanceToPinch = 0.1f;
    private float distanceToTouch = 0.05f;
    private float distanceToScale = 0.1f;
    private float ScalingDelay = 0.5f;
    private float previousDistanceBtwPinches = 0f;
    private float distanceBtwPinches = 0f;
    private float scalingFactor = 1f;
    private float scalingSpeed = 3f;
    private float snapTriggerHeight = 0.4f;
    private float LTouchIndex;
    private float RTouchIndex;
    private float LTouchHand;
    private float RTouchHand; 

    public void Start()
    {
        initialScale = transform.localScale;
        parent = transform.parent;
        source = gameObject.AddComponent<AudioSource>();

        if (!(previewURL == null))
        {
            hasPrevURL = Instantiate(GameObject.Find("Play Icon"));
            hasPrevURL.transform.SetParent(gameObject.transform, false);
            hasPrevURL.transform.position = gameObject.transform.position;
            hasPrevURL.transform.rotation = gameObject.transform.rotation;
            hasPrevURL.transform.position -= 0.001f * gameObject.transform.forward;
            hasPrevURL.transform.position += 0.031f * gameObject.transform.right;
            hasPrevURL.transform.position += 0.031f * gameObject.transform.up;
        }
    }

    public void Update()
    {

        if (scalingEnabled && hasBeenGrabbed)
        {
            wasScaling = isScaling;
            isScaling = false;

            LTouchIndex = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            RTouchIndex = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

            // no idea what these values will be 
            Debug.Log(LTouchIndex);
            Debug.Log(RTouchIndex); 

            if (LTouchIndex > 0.5 && RTouchIndex > 0.5)
            {
                ScaleDoublePinch();
            }

            if (wasScaling && !isScaling && !scalingBlocking)
            {
                scalingBlocking = true;
                StartCoroutine(UnblockScaling(ScalingDelay));
            }
        }

        wasPinching = isPinching;
        isPinching = false;

        if (!scalingBlocking)
        {
            LTouchIndex = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            RTouchIndex = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            LTouchHand = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
            RTouchHand = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);

            Debug.Log(LTouchIndex);
            Debug.Log(RTouchIndex);
            Debug.Log(LTouchHand);
            Debug.Log(RTouchHand); 

            // both triggers being held down 
            if (RTouchIndex > 0.5 && RTouchHand > 0.5)
            {
                RTouchPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

                float distanceToObject = Vector3.Distance(RTouchPos, transform.position);

                // pinching a specific album 
                if (distanceToObject < distanceToPinch)
                {
                    if (initialPinch)
                    {
                        originalPos = transform.position;
                        originalRot = transform.rotation;
                    }

                    initialPinch = false;
                    isPinching = true;
                    TransformSinglePinch(OVRInput.Controller.RTouch);
                }
            }

            if (LTouchIndex > 0.5 && LTouchHand > 0.5)
            {
                LTouchPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);

                float distanceToObject = Vector3.Distance(LTouchPos, transform.position);

                // pinching a specific album 
                if (distanceToObject < distanceToPinch)
                {
                    if (initialPinch)
                    {
                        originalPos = transform.position;
                        originalRot = transform.rotation;
                    }

                    initialPinch = false;
                    isPinching = true;
                    TransformSinglePinch(OVRInput.Controller.LTouch);
                }
            }

            if (wasPinching != isPinching)
            {
                if (isPinching && !hasBeenGrabbed)
                {
                    gameObject.layer = LayerMask.NameToLayer("Interactible");
                    gameObject.tag = "Grabbed";
                    hasBeenGrabbed = true;

                    if (objectType.Equals("track") && previewURL != null)
                    {
                        StartCoroutine(GetSourceClip());
                    }
                }
            }
        }
    }

    public void Play()
    {
        source.Play();
    }

    public void StopPlay()
    {
        source.Stop();
    }

    public void ReturnToHelix()
    {
        hasBeenGrabbed = false;
        gameObject.tag = "Untagged";
        transform.position = originalPos;
        transform.rotation = originalRot;
        transform.localScale = initialScale;
        transform.parent = parent;
    }

    public void SaveSong()
    {
        Debug.Log(gameObject.GetComponent<SpotifySongs>().objectID);
    }

    private void TransformSinglePinch(OVRInput.Controller controller)
    {
        // follow the pinching object 
        transform.position = OVRInput.GetLocalControllerPosition(controller);

        switch (oneHandedRotationMethod)
        {
            case RotationMethod.None:
                break;
            case RotationMethod.Axis:
                if (wasPinching)
                {
                    Quaternion controllerRot = OVRInput.GetLocalControllerRotation(controller);
                    // this may need to change 
                    transform.RotateAround(transform.position, transform.up, controllerRot.eulerAngles.y);
                }
                break;
        }
    }

    private void ScaleDoublePinch()
    {
        previousDistanceBtwPinches = distanceBtwPinches;

        LTouchPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        RTouchPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        Vector3 centerOfPinch = (LTouchPos + RTouchPos) / 2.0f;
        distanceBtwPinches = (LTouchPos - RTouchPos).magnitude;
        float distanceToObject = Vector3.Distance(centerOfPinch, transform.position);

        if (distanceToObject < distanceToScale)
        {
            if (wasScaling)
            {
                scalingFactor = 1f + (distanceBtwPinches - previousDistanceBtwPinches) * scalingSpeed;
                transform.localScale *= scalingFactor;
            }

            isScaling = true;
        }
    }

    private IEnumerator GetSourceClip()
    {
        WWW wwwPrevURL = new WWW(previewURL);
        yield return wwwPrevURL;

        source = gameObject.AddComponent<AudioSource>();
        source.clip = NAudioPlayer.FromMp3Data(wwwPrevURL.bytes);
    }

    private IEnumerator UnblockScaling(float delay)
    {
        yield return new WaitForSeconds(delay);
        scalingBlocking = false;
        yield return null;
    }
}
