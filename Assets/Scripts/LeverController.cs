using UnityEngine;

public class LeverController : MonoBehaviour
{
    public Sprite leverOn;
    public Sprite leverOff;
    public AudioClip toggleSound;
    public GameObject[] connectedPlatforms; // Bağlı platformlar

    private bool _isOn = true;
    private SpriteRenderer _sr;
    private AudioSource _audioSource;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _sr.sprite = _isOn ? leverOn : leverOff;
    }
    
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void ToggleLever()
    {
        _isOn = !_isOn;
        _sr.sprite = _isOn ? leverOn : leverOff;
        
        if (toggleSound != null) _audioSource.PlayOneShot(toggleSound);

        foreach (GameObject platform in connectedPlatforms)
        {
            platform.GetComponent<RotatingPlatform>()?.ToggleRotation();
        }
    }
}