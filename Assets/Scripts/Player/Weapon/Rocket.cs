using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rocket : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 35f;
    public float maxLifetime = 6f;

    [Header("Efectos y Área de Explosión")]
    public GameObject explosionPrefab;
    public float explosionRadius = 6f;
    public float explosionForce = 800f;
    public float damage = 100f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, maxLifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

        foreach (Collider hit in hitColliders)
        {
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                hitRb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            EnemyHealth health = hit.GetComponent<EnemyHealth>();
            if (health == null) health = hit.GetComponentInParent<EnemyHealth>();

            if (health != null && !damagedEnemies.Contains(health))
            {
                float finalDamage = damage;
                if (ShopManager.Instance != null)
                {
                    finalDamage *= ShopManager.Instance.GetPlayerDamageMultiplier();
                }

                health.TakeDamage(finalDamage);
                damagedEnemies.Add(health);
            }
        }

        Destroy(gameObject);
    }
}