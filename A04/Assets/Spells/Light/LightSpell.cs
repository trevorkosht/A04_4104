using UnityEngine;

public class LightSpell : MonoBehaviour
{
    public float speed = 2f;
    private float lifetime = 3f;
    [SerializeField] private GameObject collisionEffect;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Environment"))
        {
            if (collisionEffect != null)
            {
                GameObject spawnedEffect = Instantiate(collisionEffect, transform.position, transform.rotation);
                Destroy(spawnedEffect, 3f);
            }

            Destroy(gameObject);
        }
    }
}
