using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private PlayerController playerController;
    private float vidaMaxima;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        vidaMaxima = playerController.vidaMaxima;
    }

    void Update()
    {
        rellenoBarraVida.fillAmount = playerController.vida / vidaMaxima;
    }

}
