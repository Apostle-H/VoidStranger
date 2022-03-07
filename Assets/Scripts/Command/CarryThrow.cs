using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarryThrow
{
    private float CheckRadius;
    private LayerMask CheckLayer;
    private Transform Hands;

    private Vector2 ThrowForce;

    public bool carry;
    public bool throwCarried;

    private Rigidbody2D _carriedRb;

    public CarryThrow(float _cr, LayerMask _cl, Transform _h, Vector2 _tf)
    {
        CheckRadius = _cr;
        CheckLayer = _cl;
        Hands = _h;
        ThrowForce = _tf;
    }

    public bool PickUp()
    {
        if (!carry && _carriedRb) return true;
        else if (!carry) return false;

        if (_carriedRb) { Drop(); return false; }

        _carriedRb = Physics2D.OverlapCircle(Hands.position, CheckRadius, CheckLayer)?.attachedRigidbody;
        if (!_carriedRb) return false;

        _carriedRb.gameObject.SetActive(false);

        return true;
    }

    private void Drop()
    {
        _carriedRb.transform.position = Hands.position;
        _carriedRb.gameObject.SetActive(true);

        _carriedRb = null;
    }

    public bool Throw(Vector2 forward)
    {
        if (!_carriedRb) return false;

        _carriedRb.transform.position = Hands.position;
        _carriedRb.gameObject.SetActive(true);
        _carriedRb.AddForce(new Vector2(ThrowForce.x * forward.x, ThrowForce.y), ForceMode2D.Impulse);

        _carriedRb = null;

        return true;
    }
}
