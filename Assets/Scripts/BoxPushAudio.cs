using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoxPushAudio : MonoBehaviour
{
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
    }

    public void StartPushSound()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }
    }

    public void StopPushSound()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }
}
