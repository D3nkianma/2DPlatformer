using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    [Header("Basic Variables")]
    public float damage;
    [Range(0.1f, 5f)]
    public float lifetime = 1f;
    public LayerMask mask;

    [Header("VFX")]
    public GameObject impactEffect;

    private Vector2 knockBackVector;

    private List<GameObject> targets = new List<GameObject>();

    private AnimHelper animHelper;

    private void Awake()
    {
        animHelper = GetComponent<AnimHelper>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
        animHelper.StartAnimTrigger("Attack1");
    }

    public void SetKnockback(Vector2 knockback)
    {
        knockBackVector = knockback;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LayerTools.IsLayerInMask(mask, other.gameObject.layer) == false)
            return;

        if (CheckHitTargets(other.gameObject) == false)
            return;

        CreateHitEffect(other.transform.position);
        DealDamage(other.gameObject);
        ApplyForcedMovement(other.gameObject);
    }

    private void DealDamage(GameObject other)
    {
        HealthDeathManager otherHealth = other.GetComponentInChildren<HealthDeathManager>();

        if (otherHealth == null)
            return;

        otherHealth.AlterHealth(damage);
    }

    private void ApplyForcedMovement(GameObject other)
    {
        EntityMovement movement = other.GetComponentInChildren<EntityMovement>();
        if (movement == null)
            return;

        movement.ForceMovement(knockBackVector);
    }

    private bool CheckHitTargets(GameObject target)
    {
        if (target == null)
            return false;

        int count = targets.Count;
        for (int i = 0; i < count; i++)
        {
            if (targets[i] == target)
                return false;
        }

        if (targets.Contains(target) == false)
            targets.Add(target);

        return true;
    }

    private void CreateHitEffect(Vector2 location)
    {
        if (impactEffect == null)
            return;

        Vector2 loc = new Vector2(location.x + Random.Range(-0.5f, 0.5f), location.y + Random.Range(-0.5f, 0.5f));

        GameObject impact = Instantiate(impactEffect, loc, Quaternion.identity) as GameObject;
        Destroy(impact, 2f);

    }


}
