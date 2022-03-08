using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDamagedJoint : MonoBehaviour
{
    [SerializeField] private Joint2D joint;

    private void Update()
    {
        if (!joint)
        {
            Destroy(gameObject);
        }
    }
}
