using UnityEngine;

public class FemaleCat : MonoBehaviour
{
    private bool _mouseNearby; // Fare yakın mı?
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }
    
    public void OnPlayerApproach()
    {
        _animator.SetBool("isInterested", true);
    }

    public void OnPlayerLeave()
    {
        _animator.SetBool("isInterested", false);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mouse")) // Sadece fareleri algıla
        {
            _mouseNearby = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Mouse"))
        {
            _mouseNearby = false;
        }
    }

    public bool IsMouseNearby()
    {
        return _mouseNearby;
    }
}