using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class MainMenu : MonoBehaviour
{
    // Make sure your main game scene is named "GameScene"
    // Or you can change this string to match your scene's name.
    public string gameSceneName = "GameScene";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// This function will be called by the OnClick() event of your Play Button.
    /// </summary>
    public void PlayGame()
    {
        // Loads the scene specified by the name.
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// This function can be hooked to a "Quit" button.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}