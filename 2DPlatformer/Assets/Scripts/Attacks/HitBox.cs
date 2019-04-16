using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float damage;
    public float lifetime = 1f;
    public LayerMask mask;

    private Vector2 knockBackVector;

    private void Start()
    {
        if(lifetime > 0f)
        {
            Destroy(gameObject, lifetime);
        }
    }

    public void SetKnockback(Vector2 knockback)
    {
        knockBackVector = knockback;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((mask & 1 << other.gameObject.layer) == 1 << other.gameObject.layer == false)
            return;

        HealthDeathManager otherHealth = other.gameObject.GetComponentInChildren<HealthDeathManager>();

        if (otherHealth == null)
            return;

        otherHealth.AlterHealth(damage);

        EntityMovement movement = otherHealth.Owner.Movement;
        if(movement != null)
        {
            movement.ForceMovement(knockBackVector);
        }
        
    }


}
