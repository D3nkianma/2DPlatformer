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

    private bool usingDash;

    [Space(20)]
    public MoveState moveState = MoveState.Standing;

    private Rigidbody2D myBody;
    private AnimHelper animHelper;
    private SpriteRenderer spriteRenderer;
    private float horizontal;
    private float vertical;
    private int currentJumpCount;

    private Timer dashTimer;
    private Timer dashCooldownTimer;


    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        animHelper = GetComponent<AnimHelper>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        UpdateDash();
        CheckMoveState();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonDown("Dash"))
        {
            BeginDash();
        }

        if (jumpType == JumpType.Variable)
            VariableFall();
    }


    private void FixedUpdate()
    {
        MoveHorizontal();
        CheckGround();
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

        if (myBody.velocity.y < 0)
        {
            desiredFallVelocity = Vector2.up * Physics2D.gravity.y * descendingFallMod * Time.deltaTime;
        }
        else if (myBody.velocity.y > 0 && Input.GetButton("Jump") == false)
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

        if (horizontal != 0f)
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

        switch (moveState)
        {
            case MoveState.Standing:

                animHelper.StopWalk();

                break;

            case MoveState.Running:
                SetFacing();
                desiredSpeed = moveSpeed;

                if (isGrounded)
                    animHelper.PlayWalk();

                break;

            case MoveState.Dashing:
                SetFacing();
                desiredSpeed = dashSpeed;

                break;
        }

        myBody.velocity = new Vector2(horizontal * desiredSpeed, myBody.velocity.y);


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
    }

    private void EndDash()
    {
        SetMoveState(MoveState.Standing);
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        int otherLayer = collision.gameObject.layer;
        string layerName = LayerMask.LayerToName(otherLayer);

        if (layerName != "Ground")
            return;

        ResetJump();
    }


    private void SetFacing()
    {
        if (horizontal < 0 && spriteRenderer.flipX == false)
        {
            spriteRenderer.flipX = true;
        }

        if (horizontal > 0 && spriteRenderer.flipX == true)
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










}