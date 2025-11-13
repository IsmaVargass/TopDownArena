using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash settings")]
    public float dashDistance = 6f;
    public float dashDuration = 0.12f;
    public KeyCode dashKey = KeyCode.LeftShift;

    // --- CÓDIGO AÑADIDO PARA AUDIO ---
    [Header("Audio")]
    public AudioClip dashSound; // Clip de sonido para el dash
    private AudioSource audioSource; // Componente AudioSource

    // ----------------------------------

    [Header("Energy (dash bar)")]
    public float maxEnergy = 1f;        // 1 == barra llena
    public float energyRegenRate = 0.5f;    // por segundo
    public float regenDelay = 0.5f;          // segundos tras dash antes de regen

    CharacterController cc;
    public bool IsInvulnerable { get; private set; }

    // energy state
    private float energy;
    private float lastDashTime = -999f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        // --- CÓDIGO AÑADIDO PARA AUDIO ---
        // Obtener el componente AudioSource (debe estar en el mismo GameObject)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("[PlayerDash] No se encontró el componente AudioSource. El sonido del dash no funcionará.");
        }
        // ----------------------------------

        energy = maxEnergy;
        TryUpdateHUD();

        // IMPORTANTE: NO BLOQUEAR CURSOR AQUÍ. Lo hace MainMenu.cs
    }

    void Update()
    {
        // ==========================================================
        // SI EL JUEGO ESTÁ PAUSADO/CONGELADO, IGNORA EL INPUT.
        // ==========================================================
        if (Time.timeScale <= 0.01f)
        {
            return;
        }

        // regeneración tras delay
        if (Time.time - lastDashTime >= regenDelay && energy < maxEnergy)
        {
            float prev = energy;
            // Usa Time.deltaTime, que es 0 cuando el juego está pausado.
            energy += energyRegenRate * Time.deltaTime;
            energy = Mathf.Clamp(energy, 0f, maxEnergy);

            if (!Mathf.Approximately(prev, energy))
            {
                TryUpdateHUD();
            }
        }

        // Detección de input
        if (Input.GetKeyDown(dashKey))
        {
            TryDash();
        }
    }

    void TryDash()
    {
        if (energy < 1f || IsInvulnerable) return; // Chequeo de energía y si ya estamos en un dash

        Vector3 direction = Vector3.zero;

        float horiz = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");

        // Si no hay input de movimiento, usamos la dirección forward/backward
        if (horiz == 0f && vert == 0f)
        {
            direction = transform.forward;
        }
        else
        {
            // Creamos la dirección basada en el input
            direction = new Vector3(horiz, 0f, vert).normalized;
            // Si tu jugador rota con la cámara (típico FPS/TPS), usa:
            // direction = transform.TransformDirection(direction);
        }

        if (direction == Vector3.zero) return;

        // --- CÓDIGO AÑADIDO PARA AUDIO ---
        // Reproducir sonido del dash
        if (audioSource != null && dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }
        // ----------------------------------

        energy = 0f;
        lastDashTime = Time.time;
        TryUpdateHUD();

        StartCoroutine(PerformDash(direction));
    }

    IEnumerator PerformDash(Vector3 direction)
    {
        IsInvulnerable = true;
        float elapsed = 0f;

        // El movimiento del dash debe usar Time.deltaTime, que es afectado por Time.timeScale
        while (elapsed < dashDuration)
        {
            // cc.Move es CRÍTICO. Si Time.timeScale es 0, usamos Time.unscaledDeltaTime
            cc.Move(direction * dashDistance * (Time.deltaTime / dashDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // fin del dash: quitar invulnerabilidad
        IsInvulnerable = false;
    }

    // Propiedad pública para que otros scripts (DashBar) lean la fracción 0..1
    public float EnergyFraction
    {
        get { return Mathf.Clamp01(energy / maxEnergy); }
    }

    // método público para forzar la energía (si lo necesitas)
    public void SetEnergyFraction(float normalized)
    {
        float prev = energy;
        energy = Mathf.Clamp01(normalized) * maxEnergy;
        if (!Mathf.Approximately(prev, energy))
            TryUpdateHUD();
    }

    // Intenta actualizar el HUD si existe (safe). Usa API moderna para evitar warnings.
    void TryUpdateHUD()
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateDash(EnergyFraction);
            return;
        }

        // intentar encontrar un HUDManager activo en la escena (API moderna si está disponible)
#if UNITY_2023_1_OR_NEWER
            var hm = Object.FindAnyObjectByType<HUDManager>();
#else
        var hm = FindObjectOfType<HUDManager>();
#endif

        if (hm != null)
        {
            hm.UpdateDash(EnergyFraction);
        }
    }
}