using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : Collidable
{
    [SerializeField] private string sceneToLoad;
    public bool isLoaded = true;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player" && isLoaded )
        {           
            isLoaded = false;
            DungeonManager.instance.LoadScene(sceneToLoad);

            // Creating a delay to prevent the scene being loaded many times
            Task.Run(async delegate
            {
                await Task.Delay(1000);
                isLoaded = true;
            });            
        }
    }
}
