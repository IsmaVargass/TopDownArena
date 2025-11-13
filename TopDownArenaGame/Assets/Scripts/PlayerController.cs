using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Importante: Necesario para usar la clase Image

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Vida")]
    public float vida = 100f;
    public float vidaMaxima = 100f;

    // --- NUEVO PARA EFECTO DE DAÑO ---
    [Header("Efecto de Oscurecimiento (Viñeta)")]
    [Tooltip("Panel de UI negro que cubre la pantalla.")]
    public Image damageOverlay;

    [Range(0f, 1f)]
    [Tooltip("Opacidad máxima del panel cuando la vida es cero (ej. 0.7 = 70% oscuro).")]
    public float maxOpacity = 0.7f;
    // ---------------------------------

    [Header("Movimiento")]
    public float velocidad = 5f;
    private CharacterController controller;

    [Header("Rotación y Mira")]
    [Tooltip("La capa de terreno o suelo que usaremos para detectar la posición del ratón.")]
    public LayerMask floorMask; // Ya no se usa.

    [Tooltip("Velocidad de giro hacia el ratón.")]
    public float rotationSpeed = 15f; // Ya no se usa.

    // Referencia opcional al dash
    private PlayerDash dashComponent;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        vida = vidaMaxima;
        dashComponent = GetComponent<PlayerDash>();

        // Asegurarse de que el cursor esté libre y visible
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Inicializa el efecto de oscurecimiento (debe ser transparente al inicio)
        UpdateDamageOverlay();
    }

    void Update()
    {
        // 1. Protección contra la pausa (Time.timeScale = 0)
        if (Time.timeScale <= 0.01f) return;

        // Aquí iría tu lógica de movimiento si estuviera en este script.
    }

    // --- LÓGICA DE OSCURECIMIENTO ---
    /// <summary>
    /// Calcula y aplica la opacidad al panel de oscurecimiento
    /// basándose en la vida restante del jugador.
    /// </summary>
    void UpdateDamageOverlay()
    {
        if (damageOverlay == null)
        {
            // Debug.LogWarning("Damage Overlay Panel no asignado en PlayerController.");
            return;
        }

        // 1. Calcula la fracción de vida restante (0.0 a 1.0)
        float vidaFraccion = vida / vidaMaxima;

        // 2. Calcula la fracción de DAÑO recibido (1.0 - vidaFraccion)
        // 0% de vida (vidaFraccion=0) -> 1.0 (máximo daño)
        float danoFraccion = 1f - vidaFraccion;

        // 3. Aplica el límite máximo de opacidad deseado
        float targetOpacity = danoFraccion * maxOpacity;

        // 4. Aplica el alpha al color del panel
        Color c = damageOverlay.color;
        c.a = Mathf.Clamp01(targetOpacity);
        damageOverlay.color = c;
    }


    // --- FUNCIONES DE VIDA MODIFICADAS ---
    public void RecibirDaño(int daño)
    {
        if (dashComponent != null && dashComponent.IsInvulnerable)
        {
            Debug.Log("[PlayerController] Daño ignorado: jugador invulnerable por dash.");
            return;
        }
        vida -= daño;

        // ¡IMPORTANTE! Llamar a la actualización visual inmediatamente después de recibir daño
        UpdateDamageOverlay();

        if (vida <= 0f)
        {
            vida = 0f;
            MuerteJugador();
        }
    }

    public void Curar(float cantidad)
    {
        vida += cantidad;
        vida = Mathf.Clamp(vida, 0f, vidaMaxima);

        // ¡IMPORTANTE! Llamar a la actualización visual después de curar
        UpdateDamageOverlay();
    }

    void MuerteJugador()
    {
        Debug.Log("💀 Jugador muerto");
#if UNITY_2023_1_OR_NEWER
        var spawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
#else
        var spawners = FindObjectsOfType<EnemySpawner>();
#endif
        foreach (var s in spawners)
        {
            if (s != null) s.StopSpawning();
        }
        if (GameManager.Instance != null)
            GameManager.Instance.EndGame();
        else
            Debug.LogWarning("GameManager.Instance es null.");
        Destroy(gameObject);
    }
}