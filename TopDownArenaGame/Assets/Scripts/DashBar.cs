using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashBar : MonoBehaviour
{
    public Image rellenoBarraDash;       // asigna en Inspector (Image con Type=Filled)
    private PlayerDash playerDash;
    private Transform playerTransform;

    void Start()
    {
        // Buscamos al player por tag "Player" si lo tienes, si no por nombre "Player"
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null)
        {
            playerObj = GameObject.Find("Player");
        }

        if (playerObj != null)
        {
            playerDash = playerObj.GetComponent<PlayerDash>();
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("[DashBar] No se encontró el objeto Player por tag ni por nombre.");
        }
    }

    void Update()
    {
        if (playerDash == null)
        {
            // intentar encontrar dinámicamente si se instancia después
            var p = GameObject.FindWithTag("Player") ?? GameObject.Find("Player");
            if (p != null) playerDash = p.GetComponent<PlayerDash>();
        }

        if (playerDash != null && rellenoBarraDash != null)
        {
            rellenoBarraDash.fillAmount = playerDash.EnergyFraction;
        }
    }
}
