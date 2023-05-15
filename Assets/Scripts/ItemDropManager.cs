using System.Collections;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    public static ItemDropManager instance;
    GameObject itemDropObj;
    [SerializeField] private GameObject itemDropPrefab;

    private readonly float timeBeforeDestroyed = 10f;

    private void Awake()
    {
        instance = this;
    }

    // Create the item drop object and its components
    public void CreateItemDrop(Item item, Vector3 position)
    {
        itemDropObj = Instantiate(itemDropPrefab, position, Quaternion.identity);
        itemDropObj.GetComponent<SpriteRenderer>().sprite = item.itemSprite;
        itemDropObj.name = item.name;
        itemDropObj.transform.localScale = item.spriteSize;
        itemDropObj.GetComponent<ItemDrop>().Item = item;
        StartCoroutine(DestroyItemDrop(timeBeforeDestroyed));
    }
   
    // Destroy item gameObject after dropped
    private IEnumerator DestroyItemDrop(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(itemDropObj);
    }
}
