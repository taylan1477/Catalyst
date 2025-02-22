using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.0f;
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
       
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
    } 
    else
    {
        speed -= 0.02f;
    }
    Debug.Log("hız şuan: " + speed);
    speed = Mathf.Clamp(speed, 0,2);
    _animator.SetFloat("speed", speed);
    _rigidbody2D.linearVelocity = new Vector2(speed, 0f);
}
}
