using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    [Header("Public")]
    public bool running;
    public Vector2 runDirection;// Note: +x = forward, +y = right

    public bool inair;
    public float angleLookUp;
    
    public void KickBack() {
        animator.SetTrigger("Kickback");
    }

    [Header("Internal")]
    public Animator animator;

    public Transform upBone;

    public TailBone bone1;
    public TailBone bone2;

    // Update is called once per frame
    void LateUpdate()
    {
        animator.SetFloat("Forward", runDirection.x);
        animator.SetFloat("Right", runDirection.y);
        animator.SetBool("Moving", running);
        animator.SetBool("InAir", inair);

        // Angle Look "up"
        upBone.localRotation = Quaternion.AngleAxis(angleLookUp, Vector3.back);

        // Update the tail bone!
        if(bone1 != null) {
            bone1.UpdateTailBone();
        }
        if(bone2 != null) {
            bone2.UpdateTailBone();
        }
    }
}
