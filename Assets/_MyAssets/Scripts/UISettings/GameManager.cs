using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Déroulement de l'application : quitter et relancer la scène (cours 7).
/// Singleton : les autres scripts l'appellent par GameManager.Instance.
/// </summary>
public class GameManager : MonoBehaviour
{
    static GameManager _instance;

    public static GameManager Instance => _instance;

    void Awake()
    {
        // Un seul GameManager : un doublon se détruit.
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        // Dans l'éditeur, Application.Quit ne fait rien : on arrête plutôt le Play.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Recharge la scène active. Elle doit figurer dans la Scene List des Build Profiles.
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
