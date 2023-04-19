using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPowerUp : PowerUp
{
    [Header("Heal PowerUp Settings")]
    [SerializeField, Tooltip("The amount of health that is restored")]
    private int healthRestore = 6;

    public override void Activate(int player)
    {
        PowerUpManager.Instance.Heal(healthRestore);
        base.Activate(player);
    }
}
