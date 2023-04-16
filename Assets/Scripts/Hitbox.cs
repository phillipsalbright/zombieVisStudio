using Unity.VisualScripting;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private int type;
    [SerializeField] private Zombie zombie;

    private void Start()
    {
        if (zombie == null)
        {
            zombie = this.transform.root.GetComponent<Zombie>();
        }
    }

    public void DamageBodyPart(float damage) {
        zombie.TakeDamage(damage, type);
    }

    public int GetType()
    {
        return type;
    }
}
