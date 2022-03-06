using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryThrow
{
    private float CheckRadius;
    private LayerMask CheckLayer;
    private Transform Hands;
    private GameObject JointObjectPrefab;

    private Vector2 ThrowForce;

    public bool carry;
    public bool throwCarried;

    private GameObject _jointObjectInstance;
    private Rigidbody2D _carriedRb;

    public CarryThrow(float _cr, LayerMask _cl, Transform _h, GameObject _jop, Vector2 _tf)
    {
        CheckRadius = _cr;
        CheckLayer = _cl;
        Hands = _h;
        JointObjectPrefab = _jop;
        ThrowForce = _tf;
    }

    public bool PickUp()
    {
        if (!carry && _jointObjectInstance) return true;
        else if (!carry) return false;
        if (_jointObjectInstance)
        {
            Drop();
            return false;
        }

        Rigidbody2D rbToCarry = Physics2D.OverlapCircle(Hands.position, CheckRadius, CheckLayer)?.attachedRigidbody;
        if (!rbToCarry) return false;

        _jointObjectInstance = Object.Instantiate(JointObjectPrefab, Hands);
        FixedJoint2D jointInstance = _jointObjectInstance.GetComponent<FixedJoint2D>();
        jointInstance.connectedBody = rbToCarry;

        _carriedRb = rbToCarry;

        return true;
    }

    private void Drop()
    {
        Object.Destroy(_jointObjectInstance);
    }

    public bool Throw(Vector2 forward)
    {
        if (!throwCarried) return false;
        _carriedRb.AddForce(new Vector2(ThrowForce.x * forward.x, ThrowForce.y), ForceMode2D.Impulse);
        Object.Destroy(_jointObjectInstance);

        return true;
    }
}
