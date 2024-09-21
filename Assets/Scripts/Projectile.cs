using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Projectile stats
    private Vector3 initialPosition;
    public float lifeTime = 1f;
    public float radius = 120f;
    public float angularSpeed = .7f;
    public bool isRight;
    // public int damage = 10; // Damage will be incorporated later

    // Changing variables
    private float angle;
    
    // Start function: Sets initial position and calculates the angle based on said initial position, also calls destroy on projectile for its lifetime
    void Start()
    {
        initialPosition = transform.position;
        angle = Mathf.Atan2(initialPosition.z, initialPosition.x);

        Destroy(gameObject, lifeTime);
    }

    // Update function: 
    void Update()
    {
        // Updates the angle based on left or right movement
        if(isRight)
        {
            angle += angularSpeed * Time.deltaTime;
        }
        else
        {
            angle -= angularSpeed * Time.deltaTime;
        }

        // Calculate the new position on the circle (x and z axes only)
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Sets new position while keeping the initial y position
        transform.position = new Vector3(x, initialPosition.y, z);
    }

    // Destroy on entering a collider (thats not the player)
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
