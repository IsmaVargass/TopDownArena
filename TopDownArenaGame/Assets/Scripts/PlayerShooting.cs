using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    public GameObject BulletPrefab;
    public Transform BulletSpawn;

    public float TimeBetweenShots = 0.3333f;
    private float m_timeStamp = 0f;

    // --- NUEVO PARA AUDIO ---
    public AudioClip shootSound;
    private AudioSource audioSource;
    // ------------------------

    void Start()
    {
        // Obtener o añadir el componente AudioSource al inicio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // Asegurarse de que el audio no se reproduzca al iniciar la escena
        audioSource.playOnAwake = false;
    }

    void FixedUpdate()
    {
        if ((Time.time >= m_timeStamp) && (Input.GetKey(KeyCode.Mouse0)))
        {
            Fire();
            m_timeStamp = Time.time + TimeBetweenShots;
        }
    }

    void Fire()
    {
        var bullet = (GameObject)Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward * 50;

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);

        // --- NUEVO PARA AUDIO ---
        // Reproducir el sonido
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
        // ------------------------
    }
}