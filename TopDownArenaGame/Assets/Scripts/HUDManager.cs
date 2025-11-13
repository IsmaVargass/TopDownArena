// HUDManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("HUD Texts (in-game)")]
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI healthText; // opcional: porcentaje o valor de vida
    public GameObject hudRoot; // contenedor principal del HUD

    [Header("Health UI")]
    public Slider healthSlider; // slider 0..1 para vida
    public GameObject healthContainer; // padre del health bar (puede estar en Canvas principal)

    [Header("Dash UI")]
    public Slider dashSlider; // slider que representa la barra de dash (valores 0..1)
    public GameObject dashContainer; // contenedor del slider (para ocultar si quieres)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (hudRoot != null) hudRoot.SetActive(true);

        // Inicializa textos/valores
        UpdateAll(0, 0f, 0);
        UpdateHealth(1f); // vida al 100% por defecto

        // Inicializa health slider si está asignado
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.value = 1f;
            if (healthContainer != null) healthContainer.SetActive(true);
        }

        // Inicializa dash slider si está asignado
        if (dashSlider != null)
        {
            dashSlider.minValue = 0f;
            dashSlider.maxValue = 1f;
            dashSlider.value = 1f;
            if (dashContainer != null) dashContainer.SetActive(true);
        }
    }

    // Actualiza todo junto (útil al iniciar)
    public void UpdateAll(int kills, float elapsedTime, int combo)
    {
        UpdateKills(kills);
        UpdateTime(elapsedTime);
        UpdateCombo(combo);
    }

    public void UpdateKills(int kills)
    {
        if (killsText != null)
            killsText.text = "Kills: " + kills;
    }

    // elapsedTime en segundos
    public void UpdateTime(float elapsedTime)
    {
        if (timeText != null)
            timeText.text = "Time: " + Mathf.RoundToInt(elapsedTime) + "s";
    }

    public void UpdateCombo(int combo)
    {
        if (comboText != null)
            comboText.text = "Combo: " + combo;
    }

    // healthNormalized entre 0..1
    public void UpdateHealth(float healthNormalized)
    {
        float norm = Mathf.Clamp01(healthNormalized);
        if (healthText != null)
        {
            float pct = norm * 100f;
            healthText.text = Mathf.RoundToInt(pct) + "%";
        }

        if (healthSlider != null)
            healthSlider.value = norm;
    }

    // DASH: recibe un valor normalizado 0..1
    public void UpdateDash(float normalized)
    {
        if (dashSlider != null)
            dashSlider.value = Mathf.Clamp01(normalized);
    }

    public void ShowHUD(bool show)
    {
        if (hudRoot != null) hudRoot.SetActive(show);
    }

    public void ShowDashBar(bool show)
    {
        if (dashContainer != null) dashContainer.SetActive(show);
    }
}
