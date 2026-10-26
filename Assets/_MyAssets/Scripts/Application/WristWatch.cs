using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Montre au poignet gauche : on la touche du doigt (poke) pour ouvrir ou fermer
/// le menu de pause.
/// Elle s'affiche seulement une fois la partie commencée.
/// </summary>
public class WristWatch : MonoBehaviour
{
    [Tooltip("Le bouton du cadran, enfoncé par le Poke Interactor de la main droite.")]
    [SerializeField] Button _menuButton;

    [Tooltip("Le cadran de la montre : caché au menu de départ.")]
    [SerializeField] GameObject _face;

    void OnEnable()
    {
        GameManager.Instance.StateChanged += OnStateChanged;
        _menuButton.onClick.AddListener(OnMenuPressed);
    }

    void OnDisable()
    {
        // En fin de scène, le GameManager peut déjà être détruit.
        if (GameManager.Instance != null)
            GameManager.Instance.StateChanged -= OnStateChanged;
        _menuButton.onClick.RemoveListener(OnMenuPressed);
    }

    void Start()
    {
        OnStateChanged(GameManager.Instance.State);
    }

    // Au menu de départ, le menu principal suffit : la montre attend le début de la partie.
    void OnStateChanged(GameState state)
    {
        _face.SetActive(state != GameState.Menu);
    }

    void OnMenuPressed()
    {
        GameManager.Instance.TogglePause();
    }
}
