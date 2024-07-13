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
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }

        if (!IsLocked)
        {
            Vector3 movement = Vector3.zero;
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.z = Input.GetAxisRaw("Vertical");

            // Normalize diagonal movement
            if (movement.magnitude > 1)
                movement.Normalize();

            speed += movement;
        }

        // no speed
        if (speed.magnitude == 0)
            return;

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

    public void PushAway(Vector3 direction, float forceMultiplier = 1f)
    {
        speed = direction.normalized * forceMultiplier;
    }
}
