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
        StartCoroutine(WaitAndGenerateNewTarget());
    }

    void Update()
    {
        MovementUpdate();
    }

    private void FixedUpdate()
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

        speed *= smoothness;
    }
    public void MovementUpdate()
    {
        // no speed
        if (speed.magnitude <= 0.5f)
            stateMachine.Stop("walk");
        else
            stateMachine.Play("walk", overwrite: false, fade: false);

        rb.MovePosition(rb.position + Time.deltaTime * moveSpeed * speed);
    }

    private void GenerateNewTarget()
    {
        if (Enemy == null || Enemy.EnemySpawner == null)
            return;

        targetPosition = Enemy.EnemySpawner.GetRandomBoundsPoint();
    }

    IEnumerator WaitAndGenerateNewTarget()
    {
        // already waiting
        if (IsWaiting)
            yield break;

        IsWaiting = true;
        speed = Vector3.zero;
        stateMachine.Stop("walk");

        yield return new WaitForSeconds(waitTime);

        GenerateNewTarget();
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
        GenerateNewTarget();
    }

    public void PushAway(Vector3 direction, float forceMultiplier = 1f)
    {
        direction.y = 0;
        speed = direction.normalized * forceMultiplier;
    }
}
