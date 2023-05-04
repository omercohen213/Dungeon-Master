using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public interface IDamageable
{
    void ReceiveDamage(int damageAmount, float pushForce, Vector3 origin);
    void Death();
}
