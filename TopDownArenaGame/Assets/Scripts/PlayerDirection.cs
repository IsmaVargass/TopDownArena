using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Esta línea es solo para debugging visual, no afecta la rotación
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.white);

        // Physics.Raycast(ray, out hit) busca el primer Collider que golpea.
        if (Physics.Raycast(ray, out hit))
        {
            // 1. Obtener la posición de impacto del rayo, pero mantener la altura (Y) del jugador
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            // 2. Calcular la rotación que mira de la posición actual a targetPosition
            Quaternion rotation = Quaternion.LookRotation(targetPosition - transform.position);

            // 3. Aplicar la rotación con suavizado (10.0f)
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10.0f);
        }
    }
}