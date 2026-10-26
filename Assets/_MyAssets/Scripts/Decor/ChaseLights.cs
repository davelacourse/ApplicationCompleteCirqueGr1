using UnityEngine;

/// <summary>
/// Ampoules en chenillard : une ampoule sur trois est allumée, et le motif avance
/// à intervalle régulier. Décor du projet de départ : ne fait pas partie du travail du cours.
/// </summary>
public class ChaseLights : MonoBehaviour
{
    [Tooltip("Les ampoules, dans l'ordre du chenillard.")]
    [SerializeField] Renderer[] _bulbs;

    [SerializeField] Material _onMaterial;
    [SerializeField] Material _offMaterial;

    [Tooltip("Une ampoule sur combien est allumée.")]
    [SerializeField] int _spacing = 3;

    [Tooltip("Temps entre deux pas du chenillard, en secondes.")]
    [SerializeField] float _interval = 0.25f;

    int _step;
    float _nextStepTime;

    void Update()
    {
        if (Time.time < _nextStepTime)
            return;

        _nextStepTime = Time.time + _interval;
        _step = (_step + 1) % _spacing;

        // sharedMaterial : on change de matériau sans en créer une copie par ampoule.
        for (int i = 0; i < _bulbs.Length; i++)
            _bulbs[i].sharedMaterial = (i + _step) % _spacing == 0 ? _onMaterial : _offMaterial;
    }
}
