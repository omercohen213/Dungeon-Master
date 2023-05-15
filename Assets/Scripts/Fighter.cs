using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fighter : MonoBehaviour, IDamageable
{
/*    protected int hp;
    public int Hp { get => hp; set => hp = value; }
    protected int maxHp;
    public int MaxHp { get => maxHp; set => maxHp = value; }*/
    protected float xSpeed = 0.75f;
    protected float ySpeed = 1;
    protected float pushTolerance = 0.2f;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    protected BoxCollider2D boxCollider;

    private GameObject hitVFXPrefab;
    private GameObject stunPrefab;

    // Push
    protected Vector3 pushDirection;

    // HitVFX delay
    private const float HITVFX_DELAY_TIME = 0.5f;
    private float lastHitVFXTime;
    private float hitVFXDelay;

    // Damage Delay
    protected float damageDelay;
    protected float lastDamage;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        hitVFXPrefab = Resources.Load<GameObject>("Prefabs/HitVFX");
        stunPrefab = Resources.Load<GameObject>("Prefabs/Stun");
        hitVFXDelay = HITVFX_DELAY_TIME;
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
            // move vertically (hold shift to run)
            if (Input.GetKey(KeyCode.LeftShift))
                transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
            else transform.Translate(moveDelta.x * Time.deltaTime * 0.5f, 0, 0);
        }
    }

    public virtual void GetStun(float duration)
    {
        GetComponent<Collider2D>().enabled = false;
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null)
        {
            rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        }

        Vector3 stunPos = transform.position + new Vector3(0, 0.1f, 0);
        GameObject stunObj = Instantiate(stunPrefab, stunPos, Quaternion.identity);

        
        StartCoroutine(RotateForDuration(stunObj, duration));
        Destroy(stunObj, duration);
        GetComponent<Collider2D>().enabled = true;

        if (rigidbody != null)
        {
            rigidbody.constraints = RigidbodyConstraints2D.None;
        }
    }

    // Rotate the stun object for the duration
    private IEnumerator RotateForDuration(GameObject stunObj, float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            stunObj.transform.Rotate(Vector3.up, 90.0f * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public virtual void GetKnockUp(float duration, float distance)
    {
        StartCoroutine(GetKnockUpCoroutine(duration, distance));
    }

    private IEnumerator GetKnockUpCoroutine(float duration, float distance)
    {
        float startTime = Time.time;
        float peakTime = startTime + (duration / 2f);
        float endTime = startTime + duration;

        Vector3 startPosition = transform.position;
        Vector3 peakPosition = startPosition + (Vector3.up * distance);
        Vector3 endPosition = startPosition;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            if (Time.time <= peakTime)
            {
                transform.position = Vector3.Lerp(startPosition, peakPosition, t * 2f);
            }

            else
            {
                transform.position = Vector3.Lerp(peakPosition, endPosition, (t - 0.5f) * 2f);
            }
            yield return null;
        }
    }

    public virtual void ReceiveDamage(int damageAmount, float pushForce, Vector3 origin)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool IsDamageToKill(float damage)
    {
        throw new System.NotImplementedException();
    }

    public virtual void Death()
    {
        throw new System.NotImplementedException();
    }

    public void ShowHitVFX(Vector3 pos, float scale, Transform parent)
    {
        StartCoroutine(ShowHitVFXCoroutine(pos, scale, parent));
    }

    private IEnumerator ShowHitVFXCoroutine(Vector3 pos, float scale, Transform parent)
    {
        
        if (Time.time - lastHitVFXTime > hitVFXDelay)
        {
            lastHitVFXTime = Time.time;
            GameObject hitVFXObj = Instantiate(hitVFXPrefab, pos, Quaternion.identity, parent);
            float duration = 0.2f;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                hitVFXObj.transform.localScale = Vector3.Lerp(new Vector3(scale, scale), Vector3.zero, t);
                SpriteRenderer spriteRenderer = hitVFXObj.GetComponent<SpriteRenderer>();
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }       
    }
}
