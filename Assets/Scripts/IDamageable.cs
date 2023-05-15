using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public interface IDamageable
{
    void ReceiveDamage(int damageAmount, float pushForce, Vector3 origin);
    void GetStun(float duration);
    void GetKnockUp(float duration, float distance);
    bool IsDamageToKill(float damage);
    void ShowHitVFX(Vector3 pos, float scale, Transform parent);
    void Death();
}
