using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    private bool _isVertical;
    private Quaternion _originalRotation; // Orijinal rotasyonu sakla

    void Start()
    {
        _originalRotation = transform.rotation; // Başlangıç rotasyonunu kaydet
    }

    public void ToggleRotation()
    {
        _isVertical = !_isVertical;

        // Sadece Z eksenini değiştir, X ve Y'yi orijinal değerlerinde tut
        float newZRotation = _isVertical ? 15f : 90f;
        
        transform.rotation = Quaternion.Euler(
            _originalRotation.eulerAngles.x,
            _originalRotation.eulerAngles.y,
            newZRotation
        );
    }
}