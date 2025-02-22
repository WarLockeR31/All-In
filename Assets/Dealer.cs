using System.Collections;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class Dealer : MonoBehaviour
{
    [SerializeField] private AudioSource _throwCardAudio;


    [Header("Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float rotationSpeed = 10f; // Скорость поворота спрайта
    [SerializeField] private GameObject projectilePrefab; // Скорость поворота спрайта
    [SerializeField] private Transform projectileStartPos; // Скорость поворота спрайта

    private EnemyStats stats;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isReseted;
    public LayerMask wallLayer; // Указываем в инспекторе, какие слои считаем стенами

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
        if (isReseted)
        {
            isReseted = false;
            agent.isStopped = false;
        }
        // Поворачиваем спрайт к игроку (только по оси Y)
        Vector3 direction = transform.position - player.position;
        direction.y = 0; // Игнорируем разницу по высоте

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // Проверка атаки
        if (TryAttack())
        {
            Attack();
            return;
        }




        if (!player || agent.isStopped) return;

        // Обновляем направление движения
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            agent.SetDestination(player.position);
    }

    bool TryAttack()
    {
        Vector3 playerPos = player.position;
        float distanceToPlayer = Vector3.Distance(projectileStartPos.position, playerPos);
        if (distanceToPlayer > attackRange)
            return false;

        Vector3 dir = (playerPos - projectileStartPos.position).normalized;

        bool isHitted = Physics.Raycast(projectileStartPos.position + dir * 2, dir, out RaycastHit Hit, distanceToPlayer * 2, wallLayer);
        Debug.DrawRay(projectileStartPos.position + dir, dir * distanceToPlayer * 2, UnityEngine.Color.green);

        if (isHitted)
        {
            return Hit.collider.CompareTag("Player");
        }
        
        
        return false;
    }

    void Attack()
    {
        agent.isStopped = true;
        agent.ResetPath();

        animator.SetTrigger("isAttacking");


        
    }

    public void SpawnProjetile()
    {
        _throwCardAudio.Play();

        Vector3 playerPos = player.position;
        Vector3 dirToPlayer = (playerPos - projectileStartPos.position).normalized;
        GameObject cardObj = Instantiate(projectilePrefab, projectileStartPos.position + dirToPlayer, Quaternion.Euler(new Vector3(70, 0, 0)));
        CardProjectile card = cardObj.GetComponent<CardProjectile>();
        card.SetDamage(stats.Damage);
    }

    public void ResumuMovement()
    {
        isReseted = true;
    }
}