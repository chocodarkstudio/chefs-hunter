using DG.Tweening;
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
    [SerializeField] float waitTime = 2f;

    public bool IsLocked { get; protected set; }
    public bool IsWaiting { get; protected set; }

    Tween idleSquishNSquashTween;

    private void Awake()
    {
        stateMachine.onAnimationPlay.AddListener(OnAnimationPlay);
    }

    private void Start()
    {
        StartCoroutine(WaitAndGenerateNewTarget());

        Invoke(nameof(CreateIdleSquishNSquashTween), Random.Range(0f, 1f));

        if (GameManager.CombatManager.InCombat)
            StopAndLock();
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


    void OnAnimationPlay(string clipName)
    {
        Debug.Log(clipName);
        if (clipName == "idle")
        {
            // create tween
            if (!idleSquishNSquashTween.IsActive())
            {
                CreateIdleSquishNSquashTween();
            }
            else if (!idleSquishNSquashTween.IsPlaying())
                idleSquishNSquashTween.Play();
        }
        else
        {
            if (idleSquishNSquashTween.IsActive())
            {
                idleSquishNSquashTween.Pause();
                Enemy.Visuals.localScale = Vector3.one;
            }

        }
    }

    private void CreateIdleSquishNSquashTween()
    {
        // Define the squish and squash scale values
        Vector3 squishScale = new Vector3(1.1f, 0.9f, 1.0f);
        Vector3 squashScale = new Vector3(0.9f, 1.1f, 1.0f);
        float duration = 1f;

        // Create the tween sequence
        idleSquishNSquashTween = DOTween.Sequence()
            .Append(Enemy.Visuals.DOScale(squishScale, duration).SetEase(Ease.InOutQuad))
            .Append(Enemy.Visuals.DOScale(squashScale, duration).SetEase(Ease.InOutQuad))
            .SetLoops(-1, LoopType.Yoyo);
    }
}
