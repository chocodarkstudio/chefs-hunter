using System.Collections.Generic;
using UnityEngine;

namespace Items
{

    [System.Serializable]
    public class ItemRecipe : Item
    {
        [field: SerializeField]
        public List<ItemIngredientObj> Ingredients { get; protected set; } = new();
    }

}

