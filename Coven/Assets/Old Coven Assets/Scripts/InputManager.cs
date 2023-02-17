using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool attackInput;
    public bool interactInput;

    PlayerControls inputActions;
    PlayerManager playerManager;

    Vector2 movementInput;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void OnEnable()
    {
        if (inputActions == null)
        {
            // set up new Player Controls instance
            inputActions = new PlayerControls();
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerActions.Attack.performed += i => attackInput = true;
            inputActions.PlayerActions.Interact.performed += i => interactInput = true;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float delta)
    {
        HanldeMoveInput(delta);
        HandleAttackInput(delta);
        HandleInteractInput(delta);
    }

    private void HanldeMoveInput(float delta)
    {
        // update moveAmount based on player input
        horizontal = movementInput.x;
        vertical = movementInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
    }

    private void HandleAttackInput(float delta)
    {
        if (attackInput)
        {
            if(playerManager.canDoCombo || playerManager.attackCooldown <= 0)
            {
                playerManager.HandleAttack();
            }
        }
    }

    private void HandleInteractInput(float delta)
    {
        if (interactInput)
        {
            playerManager.HandleInteract();
        }
    }
}
