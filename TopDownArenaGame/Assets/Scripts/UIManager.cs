using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject gameOverPanel;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI bestComboText;

    void Awake()
    {
        Instance = this;
    }

    // Helper: intenta buscar un TextMeshProUGUI por nombre dentro del panel
    TextMeshProUGUI FindTMPInPanel(string childName)
    {
        if (gameOverPanel == null) return null;
        Transform t = gameOverPanel.transform.Find(childName);
        if (t != null)
        {
            var tmp = t.GetComponent<TextMeshProUGUI>();
            if (tmp != null) return tmp;
            // a veces el Text está en un hijo más profundo
            tmp = t.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) return tmp;
        }

        // fallback global (menos seguro): busca por nombre en toda la escena
        // Usamos la API moderna cuando esté disponible para evitar warnings
#if UNITY_2023_1_OR_NEWER
            var all = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
#else
        var all = FindObjectsOfType<TextMeshProUGUI>();
#endif

        foreach (var x in all)
        {
            if (x.gameObject.name == childName) return x;
        }
        return null;
    }

    void EnsureTextAssignedAndVisible(ref TextMeshProUGUI field, string childName)
    {
        if (field == null)
        {
            field = FindTMPInPanel(childName);
            if (field != null)
                Debug.Log($"[UIManager] Auto-asignado '{childName}' a {field.gameObject.name}");
        }

        if (field != null)
        {
            // Asegurarnos de que el GameObject del texto está activo y visible
            if (!field.gameObject.activeInHierarchy)
                field.gameObject.SetActive(true);

            // Forzamos un color/alpha legible si por accidente estaba transparente
            try
            {
                var col = field.color;
                col.a = 1f;
                field.color = col;
            }
            catch { }
        }
        else
        {
            Debug.Log($"[UIManager] WARNING: no pude encontrar '{childName}' en GameOverPanel.");
        }
    }

    public void ShowGameOver(int kills, float time, int bestCombo)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // Intentamos auto-asignar si algún campo está vacío
        EnsureTextAssignedAndVisible(ref killsText, "killsText");
        EnsureTextAssignedAndVisible(ref timeText, "timeText");
        EnsureTextAssignedAndVisible(ref bestComboText, "bestComboText");

        Debug.Log($"[UIManager] ShowGameOver called: kills={kills}, time={time}, bestCombo={bestCombo}");

        if (killsText != null)
            killsText.text = "Kills: " + kills;
        else
            Debug.Log("[UIManager] killsText sigue sin asignarse.");

        if (timeText != null)
            timeText.text = "Time: " + Mathf.RoundToInt(time) + "s";
        else
            Debug.Log("[UIManager] timeText sigue sin asignarse.");

        if (bestComboText != null)
            bestComboText.text = "Best Racha: " + bestCombo;
        else
            Debug.Log("[UIManager] bestComboText sigue sin asignarse.");
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        if (Application.CanStreamedLevelBeLoaded("SCENEBUENA"))
        {
            SceneManager.LoadScene("SCENEBUENA");
            return;
        }

        if (SceneManager.sceneCountInBuildSettings > 0)
        {
            SceneManager.LoadScene(0);
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
