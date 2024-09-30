using UnityEngine;

public class Projectile : PathFollower
{
    public float lifeTime = 1f;

    protected override void Start()
    {
        base.Start(); // Call base class's Start method

        Vector3 direction = transform.position - Vector3.zero;
        angle = Mathf.Atan2(direction.z, direction.x);

        Destroy(gameObject, lifeTime); // Destroy the projectile after its lifetime
    }

    void Update()
    {
        UpdateProjPosition(); // Move along the circular path
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroy on collision
        }
    }
}
