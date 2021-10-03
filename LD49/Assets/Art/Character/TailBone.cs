using System;
using UnityEngine;

public class TailBone : MonoBehaviour {

    // Transforms
    public Transform parentBone;
    public Transform tailBoneOrigin;
    public Transform tailBoneEnd;

    // Initial state (extracted up-front!)
    [SerializeField]
    private Vector3 extractedTailBoneLocalPosition;
    [SerializeField]
    private Quaternion extractedTailBoneLocalRotation;

    // Options
    public float tailBoneAngularAcceleration = 2f;
    public float tailBoneAngularDamping = 0.1f;
    public float pointDistance = 2f;
    public bool invert = false;
    
    // State
    private Vector3 trackedEndPosition;
    private Vector3 boneAngularVelocity;

    void Start() {
        trackedEndPosition = tailBoneEnd.transform.position;
        boneAngularVelocity = new Vector3(0, 0f, 0);
    }
    

    [ContextMenu("Extract Tail Details")]
    void ExtractTailAnimationData() {
        // Tail bone 1, parent it to Body
        Vector3 tailBone1GlobalPosition = tailBoneOrigin.transform.position;
        Quaternion tailBone1GlobalRotation = tailBoneOrigin.transform.rotation;

        extractedTailBoneLocalPosition = parentBone.InverseTransformPoint(tailBone1GlobalPosition);
        extractedTailBoneLocalRotation = Quaternion.Inverse(parentBone.rotation)*tailBone1GlobalRotation;
    }

    public void UpdateTailBone() {
        // Update tailbone position
        tailBoneOrigin.transform.position = parentBone.TransformPoint(extractedTailBoneLocalPosition);

        // Figure out tail "normal" rotation
        Quaternion tailBone1GlobalRotation = Quaternion.LookRotation(trackedEndPosition - tailBoneOrigin.transform.position, new Vector3(0, 1, 0)) * Quaternion.Inverse(Quaternion.LookRotation(new Vector3(0, 1, 0), new Vector3(-1, 0, 0)));

        // Find the difference between target & source rotations
        Quaternion targetLocation = parentBone.rotation*extractedTailBoneLocalRotation;


        // First find the angle around the Y axis
        float angleY = SignedAngleBetweenVectorsInPlane(targetLocation*Vector3.up, tailBone1GlobalRotation*Vector3.up, Vector3.up);
        Vector3 angularForceY = -angleY*Vector3.up;
        targetLocation=Quaternion.AngleAxis(angleY*Mathf.Rad2Deg, Vector3.up)*targetLocation;

        
        Debug.DrawLine(parentBone.transform.position, parentBone.transform.position + targetLocation*Vector3.up);
        Debug.DrawLine(parentBone.transform.position, parentBone.transform.position + tailBone1GlobalRotation*Vector3.up, Color.red);

        // Then find the angle around the cross axis
        Vector3 crossAxis = Vector3.Normalize(Vector3.Cross(tailBone1GlobalRotation*Vector3.up, Vector3.up));
        float angleCrossAxis = SignedAngleBetweenVectorsInPlane(targetLocation*Vector3.up, tailBone1GlobalRotation*Vector3.up, crossAxis);
        Vector3 angularForceCross = -angleCrossAxis*crossAxis;

        Vector3 angularForce = angularForceY + angularForceCross;
        boneAngularVelocity+=angularForce*tailBoneAngularAcceleration;
        

        // Apply Damping
        boneAngularVelocity*=1-tailBoneAngularDamping;

        // Ok, now actually apply the angular velocity
        Quaternion deltaRotation = Quaternion.Normalize(QuaternionDeltaRotation(boneAngularVelocity, Time.deltaTime));
        tailBone1GlobalRotation = deltaRotation*tailBone1GlobalRotation;
        tailBoneOrigin.transform.rotation = tailBone1GlobalRotation;

        // Remember tailBone1 end position
        trackedEndPosition = (tailBoneEnd.transform.position-tailBoneOrigin.transform.position).normalized*pointDistance+tailBoneOrigin.transform.position;
    }

    /**
     * Get the normalized signed angle between two vectors in a plane.
     * Assumes:
     * - planeNormal is normalized
     * - v1 and v2 have the same origin (so vectors) and ly in the plane
     *
     * Returns angle between -PI and PI
     */
    public static float SignedAngleBetweenVectorsInPlane(Vector3 v1, Vector3 v2, Vector3 planeNormal) {
        // Source: https://stackoverflow.com/questions/5188561/signed-angle-between-two-3d-vectors-with-same-origin-within-the-same-plane
        return Mathf.Atan2(Vector3.Dot(Vector3.Cross(v1, v2), planeNormal), Vector3.Dot(v1, v2));
    }

    

    // Source: https://stackoverflow.com/questions/24197182/efficient-quaternion-angular-velocity

    /** Convert "exponential map" (angular rotation) to quaternion  (PRECISE VERSION) */
    public static Quaternion QuaternionDeltaRotation(Vector3 em, float deltaTime) {
        Vector3 ha = em * deltaTime * 0.5f; // vector of half angle
        float l = Vector3.Magnitude(ha); // magnitude
        if (l > 0) {
            ha *= Mathf.Sin(l) / l;
            return new Quaternion(ha.x, ha.y, ha.z, Mathf.Cos(l));
        } else {
            return new Quaternion(ha.x, ha.y, ha.z, 1.0f);
        }
    }

    public static Quaternion QuaternionDeltaRotationAppx1(Vector3 em, float deltaTime) {
        Vector3 ha = em * deltaTime * 0.5f; // vector of half angle
        return new Quaternion(1.0f, ha.x, ha.y, ha.z);
    }
}