using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 speed = Vector3.zero;
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float smoothness = 0.8f;

    Rigidbody rb;

    public bool IsLocked { get; protected set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovementUpdate();
    }

    private void FixedUpdate()
    {
        speed *= smoothness;
    }


    public void MovementUpdate()
    {
        Vector3 movement = Vector3.zero;
        if (!IsLocked)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.z = Input.GetAxisRaw("Vertical");
        }

        speed += movement;

        // no speed
        if (speed.magnitude == 0)
            return;

        // clamp
        speed = new Vector3(Mathf.Clamp(speed.x, -1, 1), 0, Mathf.Clamp(speed.z, -1, 1));

        // Normalize diagonal movement
        if (speed.magnitude > 1f)
            speed.Normalize();

        rb.MovePosition(rb.position + Time.deltaTime * moveSpeed * speed);
    }


    public void StopAndLock()
    {
        speed = Vector3.zero;
        LockInput();
    }
    public void LockInput()
    {
        IsLocked = true;
    }
    public void UnlockInput()
    {
        IsLocked = false;
    }
}
