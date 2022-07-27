using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected BoxCollider2D boxCollider;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

}


