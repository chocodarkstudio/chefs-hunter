using System.Collections.Generic;
using UIItem_NM;
using UnityEngine;
using UnityEngine.Events;

namespace UIGridItems
{
    public class UIGridHandler : MonoBehaviour
    {
        [SerializeField] Transform previewItemContainer;
        UIGridItem previewItem;

        [SerializeField] Transform itemsContainer;
        [SerializeField] GameObject itemBase;

        public readonly UnityEvent<UIGridItem> onItemClick = new();
        public readonly UnityEvent<UIGridItem, UIGridItem> onItemDropped = new();

        private void Awake()
        {
            itemBase.SetActive(false);
        }

        UIGridItem CreateNewItem(Transform container)
        {
            GameObject gm = Instantiate(itemBase, container);
            gm.SetActive(true);

            return gm.GetComponent<UIGridItem>();
        }

        public void UpdateItems(List<UIItemData> items)
        {

            // start in 1 index to skip itemBase
            int i = 1;
            foreach (var item in items)
            {
                UIGridItem newItem;

                // create new child
                if (i >= itemsContainer.childCount)
                {
                    newItem = CreateNewItem(itemsContainer);
                }
                // get existing child
                else
                {
                    Transform child = itemsContainer.GetChild(i);
                    newItem = child.GetComponent<UIGridItem>();
                    child.gameObject.SetActive(true);
                }

                // update item data
                newItem.UpdateItem(item);
                i++;
            }

            // hide out of bounds items
            for (; i < itemsContainer.childCount; i++)
            {
                Transform child = itemsContainer.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }

        public void OnItemClick(UIGridItem gridItem)
        {
            onItemClick.Invoke(gridItem);
        }

        public void OnItemDropped(UIGridItem fromSlot, UIGridItem targetSlot)
        {
            DiscardPreviewItem();
            onItemDropped.Invoke(fromSlot, targetSlot);
            Debug.Log(fromSlot, fromSlot);
            Debug.Log(targetSlot, targetSlot);
        }

        public UIGridItem InitializePreviewItem(UIItemData item)
        {
            if (previewItemContainer == null)
            {
                if (GameManager.UITopLevel == null)
                    return null;

                previewItemContainer = GameManager.UITopLevel;
            }

            // create new if dosnt exists
            if (previewItem == null)
            {
                previewItem = CreateNewItem(previewItemContainer);

                // disable raycast
                previewItem.CanvasGroup.blocksRaycasts = false;
                previewItem.IgnoreEvents = true;

                previewItem.ActiveBackground(false);
            }

            previewItem.UpdateItem(item);
            previewItem.name = "Drag " + previewItem.name;
            previewItem.gameObject.SetActive(true);
            return previewItem;
        }

        public void DiscardPreviewItem()
        {
            // create new if dosnt exists
            if (previewItem == null)
                return;

            previewItem.gameObject.SetActive(false);
        }

        public void SetPreviewItemPosition(Vector2 uiPos)
        {
            if (previewItem == null)
                return;

            previewItem.transform.position = uiPos;
        }

        public void SetPreviewItemWorldPosition(Vector3 worldPos)
        {
            if (previewItem == null)
                return;

            //previewItem.transform.position = worldPos;
        }


    }
}
