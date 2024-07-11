using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIGridItems
{
    public struct GridItem
    {
        /// <summary>
        /// Use this id to identify items as a type or something </summary>
        [field: SerializeField] public string ID { get; set; }

        [field: SerializeField] public Sprite Icon { get; set; }
        [field: SerializeField] public string TittleText { get; set; }
        [field: SerializeField] public string BottomText { get; set; }
    }

    public class UIGridItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [field: SerializeField] public UIGridHandler ItemsHandler { get; private set; }
        public GridItem Item { get; private set; }

        [SerializeField] Image background;
        [SerializeField] Image icon;

        [SerializeField] Text tittleText;
        [SerializeField] Text bottomText;

        public bool ignoreEvents = false;
        public bool draggable = true;


        private void OnEnable()
        {
            UpdateData(Item);
        }

        public void ActiveBackground(bool active)
        {
            if (background == null)
                return;
            background.gameObject.SetActive(active);
        }

        public void SetTittleText(string text)
        {
            if (tittleText == null)
                return;

            tittleText.text = text;
            tittleText.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        public void SetBottomText(string text)
        {
            if (bottomText == null)
                return;

            bottomText.text = text;
            bottomText.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        public void SetIcon(Sprite iconSpr)
        {
            if (icon == null)
                return;

            icon.sprite = iconSpr;
            icon.gameObject.SetActive(iconSpr != null);
        }

        public void UpdateData(GridItem item)
        {
            this.Item = item;

            SetIcon(item.Icon);
            SetTittleText(item.TittleText);
            SetBottomText(item.BottomText);

            // if ID is not defined, isnt draggable (null item)
            draggable = !string.IsNullOrEmpty(item.ID);

            gameObject.name = "GridItem " + item.ID;
        }


        #region Pointer Events
        public void OnPointerDown(PointerEventData eventData)
        {
            if (ignoreEvents)
                return;

            //UIAnim.Scale(transform, Vector3.one * 0.9f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ignoreEvents)
                return;

            ItemsHandler.OnItemClick(this);
            //UIAnim.BtnClick(transform);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (ignoreEvents)
                return;

            //UIAnim.Scale(transform, Vector3.one);
        }


        // drag
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (ignoreEvents || !draggable)
                return;
            ItemsHandler.InitializePreviewItem(Item);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (ignoreEvents || !draggable)
                return;
            ItemsHandler.SetPreviewItemPosition(eventData.position);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (ignoreEvents || !draggable)
                return;
            ItemsHandler.DiscardPreviewItem();
        }
        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            if (!eventData.pointerDrag.TryGetComponent(out UIGridItem from))
                return;

            // prevent self drag and drop
            if (from == this)
                return;

            // from is not draggable
            if (from.ignoreEvents || !from.draggable)
                return;

            from.ItemsHandler.OnItemDropped(from, this);
        }

        #endregion
    }

}

