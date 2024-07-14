using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "new ItemWeaponObj", menuName = "Items/ItemWeaponObj", order = 0)]
    public class ItemWeaponObj : ItemObj<ItemWeapon>
    {

        [ContextMenu(nameof(AutoAssignIDs))]
        public override void AutoAssignIDs()
        {
            base.AutoAssignIDs();
        }
    }
}