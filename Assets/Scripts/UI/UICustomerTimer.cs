using UnityEngine;
using UnityEngine.UI;

public class UICustomerTimer : UIShowPanel
{

    [SerializeField] Image fillerImg;
    [SerializeField] Image centerImg;
    [SerializeField] Gradient gradient;

    public void SetFillPercent(float percent)
    {
        fillerImg.fillAmount = 1 - percent;
        centerImg.color = fillerImg.color = gradient.Evaluate(percent);

    }

    private void Start()
    {
        SetShow(false);
    }

    public void OnTimerStarted()
    {
        Show(true);
    }

    public void OnTimerOver()
    {
        Show(false);
    }

}
