using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1f;
    public int attackDamage = 1;
    public LayerMask mouseLayer;
    public Transform carryPosition;
    
    private GameObject _carriedMouse;
    private bool _isCarrying;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) Attack();
        if (Input.GetKeyDown(KeyCode.F)) ToggleMouseCarry();
        if (_isCarrying) UpdateCarryPosition();
    }

    void Attack()
    {
        _animator.SetTrigger(AnimatorHashes.AttackTrigger);
        
        Collider2D[] hitMice = Physics2D.OverlapCircleAll(transform.position, attackRange, mouseLayer);
        foreach (Collider2D mouse in hitMice)
        {
            mouse.GetComponent<MouseController>()?.TakeDamage(attackDamage);
        }
    }

    void ToggleMouseCarry()
    {
        if (_isCarrying) DropMouse();
        else TryPickUpMouse();
    }

    void TryPickUpMouse()
    {
        Collider2D[] mice = Physics2D.OverlapCircleAll(transform.position, 1f, mouseLayer);
        foreach (Collider2D mouse in mice)
        {
            MouseController controller = mouse.GetComponent<MouseController>();
            if (controller != null && controller.IsDead())
            {
                PickUpMouse(mouse.gameObject);
                break;
            }
        }
    }

    void PickUpMouse(GameObject mouse)
    {
        _carriedMouse = mouse;
        _isCarrying = true;
        
        _carriedMouse.transform.SetParent(carryPosition);
        _carriedMouse.transform.localPosition = Vector3.zero;
        
        Collider2D collider = _carriedMouse.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        Rigidbody2D rb = _carriedMouse.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    void DropMouse()
    {
        if (!_isCarrying) return;

        _isCarrying = false;
        _carriedMouse.transform.SetParent(null);
        
        Collider2D collider = _carriedMouse.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true;
        
        Rigidbody2D rb = _carriedMouse.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        CheckFemaleCatProximity();
        _carriedMouse = null;
    }

    void UpdateCarryPosition()
    {
        Vector3 pos = _spriteRenderer.flipX ? 
            new Vector3(-0.065f, 0.06f, 0) : 
            new Vector3(0.065f, 0.06f, 0);
        
        carryPosition.localPosition = pos;
        
        if (_carriedMouse != null)
            _carriedMouse.GetComponent<SpriteRenderer>().flipX = _spriteRenderer.flipX;
    }

    void CheckFemaleCatProximity()
    {
        FemaleCat femaleCat = FindObjectOfType<FemaleCat>();
        if (femaleCat != null && femaleCat.IsMouseNearby())
        {
            femaleCat.GetComponent<Animator>().SetTrigger("ReceiveMouse");
            Destroy(_carriedMouse);
        }
    }
}