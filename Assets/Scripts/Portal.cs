using System.Threading.Tasks;
using UnityEngine;

public class Portal : Collidable
{
    private Player playerToTeleport;
    [SerializeField] private string sceneToLoad;
    public bool isLoaded = true;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player" && isLoaded)
        {           
            isLoaded = false;
            playerToTeleport = coll.GetComponent<Player>();
            DungeonManager.instance.LoadScene(sceneToLoad);

            // Creating a delay to prevent the scene being loaded many times
            Task.Run(async delegate
            {
                await Task.Delay(3000);
                isLoaded = true;
            });            
        }       
    }
}
