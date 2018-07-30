using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity; 

public class HelixRotation : MonoBehaviour {

    // The main behaviour script of the Helix
    public Helix myObjectScript;
    public HelixGlow glowScript; 

    // Leap provider from which we get the hand data
    public LeapXRServiceProvider provider;

    // The current frame containing the hands data
    private Frame currentFrame;
    private Hand leftHand, rightHand;

    // Boolean stating if both a right hand and a left hand are present in the current frame
    private bool leftAndRight;
    private float dotProductBetweenPalmNormals;

    // The position of the right hand when the rotation gesture is started
    private float rightHandInitialPosition;

    // The position of the right hand relative to its initial position
    private float rightHandRelativePosition;

    // The palm normal vectors of the hands
    private Vector3 leftPalmNormal, rightPalmNormal;

    // Booleans about the rotation status
    private bool wasRotating;
    private bool isRotating;

    // The speed at which the albums are rotated
    public float rotationSensitivity = .5f;

    public void InitializeScript(Helix script, LeapXRServiceProvider prov)
    {
        myObjectScript = script;
        glowScript = script.glowScript; 
        provider = prov;
    }

	void Update () {

        if (provider == null)
            return;

        wasRotating = isRotating;
        isRotating = false;
        leftHand = null;
        rightHand = null;

        if (glowScript.highlightedForRotation) {

            currentFrame = provider.CurrentFrame;

            if (currentFrame != null)
            {
                // both hands must be present
                if (currentFrame.Hands.Count == 2)
                {
                    foreach (Hand h in currentFrame.Hands)
                    {
                        if (h.IsLeft)
                            leftHand = h;
                        else
                            rightHand = h;
                    }

                    leftAndRight = (leftHand != null && rightHand != null);

                    // condition to enable rotation
                    if (leftAndRight && leftHand.GrabStrength > 0.8 && rightHand.GrabStrength < 0.1) 
                        isRotating = true;
                }
            }
        }

        if (wasRotating != isRotating)
        {
            if (isRotating)
            {
                rightHandInitialPosition = rightHand.PalmPosition.x;
                glowScript.SetGlowColorToGreen();
            }
            else
            {
                glowScript.SetGlowColorToDefault();
            }
        }
        else
        {
            if (isRotating)
            {
                rightHandRelativePosition = rightHand.PalmPosition.x - rightHandInitialPosition;
            }
        }

        if (isRotating)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        // We change the angle parameter of the Helix
        myObjectScript.currentRadianAngle += rightHandRelativePosition * rotationSensitivity;
    }
}
