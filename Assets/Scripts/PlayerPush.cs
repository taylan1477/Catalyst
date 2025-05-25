using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    private bool _isPushing;
    private bool _isPulling;
    
    private PlayerMovement _playerMovement;
    private Animator _animator;
    private GameObject _boxToPush;
    private Rigidbody2D _boxRigidbody;
    private Rigidbody2D _playerRigidbody;
    private SpriteRenderer _spriteRenderer;
    private BoxPushAudio _currentPushedBoxAudio;

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerRigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
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

                    // BoxPushAudio bileşeni alınıyor
                    if (_currentPushedBoxAudio == null)
                    {
                        _currentPushedBoxAudio = _boxToPush.GetComponent<BoxPushAudio>();
                    }

                    if (moveInput * direction.x < 0)
                        PullBox(direction);
                    else
                        PushBox(direction);

                    _currentPushedBoxAudio?.StartPushSound(); // Ses başlat
                }
                else
                {
                    _boxRigidbody.linearVelocity = Vector2.zero;
                    _currentPushedBoxAudio?.StopPushSound(); // Ses durdur
                }
            }
        }
        else
        {
            _isPushing = false;
            _isPulling = false;

            if (_boxToPush != null && _boxRigidbody != null)
            {
                _boxRigidbody.linearVelocity = Vector2.zero;
                _boxRigidbody.bodyType = RigidbodyType2D.Kinematic;
            }

            _currentPushedBoxAudio?.StopPushSound(); // Shift bırakılırsa ses durdur
        }

        UpdateAnimator();
    }
    
    private void UpdateAnimator()
    {
        _animator.SetBool(AnimatorHashes.IsPushing, _isPushing);
        _animator.SetBool(AnimatorHashes.IsPulling, _isPulling);
    }
    
    private void PushBox(Vector2 direction)
    {
        _isPushing = true;
        _isPulling = false;

        float playerSpeedX = _playerRigidbody.linearVelocity.x;
        float boxSpeedX = playerSpeedX * 0.9f;

        _boxRigidbody.linearVelocity = new Vector2(boxSpeedX, 0f);
        Debug.Log("Kutu itiliyor!");
    }

    private void PullBox(Vector2 direction)
    {
        _isPulling = true;
        _isPushing = false;

        _boxRigidbody.linearVelocity = -direction * 1.8f;

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
        if (collision.CompareTag("Pushable"))
        {
            _boxToPush = collision.gameObject;
            _boxRigidbody = _boxToPush.GetComponent<Rigidbody2D>();
            _currentPushedBoxAudio = _boxToPush.GetComponent<BoxPushAudio>();

            Debug.Log("Kutu algılandı: " + _boxToPush.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Pushable"))
        {
            _currentPushedBoxAudio?.StopPushSound();
            _boxToPush = null;
            _boxRigidbody = null;
            _currentPushedBoxAudio = null;

            Debug.Log("Kutu algılanmadı.");
        }
    }
}