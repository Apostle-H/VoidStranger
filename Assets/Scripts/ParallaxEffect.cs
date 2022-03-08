using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private float ParralacEffectOnX;

    private Transform cameraTransform;
    private Vector2 lastCameraPosition;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        Vector2 deltaMovement = (Vector2)cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * ParralacEffectOnX,  deltaMovement.y * .92f);
        lastCameraPosition = cameraTransform.position;
    }
}
