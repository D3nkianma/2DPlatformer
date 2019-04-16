using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDeathManager : MonoBehaviour
{

    public Entity Owner { get; private set; }

    public float maxHealth;
    public float Ratio { get { return currentHealth / maxHealth; } }


    private float currentHealth;



    public void Initialize(Entity owner)
    {
        Owner = owner;
        currentHealth = maxHealth;
    }


    public void AlterHealth(float value)
    {
        currentHealth += value;

        if(currentHealth < 0)
            currentHealth = 0;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if(value < 0f)
        {
            Owner.AnimHelper.StartAnimTrigger("Flinch");
        }

        if(currentHealth <= 0f)
        {
            EntityMovement movement = Owner.Movement;
            if (movement != null)
            {
                movement.SpinCrazy();
            }
        }

    }

}
