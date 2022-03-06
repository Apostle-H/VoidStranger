using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    public bool climb;
    public bool pushPull;
    public bool carry;

    private void Update()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VelocityY", rb.velocity.y);
        animator.SetFloat("AbsVelocityY", Mathf.Abs(rb.velocity.y));
        animator.SetBool("Climb", climb);
        animator.SetBool("PushPull", pushPull);
        animator.SetBool("Carry", carry);
    }

    public void SetThrowObjectTrigger()
    {
        animator.SetTrigger("Throw");
    }
}
