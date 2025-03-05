using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float pushForce = 5f; // İtme/çekme kuvveti

    private PlayerMovement _playerMovement;
    private GameObject _boxToPush;
    private Rigidbody2D _boxRigidbody;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Shift tuşuna basılı olduğunda
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            _playerMovement.isSlowed = true; // Karakteri yavaşlat

            // Eğer kutu varsa ve Shift tuşuna basılıysa
            if (_boxToPush != null)
            {
                // Kutunun Rigidbody2D'sini Dynamic yap
                if (_boxRigidbody != null)
                {
                    _boxRigidbody.bodyType = RigidbodyType2D.Dynamic;
                }

                PushBox(); // Kutuyu it
            }
        }
        else
        {
            _playerMovement.isSlowed = false; // Hızı normale döndür

            // Eğer kutu varsa ve Shift tuşu bırakıldıysa
            if (_boxToPush != null)
            {
                // Kutunun Rigidbody2D'sini Kinematic yap
                if (_boxRigidbody != null)
                {
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

    private void PushBox()
    {
        // Shift tuşuna basılı olduğundan emin ol
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log("Kutu itiliyor!"); // Fonksiyonun çağrıldığını kontrol et
            Vector2 pushDirection = (_boxToPush.transform.position - transform.position).normalized;
            Debug.DrawRay(transform.position, pushDirection * 2f, Color.red, 1f); // Yönü görselleştir

            // Kutunun Rigidbody2D'sini al ve itme kuvvetini uygula
            if (_boxRigidbody != null)
            {
                _boxRigidbody.linearVelocity = pushDirection * pushForce; // Kutuyu it
            }
        }
    }
}