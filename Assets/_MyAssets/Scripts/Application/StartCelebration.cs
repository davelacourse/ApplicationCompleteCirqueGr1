using UnityEngine;

/// <summary>
/// Mise en scène du début de la partie : feux d'artifice et confettis, une seule fois,
/// au moment où le joueur clique sur Commencer. Il écoute le GameManager, comme les menus.
/// </summary>
public class StartCelebration : MonoBehaviour
{
    [Tooltip("L'Audio Source des feux d'artifice (SFX_Fireworks, Play On Awake décoché).")]
    [SerializeField] AudioSource _fireworksAudio;

    [Tooltip("Les confettis (Play On Awake décoché).")]
    [SerializeField] ParticleSystem _confetti;

    bool _hasPlayed;

    void OnEnable()
    {
        GameManager.Instance.StateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        // En fin de scène, le GameManager peut déjà être détruit.
        if (GameManager.Instance != null)
            GameManager.Instance.StateChanged -= OnStateChanged;
    }

    void OnStateChanged(GameState state)
    {
        // Seulement au premier passage à Playing :
        // reprendre après une pause ne refait pas la fête.
        if (state != GameState.Playing || _hasPlayed)
            return;

        _hasPlayed = true;
        _fireworksAudio.Play();
        _confetti.Play();
    }
}
