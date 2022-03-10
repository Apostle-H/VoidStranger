using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private Animator target;
    [SerializeField] private string enableBoxTag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(enableBoxTag))
        {
            target.SetTrigger("Activate");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(enableBoxTag))
        {
            target.SetTrigger("Activate");
        }
    }
}
