using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager için gerekli

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(2, 2, -10);
    public float smoothSpeed = 0.125f;
    
    public float minX = 5.36f;
    public float maxX = 300f;
    public float minY = 0.79f;
    public float maxY = 2.92f;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        // ESC tuş kontrolü (doğrudan LateUpdate'e ekledim)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu"); // Sahne adını kontrol et!
        }
    }
}