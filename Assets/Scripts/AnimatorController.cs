using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [HideInInspector] public bool grounded;
    [HideInInspector] public bool climb;
    [HideInInspector] public bool pushPull;
    [HideInInspector] public float pushPullDirection;

    private void Update()
    {
        animator.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VelocityY", rb.velocity.y);
        animator.SetFloat("AbsVelocityY", Mathf.Abs(rb.velocity.y));
        animator.SetBool("Grounded", grounded);
        animator.SetBool("Climb", climb);
        animator.SetBool("PushPull", pushPull);
        animator.SetFloat("PushPullDirection", pushPullDirection);
    }

    public void SetThrowObjectTrigger()
    {
        animator.SetTrigger("Throw");
    }

    public void Die()
    {
        animator.SetTrigger("Die");
    }


}
