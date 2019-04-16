using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string entityName;

    public AnimHelper AnimHelper { get; private set; }
    public EntityMovement Movement { get; private set; }

    private void Awake()
    {
        AnimHelper = GetComponent<AnimHelper>();
        Movement = GetComponent<EntityMovement>();
    }

    private void Start()
    {
        HealthDeathManager health = GetComponentInChildren<HealthDeathManager>();
        if (health != null)
            health.Initialize(this);
    }



}
