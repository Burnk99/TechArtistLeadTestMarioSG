using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{
    public void LoadHomeScreen()
    {
        SceneManager.LoadScene(SceneNames.HomeScreen);
    }

    public void LoadLevelCompletedScreen()
    {
        SceneManager.LoadScene(SceneNames.LevelCompletedScreen);
    }
}
