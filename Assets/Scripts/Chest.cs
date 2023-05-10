using UnityEngine;

public class Chest : Collidable
{
    [SerializeField] private Player player;
    [SerializeField] private HUD hud;
    [SerializeField] private Sprite emptyChest;
    [SerializeField] private int goldAmount = 10;
    private bool collected;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
            if (Input.GetKeyDown(KeyCode.E))
                OnCollect();
    }
    protected virtual void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            player.GrantGold(goldAmount);
        }
    }
}
