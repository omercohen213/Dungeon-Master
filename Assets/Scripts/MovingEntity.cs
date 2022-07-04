using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEntity : Fighter
{

    protected BoxCollider2D boxCollider;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    protected float xSpeed = 0.75f;
    protected float ySpeed = 1.0f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }


    protected virtual void UpdateMotor(Vector3 input ,float speed)
    {
        //reset moveDelta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed, 0)* speed;

        // swap sprite direction when going right or left
        if (moveDelta.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        // Reduce push forve every frame, based off push tolerance
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

