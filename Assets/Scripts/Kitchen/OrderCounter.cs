using Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OrderCounter : MonoBehaviour
{
    [SerializeField] UIRecipeItem uiRecipeItem;
    [field: SerializeField] public List<Customer> Customers { get; protected set; }
    public Customer ShowedCustomer { get; protected set; }

    // events
    public readonly UnityEvent<Customer> onCustomerOrdered = new();
    public readonly UnityEvent<Customer> onCustomerCompleted = new();
    public readonly UnityEvent<Customer> onCustomerCanceled = new();

    private void Start()
    {
        uiRecipeItem.SetShow(false);
    }

    public void OnCustomerOrdered(Customer customer)
    {
        onCustomerOrdered.Invoke(customer);

        // not showing a customer
        if (ShowedCustomer == null)
            ShowCustomerLeastTimeOrder();

        Debug.Log($"OnCustomerOrdered {customer.Order.Name}");
        GlobalAudio.PlayEffect(GlobalAudio.GeneralClips.customerBellClip);
    }
    public void OnCustomerCompleted(Customer customer)
    {
        onCustomerCompleted.Invoke(customer);

        // showed customer is completed
        if (ShowedCustomer == customer)
            ShowCustomerLeastTimeOrder();

        Debug.Log($"OnCustomerCompleted {customer.Order.Name}");
        GlobalAudio.PlayEffect(GlobalAudio.GeneralClips.completeCustomerClip);
    }
    public void OnCustomerCanceled(Customer customer)
    {
        onCustomerCanceled.Invoke(customer);

        // showed customer is canceled
        if (ShowedCustomer == customer)
            ShowCustomerLeastTimeOrder();

        Debug.Log($"OnCustomerCanceled {customer.Order.Name}");
    }

    public void CompleteCustomer(Customer customer)
    {
        // customer is not ordering
        if (!customer.IsOrdering)
            return;

        Debug.Log($"completed customer Order {customer.Order.Name}");
        customer.CompleteOrder();
    }

    public void CancelAllOrders()
    {
        foreach (Customer ctmr in Customers)
        {
            // only ordering customers
            if (!ctmr.IsOrdering)
                continue;

            ctmr.CancelOrder();
        }
    }

    #region Display customer order
    public void ShowCustomerOrder(Customer ctmr)
    {
        Debug.Log($"ShowCustomerOrder {ctmr}");
        ShowedCustomer = ctmr;

        // no customer
        if (ctmr == null)
        {
            uiRecipeItem.Show(false);
            uiRecipeItem.UpdateRecipe(null);
            return;
        }

        uiRecipeItem.UpdateRecipe(ctmr.Order.Copy());
        uiRecipeItem.Show(true);
    }

    public void ShowFirstCustomerOrder()
    {
        foreach (Customer ctmr in Customers)
        {
            // only ordering customers
            if (!ctmr.IsOrdering)
                continue;

            ShowCustomerOrder(ctmr);
            return;
        }

        ShowCustomerOrder(null);
    }

    public void ShowCustomerLeastTimeOrder()
    {
        Customer customerWithLeastTime = null;
        float leastTime = float.MaxValue;

        foreach (Customer ctmr in Customers)
        {
            // only ordering customers
            if (!ctmr.IsOrdering)
                continue;

            if (ctmr.OrderTimer.RemainingTime < leastTime)
            {
                leastTime = ctmr.OrderTimer.RemainingTime;
                customerWithLeastTime = ctmr;
            }
        }

        ShowCustomerOrder(customerWithLeastTime);
    }
    #endregion
}
