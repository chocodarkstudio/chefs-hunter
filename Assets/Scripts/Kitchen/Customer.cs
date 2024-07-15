using DG.Tweening;
using GameUtils;
using Items;
using UIItem_NM;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [SerializeField] SpriteRenderer customerSprite;
    [SerializeField] GenericStateMachine stateMachine;

    [SerializeField] RuntimeAnimatorController[] customerAnimationVariants;

    [SerializeField] UIItem uiRecipeResult;
    [SerializeField] ItemRecipeObj orderRecipeObj;
    public ItemRecipe Order { get; protected set; }

    public bool IsOrdering { get; protected set; }

    [field: SerializeField] public Timer StartOrderTimer { get; protected set; }
    [field: SerializeField] public Timer OrderTimer { get; protected set; }

    Vector3 initialPos;
    [SerializeField] Vector3 moveOffsetToAskOrder;

    Tween movingTween;

    private void Awake()
    {
        initialPos = transform.position;

        StartOrderTimer.onCompleted.AddListener(() =>
        {
            // game over
            if (GameManager.GameSequencer.GameTimer.IsOver)
                return;

            if (orderRecipeObj != null)
                NewOrder(orderRecipeObj.Get);
            else
                NewRandomOrder();
        });

        OrderTimer.onCompleted.AddListener(OnTimerOver);

        StartOrderTimer.Restart();
    }

    private void Update()
    {
        StartOrderTimer.Update();
        OrderTimer.Update();
    }


    public void NewOrder(ItemRecipe order)
    {
        IsOrdering = false;
        Order = order;

        if (uiRecipeResult != null)
        {
            uiRecipeResult.UpdateItem(new()
            {
                Icon = order.Icon
            });
        }

        GoFront(onReach: StartOrder);
    }

    [ContextMenu(nameof(NewRandomOrder))]
    public void NewRandomOrder()
        => NewOrder(GameManager.RecipeObjs.GetOneRandom().Get);


    public void StartOrder()
    {
        IsOrdering = true;
        OrderTimer.Restart();
        GameManager.OrderCounter.OnCustomerOrdered(this);
    }
    public void EndOrder()
    {
        IsOrdering = false;
        OrderTimer.Stop();
        GoBack();

        StartOrderTimer.Restart();
    }

    public void CompleteOrder()
    {
        EndOrder();
        GameManager.OrderCounter.OnCustomerCompleted(this);
    }

    public void CancelOrder()
    {
        EndOrder();
        GameManager.OrderCounter.OnCustomerCanceled(this);
    }

    void OnTimerOver()
    {
        // not ordering
        if (!IsOrdering)
            return;

        CancelOrder();
    }


    public void GoFront(TweenCallback onReach = null)
    {
        if (movingTween.IsActive())
            movingTween.Kill();
        movingTween = transform.DOMove(initialPos + moveOffsetToAskOrder, 5f);
        movingTween.onComplete = onReach;

        // animation
        stateMachine.SetController(customerAnimationVariants.GetOneRandom());
        stateMachine.Play("run_left", fade: false);
        customerSprite.flipX = false;
    }

    public void GoBack()
    {
        if (movingTween.IsActive())
            movingTween.Kill();
        movingTween = transform.DOMove(initialPos, 1f);

        // animation
        stateMachine.Play("run_left", fade: false);
        customerSprite.flipX = true;
    }

}
