using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("CONFIGURACIÓN DE ENEMIGOS TERRESTRES")]
    public GameObject enemyPrefab;
    public float spawnRate = 2f;
    public int maxEnemies = 200;
    public Vector3 spawnArea = new Vector3(45, 0, 45);

    [Header("Crecimiento (solo afecta al tiempo y cantidad por tick)")]
    public float growthInterval = 20f;      // cada cuánto (s) aumenta la cantidad por tick
    public int growthPerInterval = 2;      // cuánto aumenta la cantidad por cada intervalo
    public int maxSpawnPerTick = 8;        // tope de enemigos por tick (por seguridad)

    private float timer = 0f;
    private float elapsed = 0f;
    private int enemiesSpawned = 0;
    private bool playerAlive = true;
    private Transform playerTransform;

    // --- NUEVAS VARIABLES PARA BOMBAS AÉREAS ---
    [Header("CONFIGURACIÓN DE BOMBAS AÉREAS")]
    [Tooltip("Prefab de la Bomba Aérea (AirBombPrefab).")]
    public GameObject airBombPrefab;

    [Tooltip("Tiempo en segundos entre la caída de cada bomba.")]
    public float bombSpawnRate = 10f;

    [Tooltip("Altura desde la que caerán las bombas (ej. 20 unidades por encima de la arena).")]
    public float dropHeight = 15f;

    [Tooltip("Distancia horizontal del jugador a la que caerá la bomba (para que sea visible).")]
    public float dropDistance = 8f; // ¡NUEVO! Controla dónde cae.

    private float bombTimer = 0f;
    private Camera mainCamera; // ¡NUEVO! Necesario para saber dónde mirar.
    // ------------------------------------------

    void Start()
    {
        // Buscar al jugador por tag (asegúrate que el Player tiene el tag "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player no encontrado. Asegúrate de que tiene el Tag 'Player'.");
        }

        // Obtener la cámara principal (¡CRUCIAL! Debe tener el tag "MainCamera")
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Cámara principal no encontrada. Asegúrate de que la cámara tenga el Tag 'MainCamera'.");
        }

        for (int i = 0; i < 3; i++)
        {
            SpawnEnemy();
        }
    }

    void Update()
    {
        // No hacer nada si el jugador está muerto
        if (!playerAlive) return;

        float dt = Time.deltaTime;
        timer += dt;
        elapsed += dt;
        bombTimer += dt;

        // --- LÓGICA DE SPAWN DE ENEMIGOS TERRESTRES ---
        if (timer >= spawnRate)
        {
            int spawnCount = CalculateSpawnCount();
            for (int i = 0; i < spawnCount; i++)
            {
                SpawnEnemy();
            }

            timer = 0f;

            // Si quieres que la frecuencia también aumente con el tiempo, mantenemos la reducción suave
            if (spawnRate > 0.3f) spawnRate -= 0.05f;
        }

        // --- LÓGICA DE SPAWN DE BOMBAS AÉREAS (Cada 10 segundos) ---
        if (bombTimer >= bombSpawnRate)
        {
            SpawnAirBomb();
            bombTimer = 0f;
        }
    }

    // Calcula cuántos enemigos deben aparecer por tick, en función del tiempo transcurrido
    int CalculateSpawnCount()
    {
        if (enemiesSpawned >= maxEnemies) return 0;

        int steps = Mathf.FloorToInt(elapsed / growthInterval);
        int result = 1 + steps * growthPerInterval;      // mínimo 1
        result = Mathf.Clamp(result, 1, maxSpawnPerTick);    // limitar por seguridad

        int remaining = maxEnemies - enemiesSpawned;
        if (result > remaining) result = remaining;           // no superar el total permitido

        return result;
    }

    /// <summary>
    /// Genera enemigos terrestres en una posición aleatoria en el plano XZ.
    /// </summary>
    void SpawnEnemy()
    {
        if (enemiesSpawned >= maxEnemies) return;
        if (enemyPrefab == null) return;

        // Genera posición XZ aleatoria dentro del área definida
        float x = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float z = Random.Range(-spawnArea.z / 2, spawnArea.z / 2);

        // Usa la altura del prefab de enemigo para que aparezca justo encima del suelo
        Vector3 spawnPos = new Vector3(x, enemyPrefab.transform.position.y, z);

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemiesSpawned++;
    }

    /// <summary>
    /// Genera una Bomba Aérea en el campo de visión del jugador.
    /// </summary>
    void SpawnAirBomb()
    {
        if (airBombPrefab == null || playerTransform == null || mainCamera == null)
        {
            // Error logged in Start(), but safety check here.
            return;
        }

        // 1. Obtener la dirección a la que está mirando la CÁMARA
        Vector3 cameraForward = mainCamera.transform.forward;
        // Ignoramos la componente Y para que solo sea en el plano horizontal (XZ)
        cameraForward.y = 0;
        cameraForward.Normalize();

        // 2. Calcula la posición en el suelo donde caerá la bomba.
        // Posición del jugador + (Dirección de la cámara * Distancia de caída)
        Vector3 targetGroundPosition = playerTransform.position + (cameraForward * dropDistance);

        // 3. Añadir algo de aleatoriedad (offset lateral) para que no sea predecible
        float lateralOffset = Random.Range(-2.5f, 2.5f); // Aleatoriedad de +/- 2.5 unidades

        // Usamos la dirección lateral de la cámara para el offset
        Vector3 cameraRight = mainCamera.transform.right;
        targetGroundPosition += cameraRight * lateralOffset;

        // 4. Establece la posición de spawn final (añadiendo la altura de caída)
        Vector3 spawnPos = new Vector3(
            targetGroundPosition.x,
            dropHeight, // La altura desde donde empieza a caer
            targetGroundPosition.z
        );

        // 5. Instancia la bomba
        Instantiate(airBombPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"💣 Bomba lanzada visiblemente hacia: {targetGroundPosition}");
    }

    // 🔥 Llama a este método cuando el jugador muera
    public void StopSpawning()
    {
        playerAlive = false;
        Debug.Log("❌ Spawner detenido (jugador muerto)");
    }
}