using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 1; // daño que hace al tocar al jugador

    // --- AUDIO Y EFECTOS ---
    [Header("Efectos")]
    public AudioClip deathSound;
    public GameObject deathEffectPrefab; // **NUEVO:** Prefab del sistema de partículas
    private AudioSource audioSource;
    // ------------------------

    void Start()
    {
        // Obtener o añadir el componente AudioSource al inicio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Creamos un AudioSource si no existe
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // accedemos al PlayerController del jugador
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.RecibirDaño(damage); // restamos vida en lugar de destruir
            }
        }
        else if (other.CompareTag("Bullet"))
        {
            // destruimos la bala
            Destroy(other.gameObject);

            // Llamamos a la función de manejo de muerte
            HandleEnemyDeath();
        }
    }

    /// <summary>
    /// Gestiona la notificación, el sonido, la partícula y la destrucción del enemigo.
    /// </summary>
    void HandleEnemyDeath()
    {
        // 1. Notificación al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKill();
            Debug.Log($"[EnemyAttack] enemy {gameObject.name} killed by bullet - AddKill called.");
        }
        else
        {
            Debug.LogWarning("[EnemyAttack] GameManager.Instance es null al intentar notificar kill.");
        }

        // 2. Instanciar el efecto de partículas (antes de la destrucción visual)
        if (deathEffectPrefab != null)
        {
            // Creamos el efecto en la posición del enemigo.
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        float destructionDelay = 0f;

        // 3. Reproducir sonido y destruir el objeto
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
            destructionDelay = deathSound.length;

            // Ocultamos el renderizador, el collider y deshabilitamos el script
            var rend = GetComponent<Renderer>();
            if (rend != null) rend.enabled = false;
            var coll = GetComponent<Collider>();
            if (coll != null) coll.enabled = false;
            enabled = false;
        }

        // 4. Destrucción final (inmediata si no hay sonido, o con retardo si lo hay)
        Destroy(gameObject, destructionDelay);
    }
}