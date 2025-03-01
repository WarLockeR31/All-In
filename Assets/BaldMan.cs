using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BaldMan : MonoBehaviour
{
    [SerializeField] private AudioSource _punchAudio;

    [Header("Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float rotationSpeed = 10f; // Скорость поворота спрайта

    private EnemyStats stats;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool canAttack = true;

    void Start()
    {
        stats = GetComponent<EnemyStats>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!stats) Debug.LogError("EnemyStats component missing!");
        if (!agent) Debug.LogError("NavMeshAgent component missing!");
        if (!animator) Debug.LogError("Animator component missing!");

        // Отключаем автоматический поворот агента
        agent.updateRotation = false;
        agent.angularSpeed = 0;

        if (stats) agent.speed = stats.Speed;

        player = Player.Instance?.transform;
        if (!player) Debug.LogError("Player not found!");

        StartCoroutine(StartMovement());
    }

    IEnumerator StartMovement()
    {
        yield return new WaitForSeconds(0.5f);
        if (player) agent.isStopped = false;
        animator.SetTrigger("isActive");
    }

    void Update()
    {
        // Поворачиваем спрайт к игроку (только по оси Y)
        Vector3 direction = transform.position - player.position;
        direction.y = 0; // Игнорируем разницу по высоте

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Проверка атаки
        if (Vector3.Distance(transform.position, player.position) <= attackRange && canAttack)
        {
            Attack();
        }




        if (!player || agent.isStopped) return;

        // Обновляем направление движения
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            agent.SetDestination(player.position);
    }

    void Attack()
    {
        canAttack = false;
        agent.isStopped = true;

        animator.SetTrigger("isAttacking");
        //Debug.Log("Attacking player!");

        
        agent.isStopped = false;
        canAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (!player.GetComponent<Player>().IsInvincible)
            {
                _punchAudio.Play();
            }
        }
    }
}