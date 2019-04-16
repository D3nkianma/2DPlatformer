using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public enum AttackDirection {
        Up,
        Down,
        Left,
        Right
    }


    public Transform leftAttackOrigin;
    public Transform rightAttackOrigin;
    public GameObject attackPrefab;





    public void LaunchAttack(AttackDirection direction, float xForce = 0f, float yForce = 0f)
    {
        Transform desiredLocation = GetAttackOrigin(direction);

        GameObject attack = Instantiate(attackPrefab, desiredLocation.position, desiredLocation.rotation) as GameObject;
        //attack.transform.SetParent(desiredLocation, false);
        HitBox hitbox = attack.GetComponent<HitBox>();
        hitbox.SetKnockback(CreateKnockbak(xForce, yForce));


    }

    public void LaunchAttack(PlayerController.FacingDirection facing, float xForce = 0f, float yForce = 0f)
    {
        LaunchAttack(ConvertFacingToAttackDirection(facing), xForce, yForce);
    }


    private Vector2 CreateKnockbak(float x, float y)
    {
        return new Vector2(x, y);
    }

    private AttackDirection ConvertFacingToAttackDirection(PlayerController.FacingDirection facing)
    {
        return facing == PlayerController.FacingDirection.Left ? AttackDirection.Left : AttackDirection.Right;
    }


    private Transform GetAttackOrigin(AttackDirection direction)
    {
        switch (direction)
        {
            case AttackDirection.Left:
                return leftAttackOrigin;
            case AttackDirection.Right:
                return rightAttackOrigin;

            default:
                return null;
        }
    }




}
