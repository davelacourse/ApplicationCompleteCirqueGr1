using UnityEngine;

/// <summary>
/// Fait démarrer une boucle à un moment pris au hasard. Deux sources qui jouent le même son
/// ne sont alors pas synchronisées : on entend deux groupes d'oiseaux, et non un écho.
/// Décor du projet de départ : ne fait pas partie du travail du cours.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RandomStartTime : MonoBehaviour
{
    AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    // Start : la source a déjà commencé à jouer (Play On Awake), on la place ailleurs dans la boucle.
    void Start()
    {
        if (_source.clip != null)
            _source.timeSamples = Random.Range(0, _source.clip.samples);
    }
}
