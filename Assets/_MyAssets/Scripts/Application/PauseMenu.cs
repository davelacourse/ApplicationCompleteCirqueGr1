using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu de pause, ouvert par la montre : Reprendre, Options, Recommencer et Quitter.
/// Son canvas porte un Lazy Follow : il suit la tête du joueur avec un léger délai, et
/// réapparaît devant lui chaque fois qu'il s'ouvre.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Tooltip("Le canvas du menu, avec son Lazy Follow : activé seulement pendant la pause.")]
    [SerializeField] GameObject _canvas;

    [Tooltip("Le panneau principal : Reprendre, Options, Recommencer et Quitter.")]
    [SerializeField] GameObject _mainPanel;

    [Tooltip("Le panneau d'options du cours 7 (prefab OptionsPanel).")]
    [SerializeField] GameObject _optionsPanel;

    [SerializeField] Button _resumeButton;
    [SerializeField] Button _optionsButton;
    [SerializeField] Button _restartButton;
    [SerializeField] Button _quitButton;

    [Tooltip("Le bouton Retour du panneau d'options.")]
    [SerializeField] Button _backButton;

    void OnEnable()
    {
        GameManager.Instance.StateChanged += OnStateChanged;
        _resumeButton.onClick.AddListener(OnResumeClicked);
        _optionsButton.onClick.AddListener(ShowOptions);
        _restartButton.onClick.AddListener(OnRestartClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
        _backButton.onClick.AddListener(ShowMain);
    }

    void OnDisable()
    {
        // En fin de scène, le GameManager peut déjà être détruit.
        if (GameManager.Instance != null)
            GameManager.Instance.StateChanged -= OnStateChanged;
        _resumeButton.onClick.RemoveListener(OnResumeClicked);
        _optionsButton.onClick.RemoveListener(ShowOptions);
        _restartButton.onClick.RemoveListener(OnRestartClicked);
        _quitButton.onClick.RemoveListener(OnQuitClicked);
        _backButton.onClick.RemoveListener(ShowMain);
    }

    void Start()
    {
        OnStateChanged(GameManager.Instance.State);
    }

    void OnStateChanged(GameState state)
    {
        bool isPaused = state == GameState.Paused;

        // Réactiver le canvas déclenche le Snap On Enable du Lazy Follow :
        // le menu apparaît directement devant le joueur, où qu'il regarde.
        _canvas.SetActive(isPaused);
        if (isPaused)
            ShowMain();
    }

    void ShowMain()
    {
        _mainPanel.SetActive(true);
        _optionsPanel.SetActive(false);
    }

    void ShowOptions()
    {
        _mainPanel.SetActive(false);
        _optionsPanel.SetActive(true);
    }

    void OnResumeClicked()
    {
        GameManager.Instance.Resume();
    }

    void OnRestartClicked()
    {
        GameManager.Instance.Restart();
    }

    void OnQuitClicked()
    {
        GameManager.Instance.Quit();
    }
}
