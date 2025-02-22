using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliders : MonoBehaviour
{
    [SerializeField] private Collider attackCollider;
    [SerializeField] private Collider stunCollider;
    [SerializeField] private AudioSource swing;
    [SerializeField] private AudioClip[] _swingClips;
    [SerializeField] private AudioSource block;

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

    public void PlaySwing()
    {
        int randomIndex = Random.Range(0, _swingClips.Length);
        swing.clip = _swingClips[randomIndex];

        swing.Play();
    }

    public void PlayBlock()
    {
        block.Play();
    }
}
