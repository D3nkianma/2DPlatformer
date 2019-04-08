using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce;
    public float moveSpeed;
    private Rigidbody2D myBody;

    private float horizontal;
    private float vertical;


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


    }


    private void FixedUpdate()
    {
        MoveHorizontal();

    }



    private void Jump()
    {
        myBody.AddForce(Vector2.up * jumpForce);
    }


    private void MoveHorizontal()
    {
        if (horizontal != 0f)
        {
            myBody.velocity = new Vector2(horizontal * moveSpeed, myBody.velocity.y);
        }
    }










}