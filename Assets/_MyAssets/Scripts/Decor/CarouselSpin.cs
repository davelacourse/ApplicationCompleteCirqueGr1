using UnityEngine;

/// <summary>
/// Fait tourner le carrousel lentement autour de son axe vertical.
/// Décor du projet de départ : ne fait pas partie du travail du cours.
/// </summary>
public class CarouselSpin : MonoBehaviour
{
    [Tooltip("Vitesse de rotation, en degrés par seconde.")]
    [SerializeField] float _speed = 12f;

    void Update()
    {
        transform.Rotate(0f, _speed * Time.deltaTime, 0f, Space.Self);
    }
}
