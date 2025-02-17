using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]private float money = 100f;

    [SerializeField] private float health = 100f;

    private float playerLevel = 0;

    private float maxLevel = 5f;

    private float damage = 10f;

    public Animator animator;

    

    public void LoseMoney(float amount)
    {
        money-=amount;
    }

    public void LoseHealth(float amount) {
        if (!animator.GetBool("isBlocking"))
            health-=amount;
    }

    public void GainMoney(float amount)
    {
        money+=amount;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
