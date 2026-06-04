/// <summary>
/// Central registry of scene names used for navigation.
/// Keeps scene-name strings in one place so typos are caught at compile time.
/// When adding a new scene, add a matching constant here and register it in Build Settings.
/// </summary>
public static class SceneNames
{
    public const string HomeScreen = "HomeScreen";
    public const string LevelCompletedScreen = "LevelCompletedScreen";
}
