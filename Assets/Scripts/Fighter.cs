using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : Entity
{
    protected float xSpeed = 0.75f;
    protected float ySpeed = 1.0f;
    protected float pushTolerance = 0.2f;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;

    [SerializeField] protected int hp;
    [SerializeField] protected int maxHp;

    // Immunity 
    protected float immuneTime = 1.0f;
    protected float lastImmune;

    // Push
    protected Vector3 pushDirection;

    protected virtual void RecieveDamage(Damage dmg)
    {
        //if (Weapon.instance.)
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hp -= dmg.dmgAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;
            GameManager.instance.ShowText(dmg.dmgAmount.ToString(), 20, Color.red, dmg.origin, Vector3.up, 1f);

            if (hp <= 0)
            {
                hp = 0;
                Death();
            }
        }
    }

    protected virtual void Death()
    {
        Debug.Log("dead");
    }

    protected virtual void UpdateMotor(Vector3 input, float speed)
    {
        //reset moveDelta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed, 0) * speed;

        // swap sprite direction when going right or left
        if (moveDelta.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Reduce push force every frame, based off push tolerance
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushTolerance);

        // Add push vector
        moveDelta += pushDirection;

        // setting up the object to move in the y axis
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y),
            Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));

        if (hit.collider == null)
        {
            // move horizontally  
            transform.Translate(0, moveDelta.y * Time.deltaTime * 0.5f, 0);
        }

        // setting up the object to move in the x axis
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0),
            Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));

        if (hit.collider == null)
        {
            // move vertically (shift for running)
            if (Input.GetKey(KeyCode.LeftShift))
                transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
            else transform.Translate(moveDelta.x * Time.deltaTime * 0.5f, 0, 0);
        }
    }
}
