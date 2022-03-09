using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Ending : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachine;
    [SerializeField] private Transform Drenei;

    [SerializeField] private GameObject finalScreen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cinemachine.Follow = Drenei;
            Destroy(collision.gameObject);

            StartCoroutine("End");
        }
    }

    private IEnumerator End()
    {
        yield return new WaitForSeconds(4.5f);
        finalScreen.SetActive(true);
    }
}
