using UIAnimShortcuts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIItem_NM
{
    public struct UIItemData
    {
        /// <summary>
        /// Use this id to identify items as a type or something </summary>
        [field: SerializeField] public string ID { get; set; }

        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public string PrimaryText { get; set; }
        [field: SerializeField] public string SecondaryText { get; set; }
    }

    public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public UIItemData Item { get; protected set; }

        [field: SerializeField] public Image BackgroundImg { get; protected set; }
        [field: SerializeField] public Image IconImg { get; protected set; }

        [field: SerializeField] public Text PrimaryText { get; protected set; }
        [field: SerializeField] public Text SecondaryText { get; protected set; }

        [field: SerializeField] public CanvasGroup CanvasGroup { get; protected set; }

        [field: SerializeField] public bool IgnoreEvents { get; set; } = false;
        [field: SerializeField] public bool Draggable { get; set; } = true;


        protected virtual void OnEnable()
        {
            UpdateItem(Item);
        }

        public virtual void ActiveBackground(bool active)
        {
            if (BackgroundImg == null)
                return;
            BackgroundImg.gameObject.SetActive(active);
        }

        public virtual void SetPrimaryText(string text)
        {
            if (PrimaryText == null)
                return;

            PrimaryText.text = text;
            PrimaryText.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        public virtual void SetSecondaryText(string text)
        {
            if (SecondaryText == null)
                return;

            SecondaryText.text = text;
            SecondaryText.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        public virtual void SetIcon(Sprite iconSpr)
        {
            if (IconImg == null)
                return;

            IconImg.sprite = iconSpr;
            IconImg.gameObject.SetActive(iconSpr != null);
        }

        public virtual void UpdateItem(UIItemData item)
        {
            this.Item = item;

            SetIcon(item.Icon);
            SetPrimaryText(item.PrimaryText);
            SetSecondaryText(item.SecondaryText);

            // if ID is not defined, isnt draggable (null item)
            Draggable = !string.IsNullOrEmpty(item.ID);

            gameObject.name = "UIItemData " + item.ID;
        }


        #region Pointer Events
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (IgnoreEvents)
                return;

            UIAnim.Scale(transform, Vector3.one * 0.9f);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (IgnoreEvents)
                return;
            UIAnim.BtnClick(transform);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (IgnoreEvents)
                return;

            UIAnim.Scale(transform, Vector3.one);
        }


        // drag
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (IgnoreEvents || !Draggable)
                return;
            SetIcon(null);
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (IgnoreEvents || !Draggable)
                return;

        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (IgnoreEvents || !Draggable)
                return;
            UpdateItem(Item);
        }
        public virtual void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

        }

        #endregion
    }
}
