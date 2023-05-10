using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private Player player;
    private Inventory inventory;

    [SerializeField] private List<NPC> npcs;
    [SerializeField] private GameObject itemPreview;

    // Shop objects
    [SerializeField] private GameObject shopExitButton;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform content;

    private void Start()
    {
        player = Player.instance;
        inventory = Inventory.instance;
    }

    public void BuyItem(Item item)
    {
        if (player.Gold <= item.price)
        {
            Debug.Log("Not Enough Gold!");
            return;
        }
        inventory.AddItem(item);
        player.Gold -= item.price;
    }

    public void SellItem()
    {

    }
}
