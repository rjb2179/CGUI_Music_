using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity; 

public class ObjectRotation : MonoBehaviour {

    public LeapXRServiceProvider provider;

    private float rotationSensitivity = 4f;
    private float handsInitialPosition;
    private float handsRelativePosition = 0f;

    private bool isRotating = false; 

    private Frame currentFrame;

    void Update()
    {
        bool wasRotating = isRotating;
        isRotating = false; 
        Hand hand1 = null;
        Hand hand2 = null;
        currentFrame = provider.CurrentFrame;

        if (currentFrame != null && currentFrame.Hands.Count == 2)
        {
      
            foreach (Hand h in currentFrame.Hands)
            {
                if (h.IsLeft)
                    hand1 = h;
                else
                    hand2 = h;
            }

            bool LeftAndRight = (hand1 != null && hand2 != null);

            // both hands in fists 
            if (LeftAndRight && hand1.GrabStrength > 0.8 && hand2.GrabStrength > 0.8)
            {
                isRotating = true;
            }
        }

        if (wasRotating != isRotating)
        {
            if (isRotating)
            {
                handsInitialPosition = hand2.PalmPosition.x + hand1.PalmPosition.x / 2f;
            }
            else
            {
                handsRelativePosition = 0f;
            }
        }
        else
        {
            if (isRotating)
            {
                handsRelativePosition = hand2.PalmPosition.x + hand1.PalmPosition.x / 2f - handsInitialPosition;
            }
        }

        if (isRotating)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        GameObject target = GameObject.Find("Leap Rig");

        transform.RotateAround(target.transform.position, Vector3.up, handsRelativePosition * rotationSensitivity);  
    }
}
