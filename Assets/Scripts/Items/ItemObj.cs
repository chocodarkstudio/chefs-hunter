
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Items
{
    public class ItemObj<T> : ScriptableObject
        where T : Item
    {
        [field: SerializeField] public T Item { get; protected set; }
        /// <summary>
        /// Get a copy of the item </summary>
        public T Get => Item.Copy();

        private void OnValidate()
        {
            // Initialize
            if (Item != null)
            {
                if (Item.ID == -1)
                    AutoAssignIDs();

                if (string.IsNullOrEmpty(Item.Name))
                    Item.Name = this.name;
            }
        }

        [ContextMenu(nameof(AutoAssignIDs))]
        public virtual void AutoAssignIDs()
        {
            List<ItemObj<T>> itemsObjs = GetLocalFolderItemObjs();
            if (itemsObjs == null)
                return;

            int currentID = 0;
            foreach (ItemObj<T> itemObj in itemsObjs)
            {
                itemObj.Item.ID = currentID++;
            }
            Debug.Log("ItemObj: AutoAssignIDs");
        }

        List<ItemObj<T>> GetLocalFolderItemObjs()
        {
            int instanceID = this.GetInstanceID();

            // get file path
            string filePath = AssetDatabase.GetAssetPath(instanceID);
            if (string.IsNullOrEmpty(filePath))
                return null;

            // get folder path of file
            string currentFolder = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(currentFolder))
                return null;

            List<ItemObj<T>> itemsObjs = new();

            // get all assets on current folder
            string[] results = AssetDatabase.FindAssets("t:ScriptableObject", new[] { currentFolder });
            foreach (string guid in results)
            {
                if (string.IsNullOrEmpty(guid))
                    continue;

                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath))
                    continue;

                // get only ItemObj assets
                ItemObj<T> itemObj = AssetDatabase.LoadAssetAtPath<ItemObj<T>>(assetPath);
                if (itemObj == null)
                    continue;

                itemsObjs.Add(itemObj);
            }

            return itemsObjs;
        }


    }
}