using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    [SerializeField] private GameObject itemDropPrefab;
    GameObject itemDropObj;

    private IEnumerator coroutine;
    private readonly float timeBeforeDestroyed = 10f;

    // Create the item drop object and its components
    public void CreateItemDrop(Item item, Vector3 position)
    {
        itemDropObj = Instantiate(itemDropPrefab, position, Quaternion.identity);
        itemDropObj.GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        itemDropObj.name = item.name;
        itemDropObj.transform.localScale = item.spriteSize;
        itemDropObj.GetComponent<ItemDrop>().Item = item;        
        //DestroyItemDrop();
    }
   

    // Destroy item gameObject after dropped
    private IEnumerator DestroyItemDrop(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(itemDropObj);
    }
    public IEnumerator DestroyItemDrop()
    {
        coroutine = DestroyItemDrop(timeBeforeDestroyed);
        StartCoroutine(coroutine);
        return DestroyItemDrop(timeBeforeDestroyed);
    }
}
