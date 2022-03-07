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
    private bool _climbing;
    private bool _throwBlock;

    private void Awake()
    {
        movement = new Movement(rb, MoveSpeed, MoveSmoothTime, JumpForce, GroundCheckRadius, GroundLayer, Feet, ClimbMoveSpeed);
        pushPull = new PushPull(rb, PushPullCheckRadius, PushPullLayer, PushPullHands, PushPullJointPrefab);
        carryThrow = new CarryThrow(CarryCheckRadius, CarryLayer, CarryThrowHands, ThrowForce);
    }

    private void Update()
    {
        movement.inputX = !_throwBlock ? Input.GetAxis("Horizontal") : 0;
        movement.inputY = !_throwBlock ? Input.GetAxis("Vertical") : 0;

        if (!_throwBlock)
        {
            movement.jump = Input.GetButtonDown("Jump") ? true : movement.jump;
            carryThrow.carry = Input.GetButtonDown("PickUp") ? true : carryThrow.carry;
            pushPull.pushpull = Input.GetButton("PushPull");
            movement.startClimbing = (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W)) ? true : movement.startClimbing;

        }

        carryThrow.throwCarried = Input.GetButtonDown("Throw") ? true : carryThrow.throwCarried;

        animatorController.climb = _climbing;
    }

    private void FixedUpdate()
    {
        animatorController.grounded = Physics2D.OverlapCircle(Feet.position, GroundCheckRadius, GroundLayer);

        if ((pushPull.pushpull && (Mathf.Abs(rb.velocity.y) <= .005f) && movement.MoveSpeed != PushPullMoveSpeed) || ((!pushPull.pushpull || (Mathf.Abs(rb.velocity.y) >= .005f)) && movement.MoveSpeed != MoveSpeed))
            movement.ChangeMoveSpeed((pushPull.pushpull && (Mathf.Abs(rb.velocity.y) <= .005f)) ? PushPullMoveSpeed : MoveSpeed);

        movement.Move(pushPull.pushpull || animatorController.climb);
        movement.Jump();

        if (!pushPull.pushpull && !_climbing)
        {
            _carringObject = carryThrow.PickUp();
        }

        if (!_carringObject)
        {
            pushPull.Grab();
        }

        if (_carringObject && carryThrow.throwCarried)
            animatorController.SetThrowObjectTrigger();

        movement.jump = false;
        carryThrow.carry = false;
        carryThrow.throwCarried = false;

        animatorController.pushPull = pushPull.pushpull;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (ClimbLayer.ContainsLayer(collision.gameObject.layer) && !pushPull.pushpull)
        {
            _climbing = movement.Climb();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (ClimbLayer.ContainsLayer(collision.gameObject.layer))
            _climbing = movement.Climb(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PushPullHands.position, PushPullCheckRadius);
        Gizmos.DrawWireSphere(CarryThrowHands.position, CarryCheckRadius);
    }

    private void ThrowBlock(int block)
    {
        _throwBlock = block == 1;
    }

    private void ThrowObject()
    {
        _carringObject = carryThrow.Throw(transform.right) ? false : _carringObject;
    }
}
