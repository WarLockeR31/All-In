using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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


    private Player player;
    private Animator anim;
    private NavMeshAgent agent;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        player = Player.Instance;
        curHealth = maxHealth;
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && other.CompareTag("PlayerAttack"))
        {
            TakeDamage();
        }

        if (other.isTrigger && other.CompareTag("PlayerAttackStun"))
        {
            StartCoroutine(Stun());
            TakeDamage();
        }

        if (other.CompareTag("Player"))
        {
            player.TakeDamage(damage);
        }
    }

    private void TakeDamage()
    {
        if (curHealth <= 0)
            return;
        curHealth -= player.Damage;
        Debug.Log("AAAAAAAAAAAAAAAAA");

        StartCoroutine(FlashRed());

        if (curHealth <= 0)
            StartCoroutine(Dead());
        else
            StartCoroutine(DamageJump()); 
    }

    IEnumerator FlashRed()
    {
        float fadeDuration = 0.3f;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red; // Устанавливаем красный цвет
                                              // Плавное затухание
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / fadeDuration);
                spriteRenderer.color = Color.Lerp(Color.red, Color.white, progress);
                yield return null;
            }
        }
    }

    private IEnumerator DamageJump()
    {
        agent.isStopped = true;

        Vector3 startPosition = transform.position;
        float jumpDuration = 0.5f;
        float jumpHeight = 0.5f;
        float timer = 0;
        while (timer < jumpDuration)
        {
            // Параболическая траектория прыжка
            float progress = timer / jumpDuration;
            float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;

            transform.position = startPosition + Vector3.up * yOffset;
            timer += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
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
        anim.SetTrigger("Stunned");
        anim.SetBool("isStunned", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("isStunned", false);
    }
}
