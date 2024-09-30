using System.Collections.Generic;
using UnityEngine;

public class Movement : PathFollower
{
    public Camera mainCamera;
    public float speed = 1f;
    public float verticalSpeed = 50f;
    public List<Rigidbody> ragdollParts;
    public Rigidbody headPart;
    public float flailForce = 5f;

    private Vector3[] originalPositions;
    private float yPosition = 0.2f;
    private Quaternion targetRotation;
    private bool isShiftHeld = false;

    protected override void Start()
    {
        base.Start(); // Call the base class's Start method
        originalPositions = new Vector3[ragdollParts.Count];
        for (int i = 0; i < ragdollParts.Count; i++)
        {
            originalPositions[i] = ragdollParts[i].transform.position;
        }
    }

    void Update()
    {
        // WASD input for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Adjust direction based on input
        if (moveHorizontal != 0f && !isShiftHeld)
        {
            isRight = moveHorizontal > 0f; // Only change direction when there's movement
        }

        // Adjust movement speed and update circular position
        angularSpeed = moveHorizontal * speed;
        
        // Update the circular position
        UpdatePlayerPosition(); // Update position based on direction (right or left)

        // Adjust vertical movement and clamp the position
        yPosition = Mathf.Clamp(yPosition + moveVertical * verticalSpeed * Time.deltaTime, 0f, 40f);
        transform.position = new Vector3(transform.position.x, yPosition, transform.position.z);

        // Rotate based on the camera
        RotateCharacter(moveHorizontal);

        // Flail the ragdoll parts
        FlailRagdoll(moveHorizontal, moveVertical);
    }

    private void RotateCharacter(float moveHorizontal)
    {
        isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 cameraRight = mainCamera.transform.right;
        cameraRight.y = 0f;
        cameraRight.Normalize();

        targetRotation = isRight ? Quaternion.LookRotation(cameraRight) : Quaternion.LookRotation(-cameraRight);

        // Apply a 20 degree forward tilt when moving forward or -10 degree tilt if moving backward and shift is held
        if (moveHorizontal != 0f)
        {
            float tiltAngle = 15f;

            if (isShiftHeld && ((isRight && moveHorizontal < 0f) || (!isRight && moveHorizontal > 0f)))
            {
                tiltAngle = -10f;
            }

            targetRotation *= Quaternion.Euler(tiltAngle, 0f, 0f);
        }
    
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 7.5f);
    }

    private void FlailRagdoll(float moveHorizontal, float moveVertical)
    {
        Vector3 flailDirection = new Vector3(-Mathf.Sin(angle), moveVertical, Mathf.Cos(angle)) * -moveHorizontal;

        foreach (Rigidbody part in ragdollParts)
        {
            if (moveHorizontal != 0f || moveVertical != 0f)
            {
                part.AddForce(flailDirection * flailForce);
            }
            else
            {
                int index = ragdollParts.IndexOf(part);
                float interpolationSpeed = Time.deltaTime * (5f + part.mass * 2f);
                part.MovePosition(Vector3.Lerp(part.position, originalPositions[index], interpolationSpeed));
            }
        }

        headPart.AddForce(-flailDirection * flailForce);
    }
}
