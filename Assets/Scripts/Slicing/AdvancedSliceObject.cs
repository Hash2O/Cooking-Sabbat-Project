//using UnityEngine;
//using EzySlice;
//using Valve.VR.InteractionSystem;

//public class AdvancedSliceObject : MonoBehaviour
//{
//    public Transform startSlicePoint;
//    public Transform endSlicePoint;
//    public VelocityEstimator velocityEstimator;
//    public Material crossSectionMaterials;
//    public float moreCutForce = 200;
//    public LayerMask sliceableLayer;

//    // Update is called once per frame
//    void FixedUpdate()
//    {
//        bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
//        if (hasHit)
//        {
//            GameObject target = hit.transform.gameObject;
//            // Trying to get the material from the object being sliced
//            //crossSectionMaterials = target.GetComponent<Material>();
//            SliceAndDice(target);
//        }
//    }

//    public void SliceAndDice(GameObject target)
//    {
//        Vector3 velocity = velocityEstimator.GetVelocityEstimate();
//        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
//        planeNormal.Normalize();

//        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

//        if (hull != null)
//        {
//            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterials);
//            SetupSlicedComponents(upperHull);
//            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterials);
//            SetupSlicedComponents(lowerHull);
//            Destroy(target);
//        }
//    }

//    public void SetupSlicedComponents(GameObject slicedObject)
//    {
//        slicedObject.layer = 7;
//        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
//        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
//        IngredientBehaviour ingredient = slicedObject.AddComponent<IngredientBehaviour>();
//        //if (slicedObject.GetComponent<IngredientBehaviour>() != null) ingredient.data = slicedObject.GetComponent<IngredientBehaviour>().data;
//        collider.convex = true;
//        //rb.AddExplosionForce(moreCutForce, slicedObject.transform.position, 1);
//    }
//}

using UnityEngine;
using EzySlice;
using Valve.VR.InteractionSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AdvancedSliceObject : MonoBehaviour
{
    [Header("Slicing References")]
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator velocityEstimator;
    public Material crossSectionMaterials;
    public LayerMask sliceableLayer;
    public ParticleSystem slicingVFX;

    [Header("Physics Settings")]
    public float sliceForce = 200f;

    private bool isCutting = false;

    void FixedUpdate()
    {
        if (isCutting) return;

        if (Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer))
        {
            GameObject target = hit.transform.gameObject;
            IngredientBehaviour ingredient = target.GetComponent<IngredientBehaviour>();

            if (ingredient != null) // on ne découpe que les objets reconnus comme ingrédients
            {
                isCutting = true;
                SliceAndDice(target, ingredient);
            }
        }
    }

    private void SliceAndDice(GameObject target, IngredientBehaviour originalIngredient)
    {
        Vector3 velocity = velocityEstimator != null ? velocityEstimator.GetVelocityEstimate() : Vector3.forward;
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity).normalized;

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterials);
            SetupSlicedComponents(upperHull, originalIngredient);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterials);
            SetupSlicedComponents(lowerHull, originalIngredient);

            // On détruit l'objet original
            Destroy(target);
        }

        if (slicingVFX != null) slicingVFX.Play();

        // petit délai avant de pouvoir recouper autre chose
        Invoke(nameof(ResetCutting), 0.2f);
    }

    private void SetupSlicedComponents(GameObject slicedPart, IngredientBehaviour parentIngredient)
    {
        slicedPart.tag = "Ingredient";
        slicedPart.layer = 6;

        Rigidbody rb = slicedPart.AddComponent<Rigidbody>();
        MeshCollider collider = slicedPart.AddComponent<MeshCollider>();
        collider.convex = true;

        XRGrabInteractable grabInteractable = slicedPart.AddComponent<XRGrabInteractable>();
        grabInteractable.useDynamicAttach = true;

        // Hériter du comportement d’ingrédient
        IngredientBehaviour newIngredient = slicedPart.AddComponent<IngredientBehaviour>();

        if (parentIngredient != null)
        {
            newIngredient.data = parentIngredient.data; // Héritage de la RecipeData
        }

        // Optionnel : légère poussée pour séparer visuellement les morceaux
        //rb.AddExplosionForce(sliceForce, slicedPart.transform.position, 1f);
    }

    private void ResetCutting()
    {
        isCutting = false;
    }
}
