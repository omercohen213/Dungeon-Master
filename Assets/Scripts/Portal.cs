using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : Collidable
{
    public string[] sceneNames;
    public int currentScene = 0;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
        {
            GameManager.instance.SaveState();
            string sceneName = sceneNames[currentScene + 1];
            SceneManager.LoadScene(sceneName);
        }
    }
}
