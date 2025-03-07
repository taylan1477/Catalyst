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

    void Update()
    {
        // Shift tuşuna basılı olduğunda
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _playerMovement.isSlowed = true; // Karakteri yavaşlat
            _playerMovement.isPulling = true; // Çekme durumu

            // Eğer kutu varsa ve Shift tuşuna basılıysa
            if (_boxToPush != null)
            {
                // Kutunun Rigidbody2D'sini Dynamic yap
                if (_boxRigidbody != null)
                {
                    _boxRigidbody.bodyType = RigidbodyType2D.Dynamic;
                }

                PushOrPullBox(); // Kutuyu it veya çek
            }
        }
        else
        {
            _playerMovement.isSlowed = false; // Hızı normale döndür
            _playerMovement.isPulling = false; // Çekme durumunu bitir

            // Eğer kutu varsa ve Shift tuşu bırakıldıysa
            if (_boxToPush != null)
            {
                // Kutunun Rigidbody2D'sini Kinematic yap ve hızını sıfırla
                if (_boxRigidbody != null)
                {
                    _boxRigidbody.linearVelocity = Vector2.zero; // Hızı sıfırla
                    _boxRigidbody.bodyType = RigidbodyType2D.Kinematic;
                }
            }
        }
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

    private void PushOrPullBox()
    {
        // Shift tuşuna basılı olduğundan emin ol
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("Kutu itiliyor veya çekiliyor!"); // Fonksiyonun çağrıldığını kontrol et

            // Karakterin hareket yönünü al
            float moveInput = Input.GetAxis("Horizontal");

            // Kutunun yönünü hesapla
            Vector2 direction = (_boxToPush.transform.position - transform.position).normalized;

            // Kutunun hızını al
            Vector2 boxVelocity = _boxRigidbody.linearVelocity;

            // Karakterin hızını kutunun hızına eşitle
            _playerRigidbody.linearVelocity = boxVelocity;

            // Eğer karakter kutudan uzaklaşıyorsa (çekme)
            if (moveInput * direction.x < 0)
            {
                Debug.Log("Kutu çekiliyor!");
                _boxRigidbody.linearVelocity = -direction * pushForce; // Kutuyu kendine doğru çek
            }
            // Eğer karakter kutuyu itiyorsa
            else
            {
                Debug.Log("Kutu itiliyor!");
                _boxRigidbody.linearVelocity = direction * pushForce; // Kutuyu it
            }

            Debug.DrawRay(transform.position, direction * 2f, Color.red, 1f); // Yönü görselleştir
        }
    }
}