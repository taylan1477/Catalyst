using UnityEngine;

public class PlayerClimbUp : MonoBehaviour
{
    public Transform climbCheck;               // Duvar tepe kontrolü için boş obje
    public float climbCheckDistance = 0.3f;    // Duvar kontrol mesafesi
    public LayerMask climbableLayer;           // Tırmanılabilir layer
    public float climbDuration = 0.8f;         // Tırmanma animasyon süresi
    public Vector2 climbOffset = new (0f, 1.2f); // Yukarı taşınma mesafesi

    public Transform climbCheckAnchor;         // Karakterin sabit referans noktası
    public Vector2 climbCheckLocalOffset = new (0.5f, 1.2f); // Yön bağımlı offset

    private Animator _animator;
    private Rigidbody2D _rb;
    private bool _isClimbing;
    private bool _canClimb;
    private PlayerWallJump _wallJump; // WallJump sınıfını burada kullanacağız

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _wallJump = GetComponent<PlayerWallJump>(); // WallJump bileşenini alıyoruz
    }

    void Update()
    {
        if (_isClimbing) { return; }

        CheckClimbableWall();

        // Eğer tırmanmak için uygun duvar varsa ve space tuşuna basıldıysa
        if (_canClimb && Input.GetKeyDown(KeyCode.Space) && _wallJump.IsTouchingWall())
        {
            StartCoroutine(Climb());
        }
    }

    void LateUpdate()
    {
        if (climbCheck == null || climbCheckAnchor == null) return;

        float direction = GetFacingDirection();
        Vector3 offset = new Vector3(climbCheckLocalOffset.x * direction, climbCheckLocalOffset.y, 0);
        climbCheck.position = climbCheckAnchor.position + offset;
    }

    void CheckClimbableWall()
    {
        // Yönü belirle (sağa ya da sola)
        Vector2 direction = Vector2.right * GetFacingDirection();
        RaycastHit2D hit = Physics2D.Raycast(climbCheck.position, direction, climbCheckDistance, climbableLayer);
        
        // Duvar var mı kontrol et
        _canClimb = hit.collider != null;

        if (_canClimb)
        {
            Debug.Log("Duvar bulundu, tırmanılabilir.");
            Debug.Log(_wallJump.IsTouchingWall());

            // Eğer duvar varsa, duvarın sonuna yaklaşıp yaklaşmadığını kontrol et
            RaycastHit2D topHit = Physics2D.Raycast(climbCheck.position, Vector2.up, climbCheckDistance, climbableLayer);
            if (topHit.collider != null)
            {
                Debug.Log("Duvarın üst kısmına yaklaşılıyor.");
                if (topHit.distance < 0.2f) // Duvarın üst kısmına yaklaşıyorsa
                {
                    Debug.Log("Duvarın son kısmına ulaşıldı, tırmanma animasyonu başlatılacak.");
                    StartClimbingUp();
                }
                else
                {
                    Debug.Log("Duvarın üst kısmına yaklaşılmadı.");
                }
            }
            else
            {
                Debug.Log("Duvarın üst kısmı bulunamadı.");
            }
        }

        Debug.DrawRay(climbCheck.position, direction * climbCheckDistance, _canClimb ? Color.green : Color.red);
    }

    void StartClimbingUp()
    {
        if (!_isClimbing)
        {
            _isClimbing = true;
            Debug.Log("Tırmanma animasyonu tetiklendi.");
            _animator.SetTrigger(AnimatorHashes.ClimbUp);
            _rb.linearVelocity = Vector2.zero;
            _rb.gravityScale = 0;
            
            // StartCoroutine(Climb());
        }
    }

    int GetFacingDirection()
    {
        return GetComponent<SpriteRenderer>().flipX ? -1 : 1;
    }

    System.Collections.IEnumerator Climb()
    {
        yield return new WaitForSeconds(climbDuration); // Animasyon süresi kadar bekle

        // Karakteri yukarıya doğru taşı ve bakış yönüne göre hareket et
        Vector2 finalOffset = new Vector2(climbOffset.x * GetFacingDirection(), climbOffset.y);
        _rb.MovePosition(_rb.position + finalOffset);
        Debug.Log("Karakter yukarı taşındı.");

        // Tırmanma bitti, yerçekimini yeniden aç
        _rb.gravityScale = 1;
        _isClimbing = false;
    }
}
