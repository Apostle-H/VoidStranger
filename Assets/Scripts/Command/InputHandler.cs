using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AnimatorController animatorController;

    [Header("Move")]
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float MoveSmoothTime;

    [Header("Jump")]
    [SerializeField] private float JumpForce;
    [SerializeField] private float GroundCheckRadius;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private Transform Feet;

    [Header("Climb")]
    [SerializeField] private float ClimbMoveSpeed;
    [SerializeField] private LayerMask ClimbLayer;

    [Header("PushPull")]
    [SerializeField] private float PushPullMoveSpeed;

    [SerializeField] private float PushPullCheckRadius;
    [SerializeField] private LayerMask PushPullLayer;
    [SerializeField] private Transform PushPullHands;

    [SerializeField] private GameObject PushPullJointPrefab;

    [Header("CarryThrow")]
    [SerializeField] private float CarryCheckRadius;
    [SerializeField] private LayerMask CarryLayer;
    [SerializeField] private Transform CarryThrowHands;

    [SerializeField] private GameObject CarryJointPrefab;

    [SerializeField] Vector2 ThrowForce;

    private Movement movement;
    private PushPull pushPull;
    private CarryThrow carryThrow;

    private bool _carringObject;

    private void Awake()
    {
        movement = new Movement(rb, MoveSpeed, MoveSmoothTime, JumpForce, GroundCheckRadius, GroundLayer, Feet, ClimbMoveSpeed);
        pushPull = new PushPull(rb, PushPullCheckRadius, PushPullLayer, PushPullHands, PushPullJointPrefab);
        carryThrow = new CarryThrow(CarryCheckRadius, CarryLayer, CarryThrowHands, CarryJointPrefab, ThrowForce);
    }

    private void Update()
    {
        movement.inputX = Input.GetAxis("Horizontal");
        movement.jump = Input.GetButtonDown("Jump") ? true : movement.jump;

        movement.inputY = Input.GetAxis("Vertical");
        movement.startClimbing = (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W)) ? true : movement.startClimbing;

        pushPull.pushpull = Input.GetButton("PushPull");

        carryThrow.carry = Input.GetButtonDown("PickUp") ? true : carryThrow.carry;
        carryThrow.throwCarried = Input.GetButtonDown("Throw") ? true : carryThrow.throwCarried;

        if (carryThrow.throwCarried)
            animatorController.SetThrowObjectTrigger();
    }

    private void FixedUpdate()
    {
        if ((pushPull.pushpull && movement.MoveSpeed != PushPullMoveSpeed) || (!pushPull.pushpull && movement.MoveSpeed != MoveSpeed))
            movement.ChangeMoveSpeed(pushPull.pushpull ? PushPullMoveSpeed : MoveSpeed);

        movement.Move(pushPull.pushpull || animatorController.climb);
        movement.Jump();

        if (!pushPull.pushpull)
        {
            _carringObject = carryThrow.PickUp();
            //_carringObject = carryThrow.Throw(transform.right) ? false : _carringObject;
        }

        if (!_carringObject)
        {
            pushPull.Grab();
        }

        movement.jump = false;
        carryThrow.carry = false;
        carryThrow.throwCarried = false;

        animatorController.pushPull = pushPull.pushpull;
        animatorController.carry = _carringObject;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (ClimbLayer.ContainsLayer(collision.gameObject.layer) && !pushPull.pushpull)
        {
             animatorController.climb = movement.Climb();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (ClimbLayer.ContainsLayer(collision.gameObject.layer))
            animatorController.climb = movement.Climb(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PushPullHands.position, PushPullCheckRadius);
        Gizmos.DrawWireSphere(CarryThrowHands.position, CarryCheckRadius);
    }

    private void ThrowObject()
    {
        _carringObject = carryThrow.Throw(transform.right) ? false : _carringObject;
    }
}
