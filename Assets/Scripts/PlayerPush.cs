using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float pushForce = 3f; // İtme/çekme kuvveti

    private PlayerMovement _playerMovement;
    private GameObject _boxToPush;
    private Rigidbody2D _boxRigidbody;
    private Rigidbody2D _playerRigidbody;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerRigidbody = GetComponent<Rigidbody2D>(); // Karakterin Rigidbody2D'sini al
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _playerMovement.isSlowed = true;

            if (_boxToPush != null && _boxRigidbody != null)
            {
                _boxRigidbody.bodyType = RigidbodyType2D.Dynamic;

                float moveInput = Input.GetAxis("Horizontal");
                Vector2 direction = (_boxToPush.transform.position - transform.position).normalized;

                if (moveInput * direction.x < 0)
                    PullBox(direction);
                else
                    PushBox(direction);
            }
        }
        else
        {
            _playerMovement.isSlowed = false;
            _playerMovement.isPushing = false;
            _playerMovement.isPulling = false;

            if (_boxToPush != null && _boxRigidbody != null)
            {
                _boxRigidbody.linearVelocity = Vector2.zero;
                _boxRigidbody.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void PushBox(Vector2 direction)
    {
        _playerMovement.isPushing = true;
        _playerMovement.isPulling = false;

        _boxRigidbody.linearVelocity = direction * pushForce;

        Debug.Log("Kutu itiliyor!");
    }

    private void PullBox(Vector2 direction)
    {
        _playerMovement.isPulling = true;
        _playerMovement.isPushing = false;

        _boxRigidbody.linearVelocity = -direction * pushForce;

        Debug.Log("Kutu çekiliyor!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer çarpışan nesne "Pushable" tag'ine sahipse
        if (collision.CompareTag("Pushable"))
        {
            _boxToPush = collision.gameObject;
            _boxRigidbody = _boxToPush.GetComponent<Rigidbody2D>();
            Debug.Log("Kutu algılandı: " + _boxToPush.name); // Kutunun algılandığını kontrol et
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Eğer çarpışan nesne "Pushable" tag'ine sahipse
        if (collision.CompareTag("Pushable"))
        {
            Debug.Log("Kutu algılanmadı."); // Kutunun algılanmadığını kontrol et
            _boxToPush = null; // Kutuyu sıfırla
            _boxRigidbody = null; // Rigidbody2D'yi sıfırla
        }
    }
}