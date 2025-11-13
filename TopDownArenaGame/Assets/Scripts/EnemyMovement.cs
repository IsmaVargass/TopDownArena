using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public int EnemySpeed;
    GameObject m_player;

    private void Awake()
    {
        m_player = GameObject.Find("Player");
    }

    void Update()
    {
        // 👇 si el jugador fue destruido, no sigas ejecutando el código
        if (m_player == null) return;

        Vector3 localPosition = m_player.transform.position - transform.position;
        localPosition = localPosition.normalized;
        transform.Translate(localPosition.x * Time.deltaTime * EnemySpeed,
                            0f,
                            localPosition.z * Time.deltaTime * EnemySpeed);
    }
}
