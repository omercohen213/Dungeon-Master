using UnityEngine;

public class Chest : Collectable
{
    [SerializeField] private Player player;
    [SerializeField] private HUD hud;
    [SerializeField] private Sprite emptyChest;
    [SerializeField] private int goldAmount = 10;

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            player.SetGold(player.GetGold() + goldAmount);
            hud.onGoldChange();
            GameManager.instance.ShowText("+" + goldAmount + " gold!", 20, Color.yellow, transform.position, Vector3.up * 20, 0.5f);
        }
    }



}
