using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    private float lifetime = 3f;
    [SerializeField] private GameObject collisionEffect;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Environment"))
        {
            if (collisionEffect != null)
            {
                Instantiate(collisionEffect, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }
}