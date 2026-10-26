using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interface du panneau d'options : place les contrôles selon les préférences
/// actuelles, puis transmet chaque changement au SettingsManager.
/// Cours 9 : il est posé sur la racine du prefab OptionsPanel, réutilisé dans le menu
/// de départ et dans le menu de pause. Quitter et Relancer sont passés dans ces menus.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] SettingsManager _settings;
    [SerializeField] Slider _ambienceSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] Toggle _smoothTurnToggle;
    [SerializeField] Toggle _vignetteToggle;

    // S'abonner dans OnEnable et se désabonner dans OnDisable, par paire,
    // comme pour les événements des interactables au cours 6.
    void OnEnable()
    {
        _ambienceSlider.onValueChanged.AddListener(_settings.SetAmbienceVolume);
        _sfxSlider.onValueChanged.AddListener(_settings.SetSfxVolume);
        _smoothTurnToggle.onValueChanged.AddListener(_settings.SetSmoothTurn);
        _vignetteToggle.onValueChanged.AddListener(_settings.SetVignette);

        // Deux panneaux d'options existent maintenant : chaque fois que celui-ci
        // s'affiche, il reprend les préférences actuelles, au cas où l'autre les a changées.
        ShowSettings();
    }

    void OnDisable()
    {
        _ambienceSlider.onValueChanged.RemoveListener(_settings.SetAmbienceVolume);
        _sfxSlider.onValueChanged.RemoveListener(_settings.SetSfxVolume);
        _smoothTurnToggle.onValueChanged.RemoveListener(_settings.SetSmoothTurn);
        _vignetteToggle.onValueChanged.RemoveListener(_settings.SetVignette);
    }

    // Le SettingsManager a lu les préférences dans son Awake : elles sont prêtes.
    // (Un panneau actif au lancement peut passer son OnEnable avant cet Awake.)
    void Start()
    {
        ShowSettings();
    }

    void ShowSettings()
    {
        // WithoutNotify : on place le contrôle sans déclencher son événement.
        _ambienceSlider.SetValueWithoutNotify(_settings.AmbienceVolume);
        _sfxSlider.SetValueWithoutNotify(_settings.SfxVolume);
        _smoothTurnToggle.SetIsOnWithoutNotify(_settings.SmoothTurn);
        _vignetteToggle.SetIsOnWithoutNotify(_settings.Vignette);
    }
}
