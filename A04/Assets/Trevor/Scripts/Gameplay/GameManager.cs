using UnityEngine;
using UnityEngine.SceneManagement;
using System; // Required for Action
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Start, Play, Pause, Win, Lose }
    public GameState currentState;

    // The Event that the Audio Manager listens for
    public event Action<GameState> OnStateChanged;

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject winScreenUI;
    public GameObject loseScreenUI;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        // Initialize the game
        ChangeState(GameState.Start);
    }

    void Update()
    {
        // Handle Input here
        if (currentState == GameState.Play)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // This calls ChangeState, which invokes the event
                ChangeState(GameState.Pause);
            }
        }
        else if (currentState == GameState.Pause)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ResumeGame();
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        // 
        // 1. FIRE EVENT FIRST: This ensures Audio plays BEFORE Time.timeScale hits 0
        OnStateChanged?.Invoke(newState);

        // 2. EXECUTE STATE LOGIC
        switch (currentState)
        {
            case GameState.Start: EnterStartState(); break;
            case GameState.Play: EnterPlayState(); break;
            case GameState.Pause: EnterPauseState(); break;
            case GameState.Win: EnterWinState(); break;
            case GameState.Lose: EnterLoseState(); break;
        }
    }

    void EnterStartState()
    {
        Debug.Log("Entering START state...");
        ChangeState(GameState.Play);
    }

    void EnterPlayState()
    {
        Time.timeScale = 1f; // Unfreeze time
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (winScreenUI != null) winScreenUI.SetActive(false);
        if (loseScreenUI != null) loseScreenUI.SetActive(false);
    }

    void EnterPauseState()
    {
        Time.timeScale = 0f; // Freeze time
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
    }

    void EnterWinState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (winScreenUI != null) winScreenUI.SetActive(true);
    }

    void EnterLoseState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (loseScreenUI != null) loseScreenUI.SetActive(true);
    }

    // --- Helper Functions ---

    public void TriggerWin()
    {
        if (currentState == GameState.Play) StartCoroutine(WinSequenceRoutine());
    }

    System.Collections.IEnumerator WinSequenceRoutine()
    {
        yield return new WaitForSeconds(2f);
        ChangeState(GameState.Win);
    }

    public void TriggerLose()
    {
        if (currentState == GameState.Play) ChangeState(GameState.Lose);
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Pause) ChangeState(GameState.Play);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Important: Unfreeze before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Important: Unfreeze before leaving
        SceneManager.LoadScene("MainMenu");
    }
}