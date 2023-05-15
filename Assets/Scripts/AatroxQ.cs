using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AatroxQ : MonoBehaviour
{
    public float chargeTime = 0.5f; // The time it takes to charge the ability
    public float maxChargeTime = 2f; // The maximum charge time allowed
    public float damage = 50f; // The base damage of the ability
    public float knockupDuration = 0.75f; // The duration of the knockup effect

    private bool isCharging = false; // Whether the ability is currently charging
    private float chargeTimer = 0f; // The current charge time

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isCharging)
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;

            // Clamp the charge timer to the maximum charge time allowed
            chargeTimer = Mathf.Clamp(chargeTimer, 0f, maxChargeTime);

            // Check if the player releases the Q key to activate the ability
            if (Input.GetKeyUp(KeyCode.Q))
            {
                // Calculate the damage based on the charge time
                float chargePercent = chargeTimer / maxChargeTime;
                float chargedDamage = damage * chargePercent;

                // Apply the knockup effect to enemies in range
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f);
                foreach (Collider hitCollider in hitColliders)
                {
                    Enemy enemy = hitCollider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        //enemy.ReceiveDamage(chargedDamage);
                        //enemy.ApplyKnockup(knockupDuration);
                    }
                }

                // Reset the charge timer and charging flag
                chargeTimer = 0f;
                isCharging = false;
            }
        }
    }
}
