using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float playerSpeed = 6f;

    [Header("Input (optional)")]
#if ENABLE_INPUT_SYSTEM
    // Arrastra aquí la Action "Move" (Vector2) desde tu InputActions asset
    public InputActionReference moveAction;
#endif

    CharacterController m_charCont;

    Vector2 moveValue; // from new InputSystem
    float horiz;
    float vert;

    void Start()
    {
        m_charCont = GetComponent<CharacterController>();
        if (m_charCont == null)
            Debug.LogError("PlayerMovement -> falta CharacterController en el GameObject");
    }

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Enable();
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        if (moveAction != null && moveAction.action != null)
            moveAction.action.Disable();
#endif
    }

    void Update()
    {
        // 1. Protección contra la pausa (Time.timeScale = 0)
        if (Time.timeScale <= 0.01f) return;

        // --- Read input: prefer Input System if available and assigned, otherwise fallback to old Input ---
#if ENABLE_INPUT_SYSTEM
        if (moveAction != null && moveAction.action != null)
        {
            moveValue = moveAction.action.ReadValue<Vector2>();
            horiz = moveValue.x;
            vert = moveValue.y;
        }
        else
        {
            // fallback to old Input (only works if Project Settings Active Input Handling = Both or Old)
            horiz = Input.GetAxis("Horizontal");
            vert = Input.GetAxis("Vertical");
        }
#else
        // project not compiled with Input System package support — use old Input
        horiz = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
#endif

        // --- Movement with CharacterController ---
        Vector3 input = new Vector3(horiz, 0f, vert);
        if (input.sqrMagnitude > 1f) input = input.normalized; // evitar diagonal más rápida

        // ¡IMPORTANTE! El movimiento es relativo al mundo, NO a la rotación del jugador
        // Si quieres que el movimiento sea relativo a la dirección a la que mira el jugador,
        // deberías mover esta lógica al PlayerController.
        Vector3 move = input * playerSpeed * Time.deltaTime;

        // CharacterController.Move aplicará colisiones automáticamente
        if (m_charCont != null)
            m_charCont.Move(move);
    }
}