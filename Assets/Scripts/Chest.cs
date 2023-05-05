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
            string text = "+" + goldAmount + " gold!";
            int fontSize = 20;
            float destroyTimer = 1.5f;
            FloatingTextManager.instance.ShowFloatingText(text, fontSize, Color.yellow, transform.position, "GetResource" , destroyTimer);
        }
    }



}
