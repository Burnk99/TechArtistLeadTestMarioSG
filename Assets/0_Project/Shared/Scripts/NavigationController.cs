using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechArtistLeadTest.Core
{
    /// <summary>
    /// Handles scene transitions between the main screens of the application.
    /// Uses the SceneNames constants class to avoid fragile hardcoded string references.
    /// </summary>
    public class NavigationController : MonoBehaviour
    {
        /// <summary>Loads the Home Screen scene.</summary>
        public void LoadHomeScreen()
        {
            SceneManager.LoadScene(SceneNames.HomeScreen);
        }

        /// <summary>Loads the Level Completed Screen scene.</summary>
        public void LoadLevelCompletedScreen()
        {
            SceneManager.LoadScene(SceneNames.LevelCompletedScreen);
        }
    }
} // namespace TechArtistLeadTest.Core
