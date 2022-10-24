using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static float H; // Horizontal
    public static float V; //Vertical
    public static bool isJump;
    public static bool isAttack;
    public static bool isDash;

    PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.PlayerInputMap.Enable();
    }

    private void Update()
    {
        HandleWASDInput();
        HandleJumpInput();
        HandleAttackInput();
        HandleDashInput();
    }

    private void HandleWASDInput()
    {
        H = playerInputActions.PlayerInputMap.Movement.ReadValue<Vector2>().x;
        V = playerInputActions.PlayerInputMap.Movement.ReadValue<Vector2>().y;
    }

    private void HandleJumpInput()
    {
        isJump = playerInputActions.PlayerInputMap.Jump.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
    }
    private void HandleAttackInput()
    {
        isAttack = playerInputActions.PlayerInputMap.Attack.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
    }
    private void HandleDashInput()
    {
        isDash = playerInputActions.PlayerInputMap.Dash.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
    }
}
