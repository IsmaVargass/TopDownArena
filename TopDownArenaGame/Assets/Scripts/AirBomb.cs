using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirBomb : MonoBehaviour
{
    [Header("Ajustes de Daño y Área")]
    [Tooltip("Cantidad de vida que restará la explosión al jugador.")]
    public int damageAmount = 1;

    [Tooltip("Radio del área de efecto de la explosión.")]
    public float explosionRadius = 2.0f;

    [Header("Tiempos")]
    [Tooltip("Tiempo antes de que la bomba explote después de caer.")]
    public float timeToExplode = 1f;

    [Header("Efectos")]
    [Tooltip("Prefab del sistema de partículas de la explosión (ej. humo, fuego).")]
    public GameObject explosionEffectPrefab;

    [Tooltip("Sonido de la explosión.")]
    public AudioClip explosionSound;
    private AudioSource audioSource;

    private bool hasExploded = false;
    private Collider bombCollider;
    private Rigidbody rb;

    void Start()
    {
        // 1. Configuración de Componentes
        rb = GetComponent<Rigidbody>();
        bombCollider = GetComponent<Collider>();

        // Configuración del AudioSource (similar al enemigo anterior)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // Sonido 3D
        }

        // 2. Iniciar la cuenta atrás de la explosión
        StartCoroutine(ExplodeAfterDelay());
    }

    /// <summary>
    /// Corrutina que espera a que pase el tiempo y luego llama a la explosión.
    /// </summary>
    IEnumerator ExplodeAfterDelay()
    {
        // Puedes añadir un efecto visual aquí (ej. la bomba empieza a parpadear)
        // ...

        yield return new WaitForSeconds(timeToExplode);

        // Aseguramos que solo explote una vez
        if (!hasExploded)
        {
            Explode();
        }
    }

    /// <summary>
    /// Lógica principal de la explosión: daño, efectos y destrucción.
    /// </summary>
    void Explode()
    {
        hasExploded = true;
        Debug.Log("💣 Bomba explotando!");

        // 1. Daño en Área (usando Physics.OverlapSphere)
        // Busca todos los colliders dentro del radio de explosión
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Verificamos si el objeto impactado es el jugador
            if (hitCollider.CompareTag("Player"))
            {
                // Intentamos obtener el PlayerController
                PlayerController player = hitCollider.GetComponent<PlayerController>();

                if (player != null)
                {
                    // Aplicamos el daño al jugador
                    player.RecibirDaño(damageAmount);
                    Debug.Log($"[AirBomb] Jugador golpeado por explosión. Daño: {damageAmount}");
                }
            }
        }

        // 2. Efecto Visual (Partículas)
        if (explosionEffectPrefab != null)
        {
            // Instancia el prefab de la explosión en la posición de la bomba
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            // El efecto debe auto-destruirse (Ver sección 3.A)
        }

        // 3. Sonido
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // 4. Limpieza (Destrucción visual y física)

        // Ocultar la malla y deshabilitar el collider/rigidbody inmediatamente
        var rend = GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;

        if (bombCollider != null) bombCollider.enabled = false;
        if (rb != null) rb.isKinematic = true;

        // Si hay sonido, espera a que termine. Si no hay sonido, destruye inmediatamente.
        float destructionDelay = (explosionSound != null) ? explosionSound.length : 0f;
        Destroy(gameObject, destructionDelay);
    }

    // Muestra el radio de la explosión en la escena para debug (solo visible en el editor)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}