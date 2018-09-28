using UnityEngine;
using System.Collections;
using Leap;

public class LeapUtils
{

    public static Vector3 convertVectorToVector3(Vector v)
    {
        Vector3 v2 = new Vector3(v.x, v.y, v.z);
        return v2;
    }
}
