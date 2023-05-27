using System.Data;
using UnityEngine;

public class Asteroid : MonoBehaviour, IDamageable
{
    [SerializeField] private FracturedAsteroid _fracturedAsteroidPrefab;
    [SerializeField] private Detonator _explosionPrefab;

    private Transform _transform;
    private readonly float _minRelativeVelocityMagnitude = 50f; // Minimum velocity magnitude required to apply damage
    private readonly int _damageFactor = 5; // Damage factor to apply to the damage taken by the asteroid
    private void Awake()
    {
        _transform = transform;
    }

    public void TakeDamage(int damage, Vector3 hitPosition)
    {
        FractureAsteroid(hitPosition);
    }

    private void FractureAsteroid(Vector3 hitPosition)
    {
        if (_fracturedAsteroidPrefab != null)
        {
            Instantiate(_fracturedAsteroidPrefab, _transform.position, _transform.rotation);
        }

        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, transform.position/*hitPosition*/, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        float relativeVelocityMagnitude = collision.relativeVelocity.magnitude;

        // if collision is with a rigidbody, check if it has a velocity greater than 20f
        if (collision.rigidbody != null &&  relativeVelocityMagnitude > _minRelativeVelocityMagnitude)
        {
            Vector3 hitPosition = collision.GetContact(0).point;
            TakeDamage(1000, hitPosition);

            IDamageable damageable = collision.collider.gameObject.GetComponent<IDamageable>();            
            damageable?.TakeDamage(_damageFactor * Mathf.RoundToInt(relativeVelocityMagnitude), hitPosition);
        }


    }
}