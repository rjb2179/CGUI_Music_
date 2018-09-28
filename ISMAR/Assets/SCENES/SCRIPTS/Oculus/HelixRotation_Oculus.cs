using UnityEngine;
using System.Collections;

public class HelixRotation_Oculus : MonoBehaviour
{
    // the main behaviour script of the Helix
    public Helix_Oculus myObjectScript;
    public HelixGlow glowScript;

    Vector3 rTouchPos; 
  
    // the position of the right touch controller when the rotation gesture is started
    private float rTouchInitialPosition;

    // the position of the right hand relative to its initial position
    private float rTouchRelativePosition;

    // The palm normal vectors of the hands
    private Vector3 leftPalmNormal, rightPalmNormal;

    // Booleans about the rotation status
    private bool wasRotating;
    private bool isRotating;

    // speed at which the albums are rotated
    public float rotationSensitivity = .5f;

    public void InitializeScript(Helix_Oculus script)
    {
        myObjectScript = script;
        glowScript = script.glowScript;
    }

    void Update()
    {

        OVRInput.Update(); 
       
        wasRotating = isRotating;
        isRotating = false;

        if (glowScript.highlightedForRotation)
        {
            // condition to enable rotation
            if (OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
            {
                isRotating = true;
            }
        }
        
        if (wasRotating != isRotating)
        {
            if (isRotating)
            {
                rTouchPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                rTouchInitialPosition = rTouchPos.x; 
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
                rTouchPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                rTouchRelativePosition =  rTouchPos.x - rTouchInitialPosition;
            }
        }

        if (isRotating)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        // we change the angle parameter of the Helix
        myObjectScript.currentRadianAngle += rTouchRelativePosition * rotationSensitivity;
    }
}
