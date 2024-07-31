using DG.Tweening;
using UnityEngine;

public class UIBarFiller : UIShowPanel
{
    [SerializeField] RectTransform fillPanel;

    [SerializeField] bool invert;
    [Tooltip("Value to avoid icon overflow, in anchor min/max")]
    [SerializeField] Vector2 overflowLimit = new(0.18f, 1);

    public void SetFillerAmount(float amountPercent)
    {
        // clamp given value
        float value = Mathf.Clamp01(amountPercent);

        // invert it
        if (invert)
            value = 1 - value;

        // clamp value to avoid icon overflow
        value = Mathf.Clamp(value, overflowLimit.x, overflowLimit.y);

        fillPanel.DOAnchorMax(new Vector2(value, fillPanel.anchorMax.y), 0.1f);
        //filler.anchorMax = new Vector2(value, filler.anchorMax.y);
    }
}
