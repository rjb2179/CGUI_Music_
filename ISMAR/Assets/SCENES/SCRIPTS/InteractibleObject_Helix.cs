using UnityEngine;
using System.Collections;
using Leap; 
using Leap.Unity;
using NAudio; 
using NAudio.Wave;

public class InteractibleObject_Helix : MonoBehaviour
{
    // pinch detectors 
    public MyPinchDetector leftPinchDetector;
    public MyPinchDetector rightPinchDetector;

    public GameObject objectCopy;
    public GameObject hasPrevURL;

    public AudioSource source;
    public Material greenHighlightMaterial;
    public Material redHighlightMaterial;

    private Vector3 initialScale;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private Transform parent;

    public string objectType;
    public string previewURL;

    public int idInHelix;

    public enum RotationMethod
    {
        None,
        Single,
        Axis,
        Full
    }
    private RotationMethod oneHandedRotationMethod = RotationMethod.Axis;

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

    private float distanceToPinch = 0.1f;
    private float distanceToTouch = 0.05f;
    private float distanceToScale = 0.1f;
    private float ScalingDelay = 0.5f;
    private float previousDistanceBtwPinches = 0f;
    private float distanceBtwPinches = 0f;
    private float scalingFactor = 1f;
    private float scalingSpeed = 3f;
    private float snapTriggerHeight = 0.4f;

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
    
        if (leftPinchDetector == null || rightPinchDetector == null)
        {
            return;
        }

        if (scalingEnabled && hasBeenGrabbed)
        {
            wasScaling = isScaling;
            isScaling = false;

            if (leftPinchDetector.IsPinching && rightPinchDetector.IsPinching)
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

            if (rightPinchDetector.IsPinching)
            {
                float distanceToObject = Vector3.Distance(rightPinchDetector.Position, transform.position);

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
                    TransformSinglePinch(rightPinchDetector);
                }
            }

            if (leftPinchDetector.IsPinching)
            {
                float distanceToObject = Vector3.Distance(leftPinchDetector.Position, transform.position);

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
                    TransformSinglePinch(leftPinchDetector);
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


    private void TransformSinglePinch(MyPinchDetector singlePinch)
    {
        // follow the pinching object 
        transform.position = singlePinch.Position;

        switch (oneHandedRotationMethod)
        {
            case RotationMethod.None:
                break;
            case RotationMethod.Single:
                Vector3 p = singlePinch.Rotation * Vector3.right;
                p.y = transform.position.y;
                transform.LookAt(p);
                break;
            case RotationMethod.Full:
                transform.rotation = singlePinch.Rotation;
                break;
            case RotationMethod.Axis:
                if (wasPinching)
                {
                    transform.RotateAround(transform.position, transform.up, singlePinch.Rotation.eulerAngles.y - singlePinch.LastRotation.eulerAngles.y);
                }
                break;
        }
    }

    private void ScaleDoublePinch()
    {
        previousDistanceBtwPinches = distanceBtwPinches;
        Vector3 centerOfPinch = (leftPinchDetector.Position + rightPinchDetector.Position) / 2.0f;
        distanceBtwPinches = (leftPinchDetector.Position - rightPinchDetector.Position).magnitude;
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

