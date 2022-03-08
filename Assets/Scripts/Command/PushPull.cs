using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPull
{
    //Components
    private Rigidbody2D rb;
    private Collider2D playerCollider;

    //Pushpull
    private float CheckRadius;
    private LayerMask CheckLayer;
    private Transform Hands;
    private GameObject JointObjectPrefab;

    public bool pushpull;

    private GameObject _jointObjectInstance;
    private GameObject _pushingPullingObject;

    public PushPull(Rigidbody2D _rb, Collider2D _pc, float _cr, LayerMask _cl, Transform _h, GameObject _jop)
    {
        rb = _rb;
        playerCollider = _pc;
        CheckRadius = _cr;
        CheckLayer = _cl;
        Hands = _h;
        JointObjectPrefab = _jop;
    }

    public void Grab()
    {
        if ((!pushpull || Mathf.Abs(rb.velocity.y) > .1f) && _pushingPullingObject)
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
        _pushingPullingObject = rbToConnect.gameObject;

        Physics2D.IgnoreCollision(playerCollider, _pushingPullingObject.GetComponent<Collider2D>());

        return;
    }

    public void Release()
    {
        Physics2D.IgnoreCollision(playerCollider, _pushingPullingObject.GetComponent<Collider2D>(), false);
        Object.Destroy(_jointObjectInstance);
    }
}
