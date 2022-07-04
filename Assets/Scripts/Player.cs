using UnityEngine;

public class Player : MovingEntity
{
    public float speed = 2;
    private void FixedUpdate()
    {
        // arrow keys (returns 1/-1 on key down)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        

        UpdateMotor(new Vector3(x, y,0),speed);
    }
}
