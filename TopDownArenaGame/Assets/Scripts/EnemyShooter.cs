using UnityEngine;
using System.Collections;

public class EnemyShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform muzzle;
    public float fireRate = 1.5f;

    // --- NUEVO PARA AUDIO ---
    public AudioClip shootSound;
    private AudioSource audioSource;
    // ------------------------

    Transform player;
    void Start()
    {
        // Inicialización de Player
        player = GameObject.FindWithTag("Player")?.transform;

        // --- NUEVO PARA AUDIO ---
        // Obtener o añadir el componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // Asegurarse de que no suene al iniciar el juego
        audioSource.playOnAwake = false;
        // ------------------------

        StartCoroutine(FireLoop());
    }

    IEnumerator FireLoop()
    {
        while (true)
        {
            if (player != null)
            {
                // Lógica de movimiento y creación de bala
                Vector3 dir = (player.position - muzzle.position).normalized;
                GameObject b = Instantiate(bulletPrefab, muzzle.position, Quaternion.LookRotation(dir));
                b.GetComponent<Rigidbody>().linearVelocity = dir * 12f; // ajusta velocidad

                // --- NUEVO PARA AUDIO ---
                // Reproducir el sonido
                if (shootSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(shootSound);
                }
                // ------------------------
            }
            yield return new WaitForSeconds(fireRate);
        }
    }
}