using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Les trois états de l'application.
public enum GameState
{
    Menu,       // menu de départ : le joueur regarde autour de lui, mais ne se déplace pas
    Playing,    // la partie est en cours : le joueur est libre
    Paused      // menu de pause ouvert : le joueur est retenu, le monde continue de vivre
}

/// <summary>
/// Déroulement de l'application : l'état de la partie, la pause, recommencer et quitter.
/// C'est le seul script qui tient l'état ; les autres l'écoutent par l'événement
/// StateChanged.
/// Singleton : les autres scripts l'appellent par GameManager.Instance, sans rien glisser.
/// </summary>
// Son Awake passe avant celui des autres scripts : Instance est prêt quand ils s'abonnent.
[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Tooltip("Le ScreenFader de la Main Camera : fondu avant de recommencer ou de quitter.")]
    [SerializeField] ScreenFader _fader;

    // L'application commence toujours au menu de départ.
    GameState _state = GameState.Menu;

    // Vrai dès qu'un rechargement ou une sortie est lancé : un deuxième clic ne fait rien.
    bool _isLeaving;

    public GameState State => _state;

    // Annonce chaque changement d'état à qui veut l'entendre.
    public event Action<GameState> StateChanged;

    void Awake()
    {
        // Un seul GameManager : un doublon se détruit.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void StartGame()
    {
        if (_state == GameState.Menu)
            SetState(GameState.Playing);
    }

    public void Pause()
    {
        if (_state == GameState.Playing)
            SetState(GameState.Paused);
    }

    public void Resume()
    {
        if (_state == GameState.Paused)
            SetState(GameState.Playing);
    }

    // Appelée par la montre : ouvre ou ferme le menu de pause.
    public void TogglePause()
    {
        if (_state == GameState.Playing)
            Pause();
        else if (_state == GameState.Paused)
            Resume();
    }

    public void Restart()
    {
        if (_isLeaving)
            return;
        StartCoroutine(RestartAfterFade());
    }

    public void Quit()
    {
        if (_isLeaving)
            return;
        StartCoroutine(QuitAfterFade());
    }

    // Le seul endroit où l'état change. Un état identique n'est pas annoncé de nouveau.
    void SetState(GameState state)
    {
        if (state == _state)
            return;

        _state = state;
        StateChanged?.Invoke(_state);
    }

    IEnumerator RestartAfterFade()
    {
        _isLeaving = true;
        yield return _fader.FadeOut();

        // Chargement asynchrone : le casque garde l'image noire du fondu pendant le
        // chargement, au lieu de figer la dernière image de la scène.
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator QuitAfterFade()
    {
        _isLeaving = true;
        yield return _fader.FadeOut();

#if UNITY_EDITOR
        // Dans l'éditeur, Application.Quit ne fait rien : on arrête plutôt le Play.
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
