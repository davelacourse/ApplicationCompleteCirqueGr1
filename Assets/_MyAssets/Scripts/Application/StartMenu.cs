using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Menu de départ, placé devant le joueur à son arrivée : Commencer, Options et Quitter.
/// Il disparaît dès que la partie commence. Il ne décide de rien : il transmet les clics
/// au GameManager.
/// </summary>
public class StartMenu : MonoBehaviour
{
    [Tooltip("Le panneau principal : Commencer, Options et Quitter.")]
    [SerializeField] GameObject _mainPanel;

    [Tooltip("Le panneau d'options du cours 7 (prefab OptionsPanel).")]
    [SerializeField] GameObject _optionsPanel;

    [SerializeField] Button _startButton;
    [SerializeField] Button _optionsButton;
    [SerializeField] Button _quitButton;

    [Tooltip("Le bouton Retour du panneau d'options.")]
    [SerializeField] Button _backButton;

    void OnEnable()
    {
        GameManager.Instance.StateChanged += OnStateChanged;
        _startButton.onClick.AddListener(OnStartClicked);
        _optionsButton.onClick.AddListener(ShowOptions);
        _quitButton.onClick.AddListener(OnQuitClicked);
        _backButton.onClick.AddListener(ShowMain);
    }

    void OnDisable()
    {
        // En fin de scène, le GameManager peut déjà être détruit.
        if (GameManager.Instance != null)
            GameManager.Instance.StateChanged -= OnStateChanged;
        _startButton.onClick.RemoveListener(OnStartClicked);
        _optionsButton.onClick.RemoveListener(ShowOptions);
        _quitButton.onClick.RemoveListener(OnQuitClicked);
        _backButton.onClick.RemoveListener(ShowMain);
    }

    void Start()
    {
        OnStateChanged(GameManager.Instance.State);
    }

    // Visible seulement au menu de départ.
    void OnStateChanged(GameState state)
    {
        if (state == GameState.Menu)
        {
            ShowMain();
        }
        else
        {
            _mainPanel.SetActive(false);
            _optionsPanel.SetActive(false);
        }
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

    void OnStartClicked()
    {
        GameManager.Instance.StartGame();
    }

    void OnQuitClicked()
    {
        GameManager.Instance.Quit();
    }
}
