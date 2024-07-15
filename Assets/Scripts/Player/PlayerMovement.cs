using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GenericStateMachine stateMachine;
    [SerializeField] SpriteRenderer playerSprite;

    Vector3 speed = Vector3.zero;
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float smoothness = 0.8f;

    [SerializeField] Rigidbody rb;

    public bool IsLocked { get; protected set; }



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

        if (!IsLocked)
        {
            Vector3 movement = Vector3.zero;
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.z = Input.GetAxisRaw("Vertical");

            if (movement.x != 0)
                playerSprite.flipX = movement.x > 0;

            // Normalize diagonal movement
            if (movement.magnitude > 1)
                movement.Normalize();

            speed += movement;
        }

        // no speed
        if (speed.magnitude <= 0.5f)
            stateMachine.Stop("walk");
        else
            stateMachine.Play("walk", overwrite: false, fade: false);

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
        direction.y = 0;
        speed = direction.normalized * forceMultiplier;
    }
}
