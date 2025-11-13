using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Stats")]
    public int kills = 0;
    public int bestCombo = 0;

    [Header("Combo")]
    public int currentCombo = 0;
    public float comboWindow = 2.0f; // segundos para encadenar kills
    private float lastKillTime = -999f;

    private float startTime;
    private bool isGameOver = false;

    void Awake()
    {
        // Singleton básico
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ResetStats();
    }

    void Update()
    {
        if (isGameOver) return;

        // Actualiza el HUD con el tiempo transcurrido
        float elapsed = Time.time - startTime;
        if (HUDManager.Instance != null)
            HUDManager.Instance.UpdateTime(elapsed);
    }

    public void ResetStats()
    {
        kills = 0;
        currentCombo = 0;
        bestCombo = PlayerPrefs.GetInt("BestCombo", 0);
        startTime = Time.time;
        lastKillTime = -999f;
        isGameOver = false;

        // Actualizar HUD inicial
        if (HUDManager.Instance != null)
            HUDManager.Instance.UpdateAll(kills, 0f, currentCombo);
    }

    // Llamar desde cada enemigo cuando muera
    public void AddKill()
    {
        if (isGameOver) return;

        float now = Time.time;

        // Combo por kills rápidas
        if (now - lastKillTime <= comboWindow)
            currentCombo++;
        else
            currentCombo = 1;

        lastKillTime = now;
        kills++;
        Debug.Log($"[GameManager] AddKill called — kills now: {kills}, currentCombo: {currentCombo}, bestCombo: {bestCombo}");

        if (currentCombo > bestCombo)
        {
            bestCombo = currentCombo;
            PlayerPrefs.SetInt("BestCombo", bestCombo);
        }

        // Actualizar HUD
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateKills(kills);
            HUDManager.Instance.UpdateCombo(currentCombo);
        }
    }

    public void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        float elapsed = Time.time - startTime;

        // Detener spawners por seguridad — usando API moderna cuando esté disponible
#if UNITY_2023_1_OR_NEWER
        var spawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
#else
        var spawners = FindObjectsOfType<EnemySpawner>();
#endif

        foreach (var s in spawners)
        {
            if (s != null) s.StopSpawning();
        }

        // Mostrar Game Over en UIManager (con llaves para evitar nullrefs)
        if (UIManager.Instance != null)
        {
            Debug.Log($"[GameManager] EndGame — sending to UI: kills={kills}, elapsed={elapsed}, bestCombo={bestCombo}");
            UIManager.Instance.ShowGameOver(kills, elapsed, bestCombo);
        }
        else
        {
            Debug.LogWarning("UIManager.Instance es null — asegura UIManager en la escena.");
        }

        // Ocultar HUD
        if (HUDManager.Instance != null)
            HUDManager.Instance.ShowHUD(false);

        // Pausar el juego
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
