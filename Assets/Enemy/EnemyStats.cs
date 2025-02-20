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
    private Animator anim;

    private void Start()
    {
        col = GetComponent<Collider>();
        player = Player.Instance;
        curHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.CompareTag("PlayerAttack"))
        {
            curHealth -= player.Damage;
            Debug.Log("AAAAAAAAAAAAAAAAA");
            if (curHealth <= 0)
                StartCoroutine(Dead());
        }

        if (other.isTrigger && other.CompareTag("PlayerAttackStun"))
        {
            if (curHealth <= 0)
                return;
            curHealth -= player.Damage;
            Debug.Log("AAAAAAAAAAAAAAAAA");
            StartCoroutine(Stun());
            if (curHealth <= 0)
                StartCoroutine(Dead());
        }
    }

    IEnumerator Dead()
    {
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(0.5f);

        ArenaManager.Instance.DecEnemyCount();
        Destroy(gameObject);
    }

    public void SetStats(float hp, float dmg, float spd)
    {
        maxHealth = hp;
        curHealth = maxHealth;
        damage = dmg;
        speed = spd;
    }

    IEnumerator Stun()
    {
        anim.SetBool("isStunned", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("isStunned", false);
    }
}
