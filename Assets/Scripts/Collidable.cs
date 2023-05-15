using System.Collections;
using UnityEngine;

public class Collidable : MonoBehaviour
{
    public ContactFilter2D filter;
    private BoxCollider2D boxCollider;
    private Collider2D[] hits = new Collider2D[10];
    
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();      
    }

    protected virtual void Update()
    {
        //Collision work
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;
            OnCollide(hits[i]);

            // Array is not cleaned up so we do it by ourselves
            hits[i] = null;
        }
    }

    protected virtual void OnCollide(Collider2D coll)
    {
        Debug.Log(coll.name);
    }

    
}
