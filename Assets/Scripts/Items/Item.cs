using System;
using UnityEngine;

namespace Items
{
    [System.Serializable]
    public class Item : ICloneable
    {
        [field: SerializeField] public virtual int ID { get; set; } = -1;

        [field: SerializeField] public virtual string Name { get; set; }
        [field: SerializeField] public virtual Sprite Icon { get; set; }

        [field: SerializeField] public virtual bool Droppeable { get; set; }

        public virtual object Clone() => MemberwiseClone();

    }
}