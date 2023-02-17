using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool attackInput;
    public bool interactInput;
    public bool pauseInput;
    private static bool gamePaused;
    public bool changeSpell;

    PlayerControls inputActions;
    PlayerManager playerManager;

    Vector2 movementInput;
    Vector2 scrollWheel;

    public GameObject pauseMenu;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();

        pauseMenu = GameObject.Find("Pause Menu");
        if (pauseMenu != null)
        {
            for (int i = 0; i < pauseMenu.transform.childCount; i++)
            {
                pauseMenu.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        gamePaused = true;
        PauseGame();
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
            inputActions.PlayerActions.Pause.performed += i => pauseInput = true;
            inputActions.SpellCasting.ChangeSpell.performed += i => changeSpell = true;
            inputActions.SpellCasting.CycleSpell.performed += inputActions => scrollWheel = inputActions.ReadValue<Vector2>().normalized;
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if (pauseInput)
        {
            PauseGame();
        }
    }

    private void LateUpdate()
    {
        pauseInput = false;
    }
    public void PauseGame()
    {
        if (gamePaused)
        {
            Time.timeScale = 1f;
            pauseMenu = GameObject.Find("Pause Menu");
            if (pauseMenu != null) {
                for (int i = 0; i < pauseMenu.transform.childCount; i++)
                {
                    pauseMenu.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            
        }
        else
        {
            Time.timeScale = 0f;
            pauseMenu = GameObject.Find("Pause Menu");
            if (pauseMenu != null)
            {
                for (int i = 0; i < pauseMenu.transform.childCount; i++)
                {
                    pauseMenu.transform.GetChild(i).gameObject.SetActive(true);
                }
            }
        }

        gamePaused = !gamePaused;
    }

    public static void ChangeScene(string targetScene)
    {
        // save the stats/position for every character in the current scene
        foreach(CharacterManager character in FindObjectsOfType<CharacterManager>())
        {
            character.UpdateStats();
        }

        // transition to the next scene
        if (GameObject.FindGameObjectWithTag("Music") != null)
        {
            GameObject.FindGameObjectWithTag("Music").GetComponent<Music>().StopMusic();
        }
        SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
    }

    public void TickInput(float delta)
    {
        if (gamePaused) { return; }

        HanldeMoveInput(delta);
        HandleAttackInput(delta);
        HandleInteractInput(delta);
        HandleChangeSpellInput(delta);
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
            playerManager.HandleAttack();
        }
    }

    private void HandleInteractInput(float delta)
    {
        if (interactInput)
        {
            playerManager.HandleInteract();
        }
    }

    private void HandleChangeSpellInput(float delta)
    {
        if (changeSpell)
        {
            playerManager.ChangeActiveSpell(1f);
        }
        else if(scrollWheel.y != 0f)
        {
            playerManager.ChangeActiveSpell(scrollWheel.y);
        }
    }
}
