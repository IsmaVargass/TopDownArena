// MainMenu.cs - Adjuntar a un GameObject que ESTÉ ACTIVO al inicio de la escena.
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI del Menú")]
    [Tooltip("Panel que contiene el MainMenu (Jugar, Opciones, Salir)")]
    public GameObject mainMenuPanel; // Panel de botones (Play, Options, Quit)
    [Tooltip("Panel de Opciones")]
    public GameObject optionsPanel;

    // Ya no necesitamos la referencia a menuCanvas si este script está en el Canvas, 
    // pero la mantengo para mayor compatibilidad si lo adjuntas a otro sitio.
    // [Tooltip("El GameObject Canvas raíz de la UI")]
    // public GameObject menuCanvas; 

    [Header("Objetos de Gameplay a Desactivar/Activar")]
    [Tooltip("Objeto raíz que contiene el player, enemigos, etc. (Opcional)")]
    public GameObject gameplayRoot;
    [Tooltip("Objeto del Jugador")]
    public GameObject playerObject;
    [Tooltip("Objeto Generador de Enemigos")]
    public GameObject enemySpawner;
    [Tooltip("Objeto HUD/Interfaz de juego")]
    public GameObject hudRoot;

    bool started = false;

    // Ejecutado primero: Asegura que el cursor esté libre y visible
    void Awake()
    {
        // CRÍTICO: Forzar desbloqueo del cursor para que la UI pueda ser clicada
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Start()
    {
        Debug.Log("[MainMenu] Inicializando en modo LOBBY (Congelado).");

        // 1. Mostrar SÓLO el Panel de Menú y ocultar Opciones
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // 2. Desactivar todo el Gameplay
        // NOTA: Si este script está en el Canvas, NO DESACTIVAR EL CANVAS RAIZ.
        if (gameplayRoot != null)
        {
            gameplayRoot.SetActive(false);
        }
        else // Si no hay Root, desactivamos los componentes listados
        {
            if (playerObject != null) playerObject.SetActive(false);
            if (enemySpawner != null) enemySpawner.SetActive(false);
            if (hudRoot != null) hudRoot.SetActive(false);
        }

        // 3. CONGELAR EL JUEGO (Lobby).
        Time.timeScale = 0f;

        // Desactivar Pausa (para evitar que ESC pause el juego en el menú principal)
        if (PausarJuego.Instance != null)
        {
            PausarJuego.Instance.puedePausar = false;
        }

        Debug.Log($"[MainMenu] Time.timeScale ha sido establecido a: {Time.timeScale}");
    }

    // Método llamado por el botón "Jugar"
    public void PlayGame()
    {
        if (started) return;
        started = true;

        Debug.Log("[MainMenu] PlayGame invocado - Iniciando partida.");

        // 1. Ocultar los paneles del menú (pero no el Canvas)
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // 2. Reanudar el tiempo
        Time.timeScale = 1f;

        // 3. Activar el Gameplay
        if (gameplayRoot != null) gameplayRoot.SetActive(true);
        if (playerObject != null) playerObject.SetActive(true);
        if (enemySpawner != null) enemySpawner.SetActive(true);
        if (hudRoot != null) hudRoot.SetActive(true);

        // 4. Bloquear el cursor (para el control de la cámara)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 5. Activar el control de Pausa
        if (PausarJuego.Instance != null)
        {
            PausarJuego.Instance.puedePausar = true;
            Debug.Log("[MainMenu] Control de Pausa activado (tecla ESC).");
        }
        else
        {
            Debug.LogError("[MainMenu] No se encontró PausarJuego.Instance. Asegúrate de que PausarJuego.cs esté en la escena.");
        }
    }

    public void OpenOptionsPanel()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void OpenMainMenuPanel()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}