using System.Collections;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    private bool _isVertical;
    private Quaternion _originalRotation;
    public float rotationDuration = 0.7f;
    public AudioClip rotateSound;
    private AudioSource _audioSource;

    void Start()
    {
        _originalRotation = transform.rotation;
    }
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void ToggleRotation()
    {
        float newZRotation = _isVertical ? 15f : 90f;
        StartCoroutine(RotateToAngle(newZRotation));
        _isVertical = !_isVertical;
    }
    
    IEnumerator RotateToAngle(float newZRotation)
    {
        if (rotateSound != null) _audioSource.PlayOneShot(rotateSound);
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(
            _originalRotation.eulerAngles.x,
            _originalRotation.eulerAngles.y,
            newZRotation
        );
        float elapsed = 0f;

        while (elapsed < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / rotationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
    }
}