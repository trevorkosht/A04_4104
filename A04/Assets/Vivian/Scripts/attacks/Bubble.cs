using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float lifeTime;
    [SerializeField] int damage;   // how much damage to deal to the player

    private void Start()
    {
        Destroy(gameObject, lifeTime); // destroy after X seconds

    }

    private void Update()
    {
        //Debug.Log("Projectile moving: " + transform.position);
        transform.position += (transform.forward + (Vector3.up * 0.17f)) * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);    // Destroy on impact.
        }
        else if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }

    }
}
