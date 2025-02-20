using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    [SerializeField] private float maxHealth;
    public float MaxHealth { get { return maxHealth; } }

    [SerializeField] private float speed;
    public float Speed { get { return speed; } }

    [SerializeField] private float damage;
    public float Damage { get { return damage; } }


    private float curHealth;
    public float CurHealth { get { return curHealth; } }


    private Collider col;
    private Player player;

    private void Start()
    {
        col = GetComponent<Collider>();
        player = Player.Instance;
        curHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.CompareTag("PlayerAttack"))
        {
            curHealth -= player.Damage;
            Debug.Log("AAAAAAAAAAAAAAAAA");
            if (curHealth <= 0)
                Dead();
        }
    }

    private void Dead()
    {
        Destroy(gameObject);
        ArenaManager.Instance.DecEnemyCount();
    }
}
