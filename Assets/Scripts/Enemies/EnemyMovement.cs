using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [field: SerializeField] public Enemy Enemy { get; protected set; }
    [SerializeField] GenericStateMachine stateMachine;
    [SerializeField] SpriteRenderer enemySprite;

    Vector3 speed = Vector3.zero;
    [SerializeField] float moveSpeed = 3.5f;
    [SerializeField] float smoothness = 0.8f;

    [SerializeField] Rigidbody rb;

    private Vector3 targetPosition;
    [SerializeField] float targetReachThreshold = 0.5f; // Threshold to consider target reached
    [SerializeField] float randomRange = 2f; // Range for generating random target positions
    [SerializeField] float waitTime = 2f;

    public bool IsLocked { get; protected set; }
    public bool IsWaiting { get; protected set; }

    private void Start()
    {
        GenerateNewTargetPosition();
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
        if (!IsLocked && !IsWaiting)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            Vector3 movement = direction;

            if (movement.x != 0)
                enemySprite.flipX = movement.x > 0;

            speed += movement;

            // Check if target position is reached
            if (Vector3.Distance(transform.position, targetPosition) <= targetReachThreshold)
            {
                StartCoroutine(WaitAndGenerateNewTarget());
            }
        }

        // no speed
        if (speed.magnitude <= 0.5f)
            stateMachine.Stop("walk");
        else
            stateMachine.Play("walk", overwrite: false, fade: false);

        rb.MovePosition(rb.position + Time.deltaTime * moveSpeed * speed);
    }

    private void GenerateNewTargetPosition()
    {
        if (Enemy == null || Enemy.EnemySpawner == null)
            return;

        targetPosition = Enemy.EnemySpawner.GetRandomBoundsPoint();
    }

    IEnumerator WaitAndGenerateNewTarget()
    {
        IsWaiting = true;
        speed = Vector3.zero;
        stateMachine.Stop("walk");

        yield return new WaitForSeconds(waitTime);

        GenerateNewTargetPosition();
        IsWaiting = false;
    }

    public void StopAndLock()
    {
        speed = Vector3.zero;
        stateMachine.Stop("walk");
        Lock();
    }
    public void Lock()
    {
        IsLocked = true;
    }
    public void Unlock()
    {
        IsLocked = false;
        GenerateNewTargetPosition();
    }

    public void PushAway(Vector3 direction, float forceMultiplier = 1f)
    {
        direction.y = 0;
        speed = direction.normalized * forceMultiplier;
    }
}
