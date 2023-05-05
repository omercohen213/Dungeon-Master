using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager instance;

    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private Image progressBar;
    private float progressTarget;
    private bool gameIsLoading = false;

    private void Awake()
    {
        // to avoid creating two gameManagers
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load a new scene adding inbetween a loading screen
    public async void LoadScene(string sceneName)
    {
        gameIsLoading = true;
        progressBar.fillAmount = 0;
        progressTarget = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;    
        LoadingScreen.SetActive(true);

        do
        {
            progressTarget = scene.progress;
        }
        while (scene.progress < 0.9f);
        await Task.Delay(500);
        
        SpawnPlayer();
        scene.allowSceneActivation = true;
        LoadingScreen.SetActive(false);
        gameIsLoading = false;
    }

     void Update()
    {
        // Pause the game when a new scene is loading
        if (gameIsLoading)
        {          
            progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, progressTarget, 3 * Time.unscaledDeltaTime);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

    }

    // Spawn player on scene loaded
    public void SpawnPlayer()
    {
        // Spawn point
        RectTransform portalRectTransform = GameObject.Find("SpawnPoint").GetComponent<RectTransform>();
        Transform portal = GameObject.Find("SpawnPoint").transform;
        float portalWidth = portalRectTransform.rect.width * 0.16f;
        float portalHeight = portalRectTransform.rect.height * 0.16f;

        Player.instance.transform.position = portal.position + new Vector3(portalWidth, -portalHeight / 3, 0);
    }
}
