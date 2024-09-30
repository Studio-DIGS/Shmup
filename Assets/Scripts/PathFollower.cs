using UnityEngine;

public class PathFollower : MonoBehaviour
{
    // Common properties for circular movement
    public float radius = 120f;
    public float angularSpeed = 1f;
    public bool isRight = true;

    // Current angle on the circular path (in radians)
    public float angle = 0f;
    
    // Store initial position to maintain y-axis or any other properties
    protected Vector3 initialPosition;

    // Start: Set up initial values like angle and position
    protected virtual void Start()
    {
        initialPosition = transform.position;
        // angle = Mathf.Atan2(initialPosition.z, initialPosition.x); // Calculate initial angle based on position
    }

    // Function to update position based on angle and direction (clockwise or counterclockwise)
    protected void UpdatePlayerPosition()
    {
        // Update the angle based on direction (clockwise or counterclockwise)
        angle += angularSpeed * Time.deltaTime;

        // Calculate the new position on the circle
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Set new position while keeping y-axis constant
        transform.position = new Vector3(x, initialPosition.y, z);
    }

    protected void UpdateProjPosition()
    {
        // Update the angle based on direction (clockwise or counterclockwise)
        if (isRight)
        {
            angle += angularSpeed * Time.deltaTime;
        }
        else 
        {
            angle -= angularSpeed * Time.deltaTime;
        }

        // Calculate the new position on the circle
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Set new position while keeping y-axis constant
        transform.position = new Vector3(x, initialPosition.y, z);
    }
}
