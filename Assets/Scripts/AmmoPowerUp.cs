using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPowerUp : PowerUp
{
    [Header("Ammo PowerUp Settings")]
    [SerializeField, Tooltip("The amount of ammo that is restored")]
    private int ammoRestore = 21;
    
    public override void Activate(int player)
    {
        PowerUpManager.Instance.RestoreAmmo(ammoRestore, player);
        base.Activate(player);
    }
}
