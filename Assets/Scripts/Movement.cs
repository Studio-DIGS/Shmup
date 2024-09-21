using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // GameObject References
    public Camera mainCamera; // Camera Reference
    
    // Player movement stats
    public float speed = 1f;
    public float radius = 120f;
    public float verticalSpeed = 50f;
    private Vector3[] originalPositions; 
    
    // List of Rigidbodies for the ragdoll parts IK, plus force variable
    public List<Rigidbody> ragdollParts;
    public Rigidbody headPart;
    public float flailForce = 5f; 

    // Changing variables for player movement
    public float angle = 0f; 
    private float yPosition = 0.2f; 
    private Quaternion targetRotation; 
    public bool isRight = true;
    private bool isShiftHeld = false;

    // Start function: sets original positions to ragdoll initial positions
    void Start()
    {
        originalPositions = new Vector3[ragdollParts.Count];

        for (int i = 0; i < ragdollParts.Count; i++)
        {
            originalPositions[i] = ragdollParts[i].transform.position;
        }
    }

    // Update function: handles all of the movement
    void Update()
    {
        // Uses GetAxis for WASD Controls
        float moveHorizontal = Input.GetAxis("Horizontal"); // A and D
        float moveVertical = Input.GetAxis("Vertical"); // W and S

        // Update the angle based on horizontal input and speed
        angle += moveHorizontal * speed * Time.deltaTime;

        // Update the y position based on vertical input and speed, clamping between 0f and 40f
        yPosition = Mathf.Clamp(yPosition + moveVertical * verticalSpeed * Time.deltaTime, 0f, 40f);

        // Calculate the new position on the circle
        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        // Set the new position
        transform.position = new Vector3(x, yPosition, z);

        // Determine if Shift is held down
        isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Determine the direction the character should face based on the camera that faces player
        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        // If shift is not held, update the facing direction based on input
        if (!isShiftHeld)
        {
            if (moveHorizontal > 0f)
            {
                isRight = true;
            }
            else if (moveHorizontal < 0f)
            {
                isRight = false;
            }
        }

        // Set target rotation based on whether the player is facing right or left
        targetRotation = isRight ? Quaternion.LookRotation(cameraRight) : Quaternion.LookRotation(-cameraRight);

        // Apply a 20 degree forward tilt when moving forward or -10 degree tilt if moving backwards and shift is held
        if (moveHorizontal != 0f || moveVertical != 0f)
        {
            float tiltAngle = 20f;

            if (isShiftHeld && ((isRight && moveHorizontal < 0f) || (!isRight && moveHorizontal > 0f)))
            {
                tiltAngle = -10f;
            }

            targetRotation *= Quaternion.Euler(tiltAngle, 0f, 0f);
        }

        // Smoothly rotate the character to the target rotation, can update speed with the f value
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 7.5f);

        // Apply flailing forces to ragdoll parts
        Vector3 flailDirection = new Vector3(-Mathf.Sin(angle), moveVertical, Mathf.Cos(angle)) * -moveHorizontal;
        foreach (Rigidbody part in ragdollParts)
        {
            if (moveHorizontal != 0f || moveVertical != 0f)
            {
                if (part == ragdollParts[6] || part == ragdollParts[7] || part == ragdollParts[8] || part == ragdollParts[9])
                {
                    part.AddForce(-flailDirection * flailForce);
                }
                else
                {
                    part.AddForce(flailDirection * flailForce);
                }
            }
            else
            {
                int index = ragdollParts.IndexOf(part);
                float interpolationSpeed = Time.deltaTime * (5f + part.mass * 2f); // Increase speed based on mass
                part.MovePosition(Vector3.Lerp(part.position, originalPositions[index], interpolationSpeed));
            }
        }

        // Make head move in opposite directoin, aka forward when moving forward
        headPart.AddForce(-flailDirection * flailForce);
    }
}
