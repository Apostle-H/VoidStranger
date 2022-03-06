using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    //Components
    private Rigidbody2D rb;

    //Move
    public float MoveSpeed { get; private set; }
    private float MoveSmoothTime;

    //Jump
    private float JumpForce;
    private float GroundCheckRadius;
    private LayerMask GroundLayer;
    private Transform Feet;

    //Climb
    private float ClimbMoveSpeed;

    [HideInInspector] public float inputX;
    [HideInInspector] public float inputY;
    [HideInInspector] public bool jump;
    [HideInInspector] public bool startClimbing;

    private Vector2 _currentVelocity;
    private Vector2 _targetVelocity;

    private bool _climb;
    private float _gravityScaleSave;

    private bool _facingRight;

    public Movement(Rigidbody2D _rb, float _ms, float _mst, float _jf, float _gcr, LayerMask _gl, Transform _f, float _cms)
    {
        rb = _rb;
        MoveSpeed = _ms;
        MoveSmoothTime = _mst;
        JumpForce = _jf;
        GroundCheckRadius = _gcr;
        GroundLayer = _gl;
        Feet = _f;
        ClimbMoveSpeed = _cms;
    }

    public void Move(bool CanFlip)
    {
        if (((!_facingRight && inputX < -.1f) || (_facingRight && inputX > .1f)) && !CanFlip)
            Flip();

        _targetVelocity = new Vector2(inputX * (_climb ? ClimbMoveSpeed : MoveSpeed), _climb ? (inputY * ClimbMoveSpeed) : rb.velocity.y);
        rb.velocity = Vector2.SmoothDamp(rb.velocity, _targetVelocity, ref _currentVelocity, MoveSmoothTime);
    }

    public void Jump()
    {
        if (jump && Physics2D.OverlapCircle(Feet.position, GroundCheckRadius, GroundLayer))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
        }
        else if (jump && _climb)
            StopClimbing();
    }

    public bool Climb(bool jumpOff = false)
    {
        if (Mathf.Abs(inputY) > .1f && !_climb && startClimbing && !jumpOff)
            StartClimbing();
        
        if (_climb && jumpOff)
            StopClimbing();

        return _climb;
    }

    private void StartClimbing()
    {
        _gravityScaleSave = rb.gravityScale;
        rb.gravityScale = 0;

        _climb = true;
        startClimbing = false;
    }

    private void StopClimbing()
    {
        rb.gravityScale = _gravityScaleSave;

        _climb = false;
    }

    private void Flip()
    {
        _facingRight = !_facingRight;
        rb.transform.Rotate(0, 180, 0);
    }

    public void ChangeMoveSpeed(float newMoveSpeed)
    {
        MoveSpeed = newMoveSpeed;
    }
}
