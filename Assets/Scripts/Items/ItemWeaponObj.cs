using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "new ItemWeaponObj", menuName = "Items/ItemWeaponObj", order = 0)]
    public class ItemWeaponObj : ItemObj<ItemWeapon>
    {
#if UNITY_EDITOR
        [ContextMenu(nameof(AutoAssignIDs))]
        public override void AutoAssignIDs()
        {
            base.AutoAssignIDs();
        }
#endif
    }
}