using UnityEngine;

/// <summary>
/// Fait flotter un objet : il monte, descend et se balance doucement, comme un ballon
/// au bout de sa ficelle. Décor du projet de départ : ne fait pas partie du travail du cours.
/// </summary>
public class Bobbing : MonoBehaviour
{
    [Tooltip("Amplitude du mouvement vertical, en mètres.")]
    [SerializeField] float _height = 0.08f;

    [Tooltip("Vitesse du mouvement.")]
    [SerializeField] float _speed = 0.8f;

    [Tooltip("Balancement maximal, en degrés.")]
    [SerializeField] float _sway = 6f;

    Vector3 _startPosition;
    Quaternion _startRotation;
    float _offset;

    void Awake()
    {
        _startPosition = transform.localPosition;
        _startRotation = transform.localRotation;

        // Un décalage au hasard : les ballons ne bougent pas tous en même temps.
        _offset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float t = (Time.time + _offset) * _speed;
        transform.localPosition = _startPosition + Vector3.up * (Mathf.Sin(t) * _height);
        transform.localRotation = _startRotation * Quaternion.Euler(Mathf.Sin(t * 0.7f) * _sway, 0f, Mathf.Cos(t * 0.6f) * _sway);
    }
}
