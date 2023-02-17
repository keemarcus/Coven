using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Rigidbody2D body;
    InputManager inputManager;

    public float moveSpeed = 3f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<InputManager>();
    }

    public void HandleMovement(float delta)
    {
        if(inputManager.moveAmount > 0)
        {
            body.velocity = new Vector2(inputManager.horizontal, inputManager.vertical) * moveSpeed;
        }
        else { body.velocity = Vector2.zero; }
    }
}
