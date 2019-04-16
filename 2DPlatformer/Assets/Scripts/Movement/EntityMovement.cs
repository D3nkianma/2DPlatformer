using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{

    public Rigidbody2D myBody { get; private set; }

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }


    public void ForceMovement(Vector2 force, float duration = 0f)
    {
        myBody.velocity += force;
    }



    public void SpinCrazy()
    {
        float randongRotSpeed = Random.Range(-360f, 360f);
        float randomY = Random.Range(250f, 350f);

        Vector2 force = new Vector2(myBody.velocity.x, randomY);
        myBody.freezeRotation = false;
        myBody.angularVelocity = randongRotSpeed;
    }


}
