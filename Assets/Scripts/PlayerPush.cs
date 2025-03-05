using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float pushForce = 5f; // İtme/çekme kuvveti

    private PlayerMovement playerMovement;
    private GameObject boxToPush;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            playerMovement.isSlowed = true; // Karakteri yavaşlat

            if (boxToPush != null)
            {
                PushBox(); // Sadece Shift tuşuna basılıyken kutuyu it
            }
        }
        else
        {
            playerMovement.isSlowed = false; // Hızı normale döndür
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            boxToPush = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Box"))
        {
            boxToPush = null;
        }
    }

    private void PushBox()
    {
        Vector2 pushDirection = (boxToPush.transform.position - transform.position).normalized;
        boxToPush.GetComponent<Rigidbody2D>().AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
    }
}