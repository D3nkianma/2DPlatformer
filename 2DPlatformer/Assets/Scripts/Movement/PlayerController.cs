using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public enum JumpType {
        Standard,
        Variable
    }

    public enum MoveState {
        Standing,
        Running,
        Dashing
    }

    public enum FacingDirection {
        Left,
        Right
    }

    public FacingDirection Facing { get { return GetFacing(); } }

    public JumpType jumpType = JumpType.Standard;
    [Header("Temp Jump Variables")]
    public float jumpForce = 1200f;
    public float arialJumpForce = 500f;
    public float moveSpeed;
    public int maxJumpCount = 1;

    [Header("Variable Jump Variables")]
    public float descendingFallMod = 1.5f;
    public float ascendingFallMod = 1f;

    [Header("Dash Variables")]
    public float dashDuration = 0.5f;
    public float dashSpeed = 30f;
    public float dashCooldown = 3f;


    [Header("Layer Masks")]
    public LayerMask groundLayer;

    [Header("Misc")]
    public Transform groundFinder;

    [Header("Flags")]
    public bool isGrounded;
    public bool isHittingWall;

    private bool usingDash;
    private AttackManager attackManager;

    [Space(20)]
    public MoveState moveState = MoveState.Standing;

    private Rigidbody2D myBody;
    private AnimHelper animHelper;
    
    private SpriteRenderer spriteRenderer;
    //private float horizontal;
    //private float vertical;
    private int currentJumpCount;

    private Timer dashTimer;
    private Timer dashCooldownTimer;


    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        animHelper = GetComponent<AnimHelper>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackManager = GetComponent<AttackManager>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        dashTimer = new Timer(dashDuration, EndDash, true);
        dashCooldownTimer = new Timer(dashCooldown, RefreshDash, true);
    }


    private void Update()
    {
        UpdateDash();
        CheckMoveState();
        DetectWall();

        if (GameInput.Jump)
        {
            Jump();
        }

        if (GameInput.Dash)
        {
            BeginDash();
        }

        if (GameInput.Fire1)
            Attack();

        if (jumpType == JumpType.Variable)
            VariableFall();
    }


    private void FixedUpdate()
    {
        MoveHorizontal();
        CheckGround();
    }


    private void Attack()
    {
        animHelper.StartAnimTrigger("Punch");
        attackManager.LaunchAttack(Facing, CalcXKnockback(15f), 15f);
    }

    private float CalcXKnockback(float xForce)
    {
        if (Facing == FacingDirection.Left)
            return xForce * -1f;
        else
            return xForce;
    }

    private void Jump()
    {
        float desiredJumpForce = isGrounded ? jumpForce : arialJumpForce;

        if (currentJumpCount >= maxJumpCount)
        {
            return;
        }

        animHelper.PlayOrStopAnimBool("Jumping", true);

        myBody.velocity = new Vector2(myBody.velocity.x, 0f);

        myBody.AddForce(Vector2.up * desiredJumpForce);

        currentJumpCount++;
    }

    private void VariableFall()
    {
        Vector2 desiredFallVelocity = Vector2.zero;

        if (moveState == MoveState.Dashing)
        {
            myBody.velocity = new Vector2(myBody.velocity.x, 0f);
        }
        else if (myBody.velocity.y < 0)
        {
            desiredFallVelocity = Vector2.up * Physics2D.gravity.y * descendingFallMod * Time.deltaTime;
        }
        else if (myBody.velocity.y > 0 && GameInput.JumpHeld == false)
        {
            desiredFallVelocity = Vector2.up * Physics2D.gravity.y * ascendingFallMod * Time.deltaTime;
        }

        myBody.velocity += desiredFallVelocity;
    }

    private void CheckMoveState()
    {

        if (moveState == MoveState.Dashing)
        {
            return;
        }

        if (GameInput.Horizontal != 0f && isHittingWall == false)
        {
            SetMoveState(MoveState.Running);
        }
        else
        {
            SetMoveState(MoveState.Standing);
        }
    }

    private void MoveHorizontal()
    {
        float desiredSpeed = 0f;
        float desiredFacing = 0f;

        switch (moveState)
        {
            case MoveState.Standing:

                animHelper.StopWalk();
                desiredFacing = GameInput.Horizontal;
                break;

            case MoveState.Running:
                SetFacing();
                desiredSpeed = moveSpeed;
                desiredFacing = GameInput.Horizontal;

                if (isGrounded)
                    animHelper.PlayWalk();

                break;

            case MoveState.Dashing:
                SetFacing();
                desiredSpeed = dashSpeed;

                desiredFacing = Facing == FacingDirection.Left ? -1 : 1;

                break;
        }

        myBody.velocity = new Vector2(desiredFacing * desiredSpeed, myBody.velocity.y);
    }

    private void DetectWall()
    {
        Vector2 direction = Facing == FacingDirection.Left ? Vector2.left : Vector2.right;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, groundLayer);

        //Debug.DrawRay(transform.position, direction, Color.red);

        if (hit.collider != null && isGrounded == false && myBody.velocity.y <= 0f)
        {
            isHittingWall = true;
        }
        else
        {
            isHittingWall = false;
        }
    }

    private void UpdateDash()
    {
        if (dashCooldownTimer != null && usingDash == true)
            dashCooldownTimer.UpdateClock();

        if (dashTimer != null && moveState == MoveState.Dashing)
            dashTimer.UpdateClock();
    }

    private void BeginDash()
    {
        if (usingDash == true)
            return;


        usingDash = true;
        SetMoveState(MoveState.Dashing);
        animHelper.PlayOrStopAnimBool("Dashing", true);
    }

    private void EndDash()
    {
        SetMoveState(MoveState.Standing);
        animHelper.PlayOrStopAnimBool("Dashing", false);
    }

    private void RefreshDash()
    {
        Debug.Log("Refreshing Dash");
        usingDash = false;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundFinder.transform.position, 0.1f, groundLayer);
    }

    private void ResetJump()
    {
        if (currentJumpCount > 0 && isGrounded == true)
        {
            currentJumpCount = 0;
            animHelper.PlayOrStopAnimBool("Jumping", false);
        }
    }


    private void OnCollisionEnter2D(Collision2D other)
    {

        if (LayerTools.IsLayerInMask(groundLayer, other.gameObject.layer) == false)
            return;

        //    int otherLayer = collision.gameObject.layer;
        //string layerName = LayerMask.LayerToName(otherLayer);

        //if (layerName != "Ground")
        //    return;

        ResetJump();
    }


    private void SetFacing()
    {
        if (GameInput.Horizontal < 0 && spriteRenderer.flipX == false)
        {
            spriteRenderer.flipX = true;
        }

        if (GameInput.Horizontal > 0 && spriteRenderer.flipX == true)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void SetMoveState(MoveState state)
    {
        //Debug.Log(state + " is being set");

        if (moveState != state)
            moveState = state;
    }

    public FacingDirection GetFacing()
    {
        return spriteRenderer.flipX ? FacingDirection.Left : FacingDirection.Right;
    }




}