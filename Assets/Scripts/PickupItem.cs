using UnityEngine;
public class PickupItem : MonoBehaviour
{
    public float pickupRange = 3f;
    public float holdDistance = 2f;
    public float moveSmoothness = 10f;
    public float throwForce = 500f;

    private GameObject heldItem;
    private Rigidbody heldItemRb;
    private Quaternion relativeRotation;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldItem == null)
            {
                TryPickupItem();
            }
            else
            {
                ReleaseItem();
            }
        }
    }

    void FixedUpdate()
    {
        if (heldItem != null)
        {
            HoldItem();
        }
    }

    void TryPickupItem()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickupRange))
        {
            if (hit.transform.CompareTag("Pickup"))
            {
                heldItem = hit.transform.gameObject;
                heldItemRb = heldItem.GetComponent<Rigidbody>();

                if (heldItemRb != null)
                {
                    heldItemRb.useGravity = false;
                    heldItemRb.freezeRotation = true;
                    heldItemRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    heldItemRb.interpolation = RigidbodyInterpolation.Interpolate;
                    relativeRotation = Quaternion.Inverse(Camera.main.transform.rotation) * heldItem.transform.rotation;
                }
            }
        }
    }

    void HoldItem()
    {
        Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * holdDistance;

        Vector3 direction = (targetPosition - heldItemRb.position);
        float distance = direction.magnitude;

        heldItemRb.velocity = direction.normalized * moveSmoothness * distance;

        Quaternion targetRotation = Camera.main.transform.rotation * relativeRotation;
        heldItemRb.MoveRotation(Quaternion.Slerp(heldItemRb.rotation, targetRotation, moveSmoothness * Time.fixedDeltaTime));
    }

    void ReleaseItem()
    {
        if (heldItemRb != null)
        {
            heldItemRb.useGravity = true;
            heldItemRb.freezeRotation = false;
            heldItemRb.velocity = Vector3.zero;
            heldItemRb = null;
        }
        heldItem = null;
    }
}
