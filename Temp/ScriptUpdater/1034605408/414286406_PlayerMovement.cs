using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.0f;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer; // Sprite Renderer bileşeni
       
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
{
    if (Input.GetKey(KeyCode.D))
    {
        speed += 0.012f;
        _spriteRenderer.flipX = false; // Sprite'ı sağa dönük yap
    } 
    else if (Input.GetKey(KeyCode.A))
    {
        speed += 0.012f;
        _spriteRenderer.flipX = true; // Sprite'ı sola dönük yap
    }
    else
    {
        speed -= 0.02f;
    }
    speed = Mathf.Clamp(speed, 0,2);
    _animator.SetFloat("speed", speed);
    _rigidbody2D.linearVelocity = new Vector2(speed*4, 0f);
}
}
