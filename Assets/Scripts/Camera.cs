using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Player Transform Reference
    public float cameraRadius = 20f; // Radius of path for camera (relative to player)
    public float heightOffset = 5f; // Height offset for the camera

    void LateUpdate()
    {
        // Calculate the cameras position relative to the players circular movement
        float angle = player.GetComponent<Movement>().angle;
        float x = Mathf.Cos(angle) * cameraRadius;
        float z = Mathf.Sin(angle) * cameraRadius;
        Vector3 offset = new Vector3(x, heightOffset, z);

        // Set the cameras position relative to player, and make it look at player
        transform.position = player.position + offset;
        transform.LookAt(player.position);
    }
}
