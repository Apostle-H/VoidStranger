using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private Image hasBox;

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
    [SerializeField] private PhysicsMaterial2D NonFrictionMaterail;

    [Header("CarryThrow")]
    [SerializeField] private float CarryCheckRadius;
    [SerializeField] private LayerMask CarryLayer;
    [SerializeField] private Transform PickUpHands;
    [SerializeField] private Transform ThrowHands;

    [SerializeField] Vector2 ThrowForce;

    [Header("Death")]
    [SerializeField] private string respawTag;
    [SerializeField] private string deathTag;

    [Header("Portal")]
    [SerializeField] private string portalTag;
    [SerializeField] private float portalCheckRadius;

    private Movement movement;
    private PushPull pushPull;
    private CarryThrow carryThrow;
    private Death death;

    private bool _carringObject;
    private bool _climbing;
    private bool _inputBlock;
    private bool _tep;

    private void Awake()
    {
        movement = new Movement(rb, MoveSpeed, MoveSmoothTime, JumpForce, GroundCheckRadius, GroundLayer, Feet, ClimbMoveSpeed);
        pushPull = new PushPull(rb, playerCollider, PushPullCheckRadius, PushPullLayer, PushPullHands, PushPullJointPrefab, NonFrictionMaterail);
        carryThrow = new CarryThrow(CarryCheckRadius, CarryLayer, PickUpHands, ThrowHands, ThrowForce);
        death = new Death(gameObject);
    }

    private void Update()
    {
        movement.inputX = !_inputBlock ? Input.GetAxis("Horizontal") : 0;
        animatorController.hasInputX = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D));
        animatorController.inputX = Input.GetAxis("Horizontal");
        movement.inputY = !_inputBlock ? Input.GetAxis("Vertical") : 0;

        if (!_inputBlock)
        {
            movement.jump = Input.GetButtonDown("Jump") ? true : movement.jump;
            carryThrow.carry = Input.GetButtonDown("PickUp") ? true : carryThrow.carry;
            pushPull.pushpull = Input.GetButton("PushPull");
            if (pushPull.pushpull)
                animatorController.pushPullDirection = movement.facingRight ? (movement.inputX > 0 ? 1 : -1) : (movement.inputX < 0 ? 1 : -1);
            movement.startClimbing = (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W)) ? true : movement.startClimbing;
            _tep = Input.GetKeyDown(KeyCode.W) ? true : _tep;
        }

        if (Physics2D.OverlapCircle(Feet.position, GroundCheckRadius, GroundLayer))
        {
            carryThrow.throwCarried = Input.GetButtonDown("Throw") ? true : carryThrow.throwCarried;
        }

        animatorController.climb = _climbing;

        hasBox.enabled = _carringObject;
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

        if (_tep)
        {
            Portal portal;
            Collider2D[] PortalOrNotColliders = Physics2D.OverlapCircleAll(transform.position, portalCheckRadius);
            foreach (var collider in PortalOrNotColliders)
            {
                if (collider.CompareTag(portalTag) && collider.TryGetComponent(out portal))
                {
                    transform.position = portal.otherPortal.position;
                }
            }
        }

        pushPull.Grab();
        
        if (_carringObject && carryThrow.throwCarried)
            animatorController.SetThrowObjectTrigger();

        movement.jump = false;
        carryThrow.carry = false;
        carryThrow.throwCarried = false;
        _tep = false;

        animatorController.pushPull = pushPull.pushpull;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(deathTag))
        {
            animatorController.Die();
        }

        if (collision.CompareTag(respawTag))
        {
            death.NewCheckPoint(collision.transform.position);
        }
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
        Gizmos.DrawWireSphere(PickUpHands.position, CarryCheckRadius);
    }

    private void Block(int block)
    {
        _inputBlock = block == 1;
    }

    private void ThrowObject()
    {
        _carringObject = carryThrow.Throw(transform.right * (movement.facingRight ? 1 : -1)) ? false : _carringObject;
    }

    private void Respawn()
    {
        death.Respawn();
    }
}
