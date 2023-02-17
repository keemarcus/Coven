using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class PlayerManager : CharacterManager
{
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;

    public float attackCooldown;

    void Start()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();

        attackCooldown = 0f;
    }

    void Update()
    {
        //if (isInteracting) { return; }
        float delta = Time.deltaTime;

        // take player inputs
        inputManager.TickInput(delta);

        if(attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        //if (isDead) { inputManager.moveAmount = 0; return; }
        if (isDead || isInteracting) { inputManager.moveAmount = 0; }
        float delta = Time.fixedDeltaTime;

        // move the player
        playerLocomotion.HandleMovement(delta);

        // update the animator
        animationManager.UpdateAnimator(delta, inputManager.moveAmount, new Vector2(inputManager.horizontal, inputManager.vertical));

        if (cameraManager != null)
        {
            cameraManager.FollowTarget(delta);
        }
    }

    private void LateUpdate()
    {
        // reset all our input variables at the end of the frame so they can only be triggered once per press
        inputManager.attackInput = false;
        inputManager.interactInput = false;
    }

    public void HandleInteract()
    {
        // look for nearby things to interact with
        Interactable[] interactableObjects =  FindObjectsOfType<Interactable>();
        foreach(Interactable interactable in interactableObjects)
        {
            // if we're close enough to interact with one, do so and return
            if(Vector2.Distance(this.transform.position, interactable.transform.position) < 1.5f)
            {
                interactable.Interact();
                return;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.GetComponent<SceneTransitionManager>())
        {
            SceneTransitionManager transition = collision.collider.gameObject.GetComponent<SceneTransitionManager>();
            UpdateStats(transition.GetTargetSceneName(), transition.targetPosition, transition.targetDirection);
            transition.TransitionScene();
        }
    }
}
