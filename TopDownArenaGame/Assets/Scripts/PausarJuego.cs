// PausarJuego.cs - Adjuntar al GameObject 'SystemPause'
using UnityEngine;
using UnityEngine.EventSystems;

public class PausarJuego : MonoBehaviour
{
    // Singleton para acceso global
    public static PausarJuego Instance { get; private set; }

    [Header("UI y Selección")]
    [Tooltip("Arrastra aquí el GameObject del menú de pausa (MenuPause)")]
    public GameObject menuPausa;
    [Tooltip("El primer botón a seleccionar al pausar (para navegación con teclado)")]
    public GameObject firstSelectedOnPause;

    [Header("Control")]
    [Tooltip("Controla si el usuario puede pausar (false en menú principal, true en juego)")]
    public bool puedePausar = false;
    public bool juegoPausado { get; private set; } = false;

    // Estados previos a guardar para restaurar correctamente
    private CursorLockMode prevLockMode;
    private bool prevCursorVisible;
    private float prevFixedDeltaTime;
    private bool estadoGuardado = false;

    [Header("Opcional")]
    public bool dontDestroyOnLoadInstance = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (dontDestroyOnLoadInstance)
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning($"[PausarJuego] Instancia duplicada detectada en {gameObject.name} — desactivando componente.");
            this.enabled = false;
        }
    }

    private void Start()
    {
        // El menú de pausa siempre debe empezar oculto
        if (menuPausa != null) menuPausa.SetActive(false);
        juegoPausado = false;

        // Se desactiva por defecto. MainMenu.cs lo activa cuando se pulsa Jugar.
        puedePausar = false;
    }

    private void Update()
    {
        // El chequeo 'puedePausar' asegura que el ESC no funcione en el menú principal (lobby)
        if (!puedePausar) return;

        // DETECCIÓN DE LA TECLA ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (juegoPausado) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (juegoPausado) return;

        // Guardar el estado del juego antes de pausar
        if (!estadoGuardado)
        {
            prevLockMode = Cursor.lockState;
            prevCursorVisible = Cursor.visible;
            prevFixedDeltaTime = Time.fixedDeltaTime;
            estadoGuardado = true;
        }

        if (menuPausa != null) menuPausa.SetActive(true);

        // Ajustes para la pausa
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f; // Evitar que la física se ejecute

        juegoPausado = true;
        Debug.Log("[PausarJuego] Pausado");

        // Seleccionar el primer botón para navegación
        if (firstSelectedOnPause != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedOnPause);
        }
    }

    public void Resume()
    {
        if (!juegoPausado) return;

        if (menuPausa != null) menuPausa.SetActive(false);

        // Restaurar estado del juego
        Time.timeScale = 1f;
        if (estadoGuardado)
        {
            Time.fixedDeltaTime = prevFixedDeltaTime;
            Cursor.lockState = prevLockMode;
            Cursor.visible = prevCursorVisible;
            estadoGuardado = false;
        }
        else // Fallback si el estado no fue guardado por alguna razón
        {
            Time.fixedDeltaTime = 0.02f; // Valor por defecto
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        juegoPausado = false;
        Debug.Log("[PausarJuego] Reanudado");
    }
}