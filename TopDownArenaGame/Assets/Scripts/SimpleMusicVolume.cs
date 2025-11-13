using UnityEngine;
using UnityEngine.UI;

public class SimpleMusicVolume : MonoBehaviour
{
    // Las referencias se asignan en el Inspector
    [Header("CONFIGURACIÓN")]
    [Tooltip("El AudioSource que está reproduciendo la música de fondo (BGM).")]
    public AudioSource musicSource;

    [Tooltip("El Slider de Música del menú de opciones.")]
    public Slider musicSlider;

    // Clave para guardar el valor del volumen
    private const string MUSIC_VOLUME_KEY = "MusicVolumeSimple";

    void Start()
    {
        if (musicSource == null || musicSlider == null)
        {
            Debug.LogError("¡ERROR! AudioSource o Slider no asignados en SimpleMusicVolume.");
            return;
        }

        // 1. Cargar el último volumen guardado. 
        // Si no hay valor, lo inicializamos en 0.75 (3/4 del volumen total)
        float savedVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.75f);

        // 2. Aplicar el volumen cargado al Slider para que refleje el valor.
        musicSlider.value = savedVolume;

        // 3. Aplicar el volumen al AudioSource al inicio.
        ApplyVolume(savedVolume);

        // 4. Conectar la función al Slider.
        // Cada vez que el Slider se mueva, ApplyVolume(float volume) será llamado.
        musicSlider.onValueChanged.AddListener(ApplyVolume);
    }

    /// <summary>
    /// Recibe el valor del Slider (0.0 a 1.0) y lo aplica al AudioSource.
    /// </summary>
    /// <param name="volume">Valor del Slider (0.0 a 1.0).</param>
    public void ApplyVolume(float volume)
    {
        // 1. Asignar el volumen directamente al AudioSource
        // (0 es silencio, 1 es volumen máximo)
        musicSource.volume = volume;

        // 2. Guardar el valor en PlayerPrefs para la próxima sesión
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }
}