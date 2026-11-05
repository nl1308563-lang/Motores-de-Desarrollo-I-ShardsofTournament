
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RangedEnemy : MonoBehaviour
{
    [Header("Ajustes de Disparo")]
    public float shootingRange = 12f;
    public float fireRate = 1.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Referencias")]
    public string playerTag = "Player";

    private NavMeshAgent agent;
    private Transform playerTransform;
    private Animator animator;

    private float nextFireTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Buscar el Animator en el modelo hijo.
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("RangedEnemy: No se encontró un Animator.", this);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        agent.stoppingDistance = shootingRange - 1f;
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

        // Ir hacia el jugador.
        agent.SetDestination(playerTransform.position);

        // Actualizar Idle / Run.
        UpdateAnimationSpeed();

        // Si está en rango de tiro.
        if (distanceToPlayer <= shootingRange)
        {
            RotateTowardsPlayer();

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
        }
    }

    void UpdateAnimationSpeed()
    {
        if (animator == null || agent == null)
            return;

        animator.SetFloat("Speed", agent.velocity.magnitude);
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
                Time.deltaTime * 8f
            );
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Reproducir animación de disparo.
            if (animator != null)
            {
                animator.SetTrigger("Shoot");
            }

            // Apuntar al centro del jugador.
            Vector3 targetPosition = playerTransform.position + Vector3.up * 1f;

            Vector3 shootDirection = (
                targetPosition - firePoint.position
            ).normalized;

            Quaternion shootRotation = Quaternion.LookRotation(shootDirection);

            Instantiate(
                projectilePrefab,
                firePoint.position,
                shootRotation
            );
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
}
