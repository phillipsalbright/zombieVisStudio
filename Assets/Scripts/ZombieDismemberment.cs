using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDismemberment : MonoBehaviour
{
    [SerializeField] private int type;
    [SerializeField] private Zombie zombie;
    public void DamageBodyPart(float damage) {
        zombie.TakeDamage(damage, type);
    }
}
