using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Referencia al Transform del jugador.")]
    public Transform Player;

    [Header("CONFIGURACIÓN DE LA VISTA")]
    [Tooltip("Distancia en el eje Y (Altura) de la cámara al jugador.")]
    public float camOffsetY = 15f; // Valor alto para vista aérea

    [Tooltip("Distancia en el eje Z (Profundidad) de la cámara al jugador.")]
    public float camOffsetZ = -10f; // Valor negativo para estar detrás del jugador

    // No necesitamos una variable 'private' de offset ya que usaremos las públicas directamente.

    void Start()
    {
        // No necesitamos calcular el offset, ya que lo leeremos de las variables públicas.
        // Solo verificamos si el jugador existe.
        if (Player == null)
        {
            Debug.LogError("La referencia 'Player' en CameraFollow no está asignada.");
        }
    }

    void LateUpdate()
    {
        // Usamos LateUpdate para asegurar que el jugador haya terminado de moverse antes de que la cámara lo siga.

        // Si el jugador fue destruido, sal del método
        if (Player == null) return;

        // Calcula la posición objetivo de la cámara:
        Vector3 m_cameraPos = new Vector3(
            // Sigue la posición X del jugador
            Player.position.x,
            // Usa el offset vertical (camOffsetY) en lugar de la posición inicial de la cámara
            Player.position.y + camOffsetY,
            // Sigue la posición Z del jugador + el offset de profundidad
            Player.position.z + camOffsetZ
        );

        // Aplica la nueva posición
        transform.position = m_cameraPos;
    }
}