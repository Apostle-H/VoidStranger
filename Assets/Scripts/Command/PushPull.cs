using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPull
{
    //Components
    private Rigidbody2D rb;

    //Pushpull
    private float CheckRadius;
    private LayerMask CheckLayer;
    private Transform Hands;
    private GameObject JointObjectPrefab;

    public bool pushpull;

    private GameObject _jointObjectInstance;

    public PushPull(Rigidbody2D _rb, float _cr, LayerMask _cl, Transform _h, GameObject _jop)
    {
        rb = _rb;
        CheckRadius = _cr;
        CheckLayer = _cl;
        Hands = _h;
        JointObjectPrefab = _jop;
    }

    public void Grab()
    {
        if (!pushpull || Mathf.Abs(rb.velocity.y) > .1f)
        {
            Release();
            return;
        }

        if (_jointObjectInstance) return;

        Rigidbody2D rbToConnect = Physics2D.OverlapCircle(Hands.position, CheckRadius, CheckLayer)?.attachedRigidbody;
        if (rbToConnect == null || rbToConnect.velocity.magnitude > .02f) return;

        _jointObjectInstance = Object.Instantiate(JointObjectPrefab, Hands);
        DistanceJoint2D jointInstance = _jointObjectInstance.GetComponent<DistanceJoint2D>();
        jointInstance.connectedBody = rbToConnect;

        return;
    }

    public void Release()
    {
        Object.Destroy(_jointObjectInstance);
    }
}
