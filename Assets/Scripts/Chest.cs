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
            player.GrantGold(goldAmount);
            GameManager.instance.SaveGame();
            hud.onGoldChange();
            FloatingTextManager.instance.ShowFloatingText("+" + goldAmount + " gold!", 20, Color.yellow, transform.position, "GetResource" , 1.5f);
        }
    }



}
