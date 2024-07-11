using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    private Rigidbody rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
       rb.MovePosition(rb.position + (Vector3)movement * Time.fixedDeltaTime * moveSpeed);
    }
}
