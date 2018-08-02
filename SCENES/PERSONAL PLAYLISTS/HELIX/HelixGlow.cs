using UnityEngine;
using System.Collections;


public class HelixGlow : GazeSelectionTarget {

    private float helixRadius;
    private float halfHeight;

    // glow
    private GameObject projectorGlow;
    private float factorOnBase = 3f;
    private float defaultHeightOfGlow = 0.4f;
    private Material greenHighlightMaterial;
    private Material defaultMaterial;

    // collider
    private CapsuleCollider capsule;
    private float relativeRadiusOfCapsule = 1.2f;

    public bool highlightedForRotation = true;

    public void InitializeGlow(float helixRad, float halfHeightForCylinder)
    {
        helixRadius = helixRad;
        halfHeight = Mathf.Max(halfHeightForCylinder,defaultHeightOfGlow);

        capsule = gameObject.AddComponent<CapsuleCollider>();
        SetCollider();

        projectorGlow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SetGlowTransform();
        SetGlowMaterials();
    }

    private void SetCollider()
    {
        capsule.radius = helixRadius * relativeRadiusOfCapsule;
        capsule.height = 2 * halfHeight;
        capsule.direction = 1;
        capsule.center = new Vector3(0f, 0f, 0f);
    }

    private void SetGlowTransform()
    {
        projectorGlow.transform.parent = this.transform;
        projectorGlow.transform.position = transform.position + (halfHeight - defaultHeightOfGlow) * this.transform.up;  
        projectorGlow.transform.rotation = transform.rotation;
        projectorGlow.transform.localScale = new Vector3(factorOnBase * helixRadius, halfHeight * 2, factorOnBase * helixRadius);
    }

    private void SetGlowMaterials()
    {

        projectorGlow.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Transparent", typeof(Material)) as Material;
        greenHighlightMaterial = Resources.Load("Materials/AdditiveRim/AdditiveGreenShader", typeof(Material)) as Material;
        defaultMaterial = projectorGlow.GetComponent<MeshRenderer>().material;
    }

    public void SetGlowColorToGreen()
    {
        projectorGlow.GetComponent<MeshRenderer>().material = greenHighlightMaterial;
    }

    public void SetGlowColorToDefault()
    {
        projectorGlow.GetComponent<MeshRenderer>().material = defaultMaterial;
    }


}
