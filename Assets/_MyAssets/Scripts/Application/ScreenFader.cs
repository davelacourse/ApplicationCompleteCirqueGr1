using System.Collections;
using UnityEngine;

/// <summary>
/// Fondu au noir devant les yeux du joueur. Posé sur un petit canvas noir placé juste
/// devant la Main Camera : il ouvre la scène en fondu et la ferme avant un changement.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    [Tooltip("Durée d'un fondu, en secondes.")]
    [SerializeField] float _duration = 1f;

    [Tooltip("Ouvrir la scène en fondu depuis le noir.")]
    [SerializeField] bool _fadeInOnStart = true;

    CanvasGroup _group;

    public float Duration => _duration;

    void Awake()
    {
        _group = GetComponent<CanvasGroup>();

        // Le voile ne doit jamais arrêter le rayon ni recevoir de clic.
        _group.interactable = false;
        _group.blocksRaycasts = false;

        // Noir dès la première image, pour ne pas voir la scène apparaître d'un coup.
        _group.alpha = _fadeInOnStart ? 1f : 0f;
    }

    void Start()
    {
        if (_fadeInOnStart)
            StartCoroutine(FadeIn());
    }

    // Les deux méthodes renvoient la coroutine : un autre script peut l'attendre
    // avec yield return, comme le fait le GameManager avant de recharger la scène.
    public IEnumerator FadeIn()
    {
        return Fade(1f, 0f);
    }

    public IEnumerator FadeOut()
    {
        return Fade(0f, 1f);
    }

    IEnumerator Fade(float from, float to)
    {
        // Temps réel (unscaled) : le fondu fonctionne même si un jeu met Time.timeScale à 0.
        // Au plus 1/30 s par image : l'image qui suit un chargement (souvent plusieurs
        // centaines de ms) ne doit pas faire sauter le fondu.
        for (float time = 0f; time < _duration;
             time += Mathf.Min(Time.unscaledDeltaTime, 1f / 30f))
        {
            _group.alpha = Mathf.Lerp(from, to, time / _duration);
            yield return null;
        }
        _group.alpha = to;
    }
}
