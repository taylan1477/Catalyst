using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player; // Karakterin transform bileşeni
    public Vector3 offset = new Vector3(2, 2, -10); // Kamera ile karakter arasındaki mesafe
    public float smoothSpeed = 0.125f; // Kamera hareketinin yumuşaklığı

    // Kamera sınırları
    public float minX = 5.36f; // X min
    public float maxX = 300f;   // X maks
    public float minY = 0.79f; // Y min
    public float maxY = 2.92f; // Y maks

    void LateUpdate()
    {
        if (player != null)
        {
            // Kameranın gitmesi gereken pozisyonu hesapla
            Vector3 desiredPosition = player.position + offset;

            // X ve Y sınırlarını uygula
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

            // Yumuşak geçiş ile kamerayı güncelle
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}