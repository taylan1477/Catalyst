using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float pushForce = 3f; // İtme/çekme kuvveti

    private PlayerMovement _playerMovement;
    private GameObject _boxToPush;
    private Rigidbody2D _boxRigidbody;
    private Rigidbody2D _playerRigidbody;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerRigidbody = GetComponent<Rigidbody2D>(); // Karakterin Rigidbody2D'sini al
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // HER ZAMAN önce shift'e basılı mı kontrol et
        _playerMovement.isSlowed = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_boxToPush != null && _boxRigidbody != null)
            {
                _boxRigidbody.bodyType = RigidbodyType2D.Dynamic;

                float moveInput = Input.GetAxis("Horizontal");

                if (Mathf.Abs(moveInput) > 0.01f)
                {
                    Vector2 direction = (_boxToPush.transform.position - transform.position).normalized;

                    if (moveInput * direction.x < 0)
                        PullBox(direction);
                    else
                        PushBox(direction);
                }
                else
                {
                    _boxRigidbody.linearVelocity = Vector2.zero;
                }
            }
        }
        else
        {
            // Shift bırakıldığında pushing/pulling durumu sıfırlanmalı
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

        float playerSpeedX = _playerRigidbody.linearVelocity.x;

        // Hızı birebir kopyala ya da hafifçe artır
        float boxSpeedX = playerSpeedX * 0.9f;

        _boxRigidbody.linearVelocity = new Vector2(boxSpeedX, 0f);

        Debug.Log("Kutu itiliyor!");
    }

    private void PullBox(Vector2 direction)
    {
        _playerMovement.isPulling = true;
        _playerMovement.isPushing = false;

        _boxRigidbody.linearVelocity = -direction * 1.8f;

        // Sprite'ı kutunun olduğu yöne döndür
        if (_boxToPush != null)
        {
            float boxPosX = _boxToPush.transform.position.x;
            float playerPosX = transform.position.x;

            _spriteRenderer.flipX = boxPosX < playerPosX;
        }

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
            Debug.Log("Center of mass: " + _boxRigidbody.centerOfMass);
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