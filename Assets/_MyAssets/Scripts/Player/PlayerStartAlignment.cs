using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Place le joueur au point de départ, face au menu, au chargement de la scène.
/// À poser sur la racine XROrigin de la scène (le composant XROrigin est trouvé dans ses enfants).
///
/// Pourquoi : dans le casque, le « devant » de l'application est celui de la zone de jeu
/// (Guardian) ou du dernier recentrage, pas la direction où le joueur regarde au lancement.
/// L'XR Origin est bien tourné vers le menu, mais la tête peut être tournée n'importe où.
/// On tourne donc l'XR Origin autour de la tête pour que le regard tombe vers le menu,
/// puis on le déplace pour que la tête soit au-dessus du point de départ.
/// </summary>
public class PlayerStartAlignment : MonoBehaviour
{
    [Tooltip("Temps maximal d'attente du suivi de la tête, en secondes (sans casque, on abandonne sans erreur).")]
    [SerializeField] float _maxWaitSeconds = 3f;

    [Tooltip("Images à attendre après le début du suivi, le temps que la pose de la tête soit appliquée à la caméra.")]
    [SerializeField] int _settleFrames = 2;

    XROrigin _origin;
    Vector3 _startPosition;
    Vector3 _startForward;
    Coroutine _alignRoutine;

    void Awake()
    {
        _origin = GetComponentInChildren<XROrigin>();
        if (_origin == null)
        {
            Debug.LogWarning("PlayerStartAlignment : aucun XROrigin dans les enfants, rien à aligner.", this);
            return;
        }

        // La direction et la position voulues : celles de l'XR Origin tel que placé dans la scène.
        // On retire la composante verticale du forward pour regarder à l'horizontale.
        _startPosition = _origin.Origin.transform.position;
        _startForward = Vector3.ProjectOnPlane(_origin.Origin.transform.forward, Vector3.up).normalized;
    }

    void OnEnable()
    {
        // Awake et OnEnable reviennent à chaque chargement de la scène (Recommencer recharge la scène) :
        // l'alignement se refait donc à chaque fois.
        if (_origin != null) _alignRoutine = StartCoroutine(AlignWhenTracked());
    }

    void OnDisable()
    {
        if (_alignRoutine != null) StopCoroutine(_alignRoutine);
        _alignRoutine = null;
    }

    IEnumerator AlignWhenTracked()
    {
        // Attendre que la tête soit vraiment suivie : l'appareil XRNode.Head doit exister
        // et annoncer isTracked. XR Core Utils n'offre pas d'indicateur de suivi : on lit l'API XR d'Unity.
        float limit = Time.realtimeSinceStartup + _maxWaitSeconds;
        while (!IsHeadTracked())
        {
            if (Time.realtimeSinceStartup > limit)
            {
                // Dans l'éditeur sans casque : pas de suivi, la caméra est déjà au départ. Aucune erreur.
                Debug.Log("PlayerStartAlignment : pas de suivi de la tête, alignement non nécessaire.");
                _alignRoutine = null;
                yield break;
            }
            yield return null;
        }

        // Quelques images de plus : la pose de la tête doit être appliquée à la caméra (Tracked Pose Driver).
        for (int i = 0; i < _settleFrames; i++) yield return null;

        AlignToStart();
        Debug.Log($"PlayerStartAlignment : aligné {Time.timeSinceLevelLoad:0.00} s après le chargement.");
        _alignRoutine = null;
    }

    static bool IsHeadTracked()
    {
        var head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        return head.isValid && head.TryGetFeatureValue(CommonUsages.isTracked, out bool tracked) && tracked;
    }

    /// <summary>
    /// Tourne l'XR Origin pour que le regard tombe dans la direction de départ, puis le déplace
    /// pour que la tête soit au-dessus du point de départ. Utilisé aussi par FallSafety après une chute.
    /// </summary>
    public bool AlignToStart()
    {
        if (_origin == null || _origin.Camera == null) return false;

        // 1. Rotation autour de la tête : le forward de la caméra (à plat) devient la direction de départ.
        _origin.MatchOriginUpCameraForward(Vector3.up, _startForward);

        // 2. Translation : la tête au-dessus du point de départ, à la hauteur réelle du joueur.
        var target = _startPosition + Vector3.up * _origin.CameraInOriginSpaceHeight;
        _origin.MoveCameraToWorldLocation(target);

        // Le CharacterController doit connaître la nouvelle position tout de suite, sinon
        // son prochain Move part de l'ancienne (Auto Sync Transforms est décoché par défaut).
        Physics.SyncTransforms();
        return true;
    }
}
