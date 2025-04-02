using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerCustom : MonoBehaviour
{
    public static SceneManagerCustom Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps SceneManager alive
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        // Reset GameManager values before restarting
        ResetGameManager();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void LoadMainMenu()
    {
        ResetGameManager(); // Reset progress when going to the main menu
        SceneManager.LoadScene(0);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetGameManager()
    {
        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject); // Reset player progress
        }
    }
}
