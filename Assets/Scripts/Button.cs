using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Animator target;
    [SerializeField] private string enableBoxTag;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.CompareTag(enableBoxTag));
        if (collision.gameObject.CompareTag(enableBoxTag))
        {
            target.SetTrigger("Activate");
        }
    }
}
