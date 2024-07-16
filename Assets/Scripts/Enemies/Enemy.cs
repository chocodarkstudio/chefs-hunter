using Items;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public Transform Visuals { get; protected set; }
    [field: SerializeField] public ItemIngredientObj Type { get; protected set; }
    [field: SerializeField] public ItemIngredientObj[] Drops { get; protected set; }
    public EnemySpawner EnemySpawner { get; set; }

    public void DestroyEnemy()
    {
        this.EnemySpawner.RemoveEnemy(this);

        Destroy(gameObject);
    }

}
