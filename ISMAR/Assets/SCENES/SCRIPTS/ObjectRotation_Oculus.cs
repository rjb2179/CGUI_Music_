using UnityEngine;
using System.Collections;

public class ObjectRotation_Oculus : MonoBehaviour
{

    private float rotationSensitivity = 4f;
    private float handsInitialPosition;
    private float handsRelativePosition = 0f;

    private bool isRotating = false;

    void Update()
    {

    }

    private void Rotate()
    {
        GameObject target = GameObject.Find("STONES");
        transform.RotateAround(target.transform.position, Vector3.up, handsRelativePosition * rotationSensitivity);
    }
}
