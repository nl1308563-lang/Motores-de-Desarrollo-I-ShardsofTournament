
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemy : MonoBehaviour
{
    [Header("Ajustes de Combate")]
    public float attackRange = 2f;
    public float attackDamage = 15f;
    public float attackRate = 1.2f;

    [Header("Referencias")]
    public string playerTag = "Player";

    private NavMeshAgent agent;
    private Transform playerTransform;
    private PlayerShield playerShield;
    private PlayerHealth playerHealth;
    private Animator animator;

    private float nextAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Buscamos el Animator del modelo hijo.
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("MeleeEnemy: No se encontró un Animator en sus hijos.", this);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;

            playerShield = playerObj.GetComponent<PlayerShield>();
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        agent.stoppingDistance = attackRange - 0.2f;
    }

    void Update()
    {
        if (playerTransform == null)
        {
            UpdateAnimationSpeed();
            return;
        }

        float distanceToPlayer = Vector3.Distance(
            transform.position,
            playerTransform.position
        );

        agent.SetDestination(playerTransform.position);

        // Actualizamos la animación según el movimiento real.
        UpdateAnimationSpeed();

        if (distanceToPlayer <= attackRange)
        {
            RotateTowardsPlayer();

            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackRate;
                Attack();
            }
        }
    }

    void UpdateAnimationSpeed()
    {
        if (animator == null || agent == null)
            return;

        float speed = agent.velocity.magnitude;

        animator.SetFloat("Speed", speed);
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = (
            playerTransform.position - transform.position
        ).normalized;

        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * 10f
            );
        }
    }

    void Attack()
    {
        // Validar si el jugador no está muerto antes de atacar.
        bool isDead = playerHealth != null && playerHealth.isDead;

        if (!isDead)
        {
            // Atacamos primero al escudo si existe.
            if (playerShield != null)
            {
                playerShield.TakeDamage(attackDamage);

                Debug.Log(
                    $"¡MeleeEnemy atacó el escudo del jugador con {attackDamage} de daño!"
                );
            }
            else if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);

                Debug.Log(
                    $"¡MeleeEnemy atacó la vida directa del jugador con {attackDamage} de daño!"
                );
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
