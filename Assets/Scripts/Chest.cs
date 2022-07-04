using UnityEngine;

public class Chest : Collectable
{
    public Sprite emptyChest;
    public int goldAmount = 10;
    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            GameManager.instance.gold += goldAmount;
            GameManager.instance.ShowText("+" + goldAmount + " gold!", 20, Color.yellow, transform.position, Vector3.up * 20, 0.5f);
        }
    }



}
