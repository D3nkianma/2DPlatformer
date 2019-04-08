using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Temp Jump Variables")]
    public float jumpForce = 1200f;
    public float arialJumpForce = 500f;
    public float moveSpeed;
    public int maxJumpCount = 1;

    [Header("Layer Masks")]
    public LayerMask groundLayer;

    [Header("Misc")]
    public Transform groundFinder;

    [Header("Flags")]
    public bool isGrounded;


    private Rigidbody2D myBody;
    private float horizontal;
    private float vertical;
    private int currentJumpCount;


    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
         horizontal = Input.GetAxis("Horizontal");
         vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        //ResetJump();
    }


    private void FixedUpdate()
    {
        MoveHorizontal();
        CheckGround();
    }



    private void Jump()
    {

        float desiredJumpForce = isGrounded ? jumpForce : arialJumpForce;


        if(currentJumpCount >= maxJumpCount)
        {
            return;
        }

        myBody.velocity = new Vector2(myBody.velocity.x, 0f);

        myBody.AddForce(Vector2.up * desiredJumpForce);

        currentJumpCount++;


    }


    private void MoveHorizontal()
    {
        if (horizontal != 0f)
        {
            myBody.velocity = new Vector2(horizontal * moveSpeed, myBody.velocity.y);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundFinder.transform.position, 0.1f, groundLayer);
    }

    private void ResetJump()
    {
        if (currentJumpCount > 0 && isGrounded == true)
        {
            Debug.Log("Resetting Jump Count");
            currentJumpCount = 0;
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










}