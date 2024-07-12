using UIItem_NM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UIGridItems
{

    public class UIGridItem : UIItem
    {
        [field: SerializeField] public UIGridHandler ItemsHandler { get; private set; }

        #region Pointer Events
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IgnoreEvents)
                return;

            base.OnPointerClick(eventData);

            ItemsHandler.OnItemClick(this);
        }



        // drag
        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (IgnoreEvents || !Draggable)
                return;

            base.OnBeginDrag(eventData);

            ItemsHandler.InitializePreviewItem(Item);
        }
        public override void OnDrag(PointerEventData eventData)
        {
            if (IgnoreEvents || !Draggable)
                return;

            base.OnDrag(eventData);

            ItemsHandler.SetPreviewItemPosition(eventData.position);
        }
        public override void OnEndDrag(PointerEventData eventData)
        {
            if (IgnoreEvents || !Draggable)
                return;

            base.OnEndDrag(eventData);

            ItemsHandler.DiscardPreviewItem();
        }
        public override void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            base.OnDrop(eventData);

            if (!eventData.pointerDrag.TryGetComponent(out UIGridItem from))
                return;

            // prevent self drag and drop
            if (from == this)
                return;

            // from is not draggable
            if (from.IgnoreEvents || !from.Draggable)
                return;

            from.ItemsHandler.OnItemDropped(from, this);
        }

        #endregion
    }

}

