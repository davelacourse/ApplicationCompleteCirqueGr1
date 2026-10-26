using System.Collections;
using System.Reflection;
using Unity.XR.CoreUtils;
using UnityEngine;

/// <summary>
/// Filet de sécurité : si la tête du joueur descend trop bas (il est tombé de la plateforme),
/// fondu au noir, retour au point de départ, puis fondu d'ouverture.
/// À poser sur la racine XROrigin de la scène, avec PlayerStartAlignment.
///
/// Pourquoi, malgré la bordure invisible : dans un casque, on peut toujours marcher
/// physiquement à travers un mur virtuel. La bordure bloque le joystick, pas les pieds.
///
/// Le ScreenFader est appelé par réflexion (par le nom de son type et de ses méthodes) :
/// ce script compile ainsi dans tous les projets du cirque, même ceux qui n'ont pas encore
/// de ScreenFader (le template du cours 9). Sans ScreenFader, le retour se fait sans fondu.
/// </summary>
[RequireComponent(typeof(PlayerStartAlignment))]
public class FallSafety : MonoBehaviour
{
    [Tooltip("Distance sous le sol du départ, en mètres, à partir de laquelle la tête est considérée comme tombée.")]
    [SerializeField] float _fallDistance = 2f;

    XROrigin _origin;
    PlayerStartAlignment _alignment;
    MonoBehaviour _fader;
    MethodInfo _fadeOut;
    MethodInfo _fadeIn;
    Transform _head;
    float _limitY;
    bool _recovering;

    void Awake()
    {
        // Tout se trouve par code : aucun champ à remplir dans l'inspecteur.
        _origin = GetComponentInChildren<XROrigin>();
        _alignment = GetComponent<PlayerStartAlignment>();
        _fader = FindScreenFader(transform);
        if (_fader != null)
        {
            _fadeOut = _fader.GetType().GetMethod("FadeOut");
            _fadeIn = _fader.GetType().GetMethod("FadeIn");
        }
        if (_origin != null && _origin.Camera != null)
        {
            _head = _origin.Camera.transform;
            // Le sol du départ est la hauteur de l'XR Origin tel que placé dans la scène.
            _limitY = _origin.Origin.transform.position.y - _fallDistance;
        }
        else
        {
            Debug.LogWarning("FallSafety : aucun XROrigin avec une caméra dans les enfants, filet désactivé.", this);
            enabled = false;
        }
    }

    /// <summary>
    /// Le ScreenFader de l'XR Origin (sur la Main Camera), trouvé par le nom de son type dans les enfants.
    /// Retourne null si le projet n'en a pas.
    /// </summary>
    public static MonoBehaviour FindScreenFader(Transform root)
    {
        foreach (var component in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (component != null && component.GetType().Name == "ScreenFader") return component;
        }
        return null;
    }

    void OnDisable()
    {
        // Si l'objet est désactivé en pleine récupération, la coroutine s'arrête : on permet une prochaine chute.
        _recovering = false;
    }

    void Update()
    {
        // Une seule récupération à la fois : tant qu'elle dure, on ne surveille plus.
        if (_recovering || _head.position.y > _limitY) return;
        StartCoroutine(Recover());
    }

    IEnumerator Recover()
    {
        _recovering = true;

        // 1. Fondu au noir (le joueur continue de tomber pendant ce temps, mais ne le voit plus).
        if (_fadeOut != null) yield return (IEnumerator)_fadeOut.Invoke(_fader, null);

        // 2. Retour au départ, face au menu.
        _alignment.AlignToStart();

        // 3. Une image pour que le corps (CharacterController) se pose sur le sol avant d'ouvrir.
        yield return null;

        // 4. Fondu d'ouverture.
        if (_fadeIn != null) yield return (IEnumerator)_fadeIn.Invoke(_fader, null);

        _recovering = false;
    }
}
