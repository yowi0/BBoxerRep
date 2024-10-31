using System.Collections;
using EzySlice;
using UnityEngine;
using UnityEngine.InputSystem;

public class SliceObject : MonoBehaviour
{
    public Transform startSlicePoint;
    public Transform endSlicePoint;
    public VelocityEstimator VelocityEstimator;
    public LayerMask sliceableLayer;

    public Material crossSectionMaterial;
    public float cutForce = 2000;

    void Start()
    {
        if (VelocityEstimator == null)
        {
            Debug.LogError("VelocityEstimator component is missing.");
            enabled = false; // Disable this script if VelocityEstimator is not set
        }
    }

    void FixedUpdate()
    {
        if (VelocityEstimator != null)
        {
            bool hasHit = Physics.Linecast(startSlicePoint.position, endSlicePoint.position, out RaycastHit hit, sliceableLayer);
            if (hasHit)
            {
                GameObject target = hit.transform.gameObject;
                Slice(target);
            }
        }
    }

    public void Slice(GameObject target)
    {
        Vector3 velocity = VelocityEstimator.GetVelocityEstimate();
        Vector3 planeNormal = Vector3.Cross(endSlicePoint.position - startSlicePoint.position, velocity);
        planeNormal.Normalize();

        SlicedHull hull = target.Slice(endSlicePoint.position, planeNormal);

        if (hull != null)
        {
            GameObject upperHull = hull.CreateUpperHull(target, crossSectionMaterial);
            SetupSlicedComponent(upperHull, target.transform.position);

            GameObject lowerHull = hull.CreateLowerHull(target, crossSectionMaterial);
            SetupSlicedComponent(lowerHull, target.transform.position);

            target.SetActive(false);

            StartCoroutine(DestruirFragmentos(upperHull, lowerHull, 2f));
        }
    }

    IEnumerator DestruirFragmentos(GameObject upperHull, GameObject lowerHull, float tiempoEspera)
    {
        yield return new WaitForSeconds(tiempoEspera);

        Destroy(upperHull);
        Destroy(lowerHull);
    }

    public void SetupSlicedComponent(GameObject slicedObject, Vector3 originalPosition)
    {
        Rigidbody rb = slicedObject.AddComponent<Rigidbody>();
        MeshCollider collider = slicedObject.AddComponent<MeshCollider>();
        collider.convex = true;
        rb.AddExplosionForce(cutForce, slicedObject.transform.position, 1);

        slicedObject.transform.position = originalPosition;
    }
}
