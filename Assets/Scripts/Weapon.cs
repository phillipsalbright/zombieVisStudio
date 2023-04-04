using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract void AttackDown();
    public abstract void AttackRelease();
    public abstract void Reload();
    public abstract void PutAway();
    public abstract void MakeActive();
    public abstract Transform GetModelTransform();
}
