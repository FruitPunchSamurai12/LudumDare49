using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterGrounding : MonoBehaviour
{
    [SerializeField]
    Transform[] positions;

    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private LayerMask layerMask;

    private Transform groundedObject;
    private Vector3? groundedObjectLastPosition;
    public bool IsGrounded { get; private set; }
    public Vector3 GroundedDirection { get; private set; }
    PressurePlate pp;

    // Update is called once per frame
    void Update()
    {
        foreach (var pos in positions)
        {
            CheckFootForGrounding(pos);
            if (IsGrounded)
                break;
        }
        StickToMovingObjects();
    }

    void StickToMovingObjects()
    {
        if (groundedObject != null)
        {
            if (groundedObjectLastPosition.HasValue && groundedObjectLastPosition.Value != groundedObject.position)
            {
                Vector3 delta = groundedObject.position - groundedObjectLastPosition.Value;
                transform.position += delta;
            }
            groundedObjectLastPosition = groundedObject.position;
        }
        else
        {
            groundedObjectLastPosition = null;
        }
    }

    private void CheckFootForGrounding(Transform foot)
    {
        if (Physics.Raycast(foot.position, foot.forward,out RaycastHit hitInfo, maxDistance, layerMask))
        {
            if (groundedObject != hitInfo.collider.transform)
            {
                groundedObjectLastPosition = hitInfo.collider.transform.position;
            }
            if(!IsGrounded)
            {
                AudioManager.Instance.PlaySoundEffect("Land", transform.position);
            }
            IsGrounded = true;
            GroundedDirection = foot.forward;
            groundedObject = hitInfo.collider.transform;
            if(groundedObject.CompareTag("PressurePlate"))
            {
                pp = groundedObject.GetComponent<PressurePlate>();
                pp.PlayerOnTop();
            }
            else
            {
                if(pp!=null)
                {
                    pp.PlayerLeft();
                    pp = null;
                }
            }
        }
        else
        {
            if (pp != null)
            {
                pp.PlayerLeft();
                pp = null;
            }
            groundedObject = null;
            IsGrounded = false;
        }
    }
}
