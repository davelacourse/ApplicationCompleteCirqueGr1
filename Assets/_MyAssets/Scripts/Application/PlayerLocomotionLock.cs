using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

/// <summary>
/// Retient le joueur sur place tant que la partie n'est pas en cours (menu de départ, pause).
/// Il désactive les composants qui le déplacent, puis les réactive tels quels :
/// leurs réglages sont conservés. La rotation n'est pas touchée : le joueur peut
/// toujours regarder autour de lui. À poser sur la racine de l'XR Origin.
/// </summary>
public class PlayerLocomotionLock : MonoBehaviour
{
    // Ce qui déplace le joueur. Pas les Turn Providers : il peut toujours tourner.
    Behaviour[] _locomotion;

    void Awake()
    {
        // Chacun est unique dans l'XR Origin : on le trouve par son type, sans rien glisser.
        // true : inclure les objets inactifs, comme le rayon de téléportation au repos.
        _locomotion = new Behaviour[]
        {
            GetComponentInChildren<ContinuousMoveProvider>(true),
            GetComponentInChildren<TeleportationProvider>(true),
            GetComponentInChildren<XRRayInteractor>(true)
        };
    }

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

    // L'état de départ est lu ici : l'événement n'annonce que les changements.
    void Start()
    {
        OnStateChanged(GameManager.Instance.State);
    }

    void OnStateChanged(GameState state)
    {
        bool canMove = state == GameState.Playing;
        foreach (Behaviour component in _locomotion)
            component.enabled = canMove;
    }
}
