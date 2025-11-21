using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // --- SINGLETON ---
    // This makes it so any other script can access this GameManager
    // by simply typing "GameManager.Instance"
    public static GameManager Instance { get; private set; }

    // --- GAME STATES ---
    // We define all the possible states our game can be in.
    public enum GameState
    {
        Start,
        Play,
        Pause,
        Win,
        Lose
    }

    // This variable will hold the game's current state.
    public GameState currentState;

    // --- UI REFERENCES ---
    // Drag your UI panels from the Hierarchy into these slots in the Inspector.
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject winScreenUI;
    public GameObject loseScreenUI;

    // --- SETUP ---
    void Awake()
    {
        // Set up the Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate GameManagers
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional: if you want it to persist between scenes
        }
    }

    void Start()
    {
        // The game always begins in the Start state.
        ChangeState(GameState.Start);
    }

    // --- STATE MACHINE ---
    // This is the main function that manages state transitions.
    public void ChangeState(GameState newState)
    {
        // Set the new state
        currentState = newState;

        // Run logic based on which state we just entered
        switch (currentState)
        {
            case GameState.Start:
                EnterStartState();
                break;

            case GameState.Play:
                EnterPlayState();
                break;

            case GameState.Pause:
                EnterPauseState();
                break;

            case GameState.Win:
                EnterWinState();
                break;

            case GameState.Lose:
                EnterLoseState();
                break;
        }
    }

    // --- STATE LOGIC ---
    // These functions run ONCE when the state is first entered.

    void EnterStartState()
    {
        Debug.Log("Entering START state...");

        // --- PLACEHOLDER ---
        // This is where you will call your other systems.
        // DungeonGenerator.Instance.GenerateDungeon();
        // PlayerSpawner.Instance.SpawnPlayer();
        // EnemyManager.Instance.SpawnEnemies();

        // Once setup is done, immediately move to the Play state.
        Debug.Log("Startup complete. Moving to PLAY state.");
        ChangeState(GameState.Play);
    }

    // --- Inside GameManager.cs ---

    void EnterPlayState()
    {
        Debug.Log("Entering PLAY state...");
        Time.timeScale = 1f;

        // --- ADD THESE LINES ---
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // ---------------------

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (winScreenUI != null) winScreenUI.SetActive(false);
        if (loseScreenUI != null) loseScreenUI.SetActive(false);
    }

    void EnterPauseState()
    {
        Debug.Log("Entering PAUSE state...");
        Time.timeScale = 0f;

        // --- ADD THESE LINES ---
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // ---------------------

        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    void EnterWinState()
    {
        Debug.Log("Entering WIN state...");

        // --- ADD THESE LINES ---
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // ---------------------

        if (winScreenUI != null) winScreenUI.SetActive(true);
    }

    void EnterLoseState()
    {
        Debug.Log("Entering LOSE state...");

        // --- ADD THESE LINES ---
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // ---------------------

        if (loseScreenUI != null) loseScreenUI.SetActive(true);
    }


    // --- UPDATE LOOP ---
    // The Update function checks for input that changes the state.
    void Update()
    {
        // We only want to check for the pause button if we are in the Play state.
        if (currentState == GameState.Play)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Go to Pause state
                ChangeState(GameState.Pause);
            }

        }
        // If we are paused, check for the "unpause" button
        else if (currentState == GameState.Pause)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Go back to Play state
                ResumeGame();
            }
        }
    }


    // --- PUBLIC HELPER FUNCTIONS ---
    // These are functions that other scripts (like UI buttons) can call.

    // --- Functions for other scripts to call ---
    public void TriggerWin()
    {
        if (currentState == GameState.Play)
        {
            // Start the delay coroutine instead of switching state immediately
            StartCoroutine(WinSequenceRoutine());
        }
    }

    // Add the namespace: using System.Collections; at the top of the script
    System.Collections.IEnumerator WinSequenceRoutine()
    {
        // Optional: Play a "All Collected!" sound effect here

        // Wait for 2 seconds so the player can see the last sticker
        yield return new WaitForSeconds(2f);

        ChangeState(GameState.Win);
    }

    public void TriggerLose()
    {
        if (currentState == GameState.Play) // Only lose if you are playing
        {
            ChangeState(GameState.Lose);
        }
    }

    // --- Functions for UI Buttons ---
    public void ResumeGame()
    {
        // This is called by the "Resume" button on the pause menu.
        if (currentState == GameState.Pause)
        {
            ChangeState(GameState.Play);
        }
    }

    public void RestartGame()
    {
        // This is called by "Restart" buttons on the Pause, Win, or Lose screens.

        // IMPORTANT: Ensure time is unfrozen before reloading
        Time.timeScale = 1f;

        // Reloads the currently active scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        // This is called by "Main Menu" buttons.

        // IMPORTANT: Ensure time is unfrozen before leaving
        Time.timeScale = 1f;

        // --- !!! CHANGE THIS !!! ---
        // Make sure to change "MainMenuScene" to the exact name of your main menu scene.
        SceneManager.LoadScene("MainMenu");
    }
}