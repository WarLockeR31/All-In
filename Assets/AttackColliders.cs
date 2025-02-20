using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliders : MonoBehaviour
{
    [SerializeField] private Collider attackCollider;
    [SerializeField] private Collider stunCollider;

    public void EnableAttackCollider()
    {
        attackCollider.enabled = true;
    }
    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }

    public void EnableStunCollider()
    {
        stunCollider.enabled = true;
    }
    public void DisableStunCollider()
    {
        stunCollider.enabled = false;
    }
}
