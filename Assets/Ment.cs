using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ment : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float rotationSpeed = 10f; // Скорость поворота спрайта

    [Header("Dash")]
    [SerializeField] private float dashPower = 15f;     // Начальная скорость рывка
    [SerializeField] private float deceleration = 5f;    // Сила замедления
    [SerializeField] private float minSpeed = 0.1f;

    private EnemyStats stats;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private bool canAttack = true;
    private bool isDashing = false;

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

        if (isDashing) { return; }

        // Проверка атаки
        if ((Vector2.Distance(transform.position, player.position) <= attackRange && Mathf.Abs(transform.position.y - player.position.y) <= 3) && canAttack)
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
        agent.ResetPath();

        animator.SetTrigger("isAttacking");
        //Debug.Log("Attacking player!");


        //agent.isStopped = false;
        canAttack = true;
    }


    public void ResumuMovement()
    {
        agent.isStopped = false;
    }

    public void Dash()
    {
        StartCoroutine(PerformDash());
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;

        Vector3 dashDirection = -transform.forward;
        float currentSpeed = dashPower;

        while (currentSpeed > minSpeed)
        {
            transform.position += dashDirection * currentSpeed * Time.deltaTime;

            currentSpeed = Mathf.Lerp(currentSpeed, 0, deceleration * Time.deltaTime);

            yield return null; 
        }

        isDashing = false;
    }
}