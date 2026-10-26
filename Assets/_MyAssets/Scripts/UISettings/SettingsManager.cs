using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

/// <summary>
/// Préférences du joueur : volume des groupes du mixer, rotation continue et vignette.
/// Les lit au démarrage, les applique, et les enregistre avec PlayerPrefs chaque fois
/// qu'elles changent. Ne connaît aucun élément d'interface.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    // Clés PlayerPrefs. Pour les volumes, ce sont aussi les noms des paramètres
    // exposés dans l'Audio Mixer.
    const string AmbienceKey = "AmbienceVolume";
    const string SfxKey = "SfxVolume";
    const string SmoothTurnKey = "SmoothTurn";
    const string VignetteKey = "Vignette";

    [SerializeField] AudioMixer _mixer;

    [Tooltip("Le ControllerInputActionManager du RightController.")]
    [SerializeField] ControllerInputActionManager _rightController;

    [Tooltip("Le Tunneling Vignette Controller de la Main Camera.")]
    [SerializeField] TunnelingVignetteController _vignette;

    float _ambienceVolume;
    float _sfxVolume;
    bool _smoothTurn;
    bool _vignetteOn;

    public float AmbienceVolume => _ambienceVolume;
    public float SfxVolume => _sfxVolume;
    public bool SmoothTurn => _smoothTurn;
    public bool Vignette => _vignetteOn;

    // Awake : on lit les préférences tôt, pour que l'UIManager les trouve prêtes
    // dans son Start. Tous les Awake passent avant tous les Start.
    void Awake()
    {
        _ambienceVolume = PlayerPrefs.GetFloat(AmbienceKey, 1f);
        _sfxVolume = PlayerPrefs.GetFloat(SfxKey, 1f);
        _smoothTurn = PlayerPrefs.GetInt(SmoothTurnKey, 0) == 1;
        _vignetteOn = PlayerPrefs.GetInt(VignetteKey, 1) == 1;
    }

    // Start : on les applique. Le mixer n'accepte pas encore SetFloat pendant Awake.
    void Start()
    {
        SetAmbienceVolume(_ambienceVolume);
        SetSfxVolume(_sfxVolume);
        SetSmoothTurn(_smoothTurn);
        SetVignette(_vignetteOn);
    }

    public void SetAmbienceVolume(float value)
    {
        _ambienceVolume = ApplyVolume(AmbienceKey, value);
    }

    public void SetSfxVolume(float value)
    {
        _sfxVolume = ApplyVolume(SfxKey, value);
    }

    public void SetSmoothTurn(bool on)
    {
        _smoothTurn = on;
        _rightController.smoothTurnEnabled = on;
        PlayerPrefs.SetInt(SmoothTurnKey, on ? 1 : 0);
    }

    public void SetVignette(bool on)
    {
        _vignetteOn = on;
        _vignette.enabled = on;
        PlayerPrefs.SetInt(VignetteKey, on ? 1 : 0);
    }

    float ApplyVolume(string parameter, float value)
    {
        // L'oreille perçoit le volume de façon logarithmique :
        // 1 donne 0 dB, 0,0001 donne -80 dB, le silence pour le mixer.
        value = Mathf.Max(value, 0.0001f);
        _mixer.SetFloat(parameter, Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat(parameter, value);
        return value;
    }
}
