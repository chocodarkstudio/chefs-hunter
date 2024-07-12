using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 speed = Vector3.zero;
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float smoothness = 0.8f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        speed += movement;
        // clamp
        speed = new Vector3(Mathf.Clamp(speed.x, -1, 1), 0, Mathf.Clamp(speed.z, -1, 1));

        // Normalize diagonal movement
        if (speed.magnitude > 1f)
            speed.Normalize();

        rb.MovePosition(rb.position + Time.deltaTime * moveSpeed * speed);
    }

    private void FixedUpdate()
    {
        speed *= smoothness;
    }

}
