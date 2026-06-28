#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor-only. Lets you press Play directly inside the Lobby or Game scene
/// in the editor without manually opening Bootstrap first.
///
/// Detects that PlayerManager (and therefore the rest of the persistent
/// singleton chain) hasn't initialised, remembers which scene you were
/// trying to test, loads Bootstrap to set up persistent state, then
/// Bootstrap hands you back to the scene you originally opened.
///
/// Has zero effect on builds — BeforeSceneLoad still runs in builds, but
/// Bootstrap will always be scene index 0 there, so PlayerManager.Instance
/// is never null by the time this check runs.
/// </summary>
public static class EditorBootstrapGuard
{
    private const string BootstrapSceneName = "Bootstrap";
    private const string ReturnSceneKey      = "EditorBootstrap_ReturnScene";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureBootstrapped()
    {
        if (PlayerManager.Instance != null) return;

        string activeScene = SceneManager.GetActiveScene().name;
        if (activeScene == BootstrapSceneName) return; // normal startup path

        Debug.Log($"[EditorBootstrapGuard] Entered '{activeScene}' directly without " +
                  "Bootstrap. Loading Bootstrap to initialise persistent singletons, " +
                  "then returning to this scene.");

        PlayerPrefs.SetString(ReturnSceneKey, activeScene);
        SceneManager.LoadScene(BootstrapSceneName, LoadSceneMode.Single);
    }
}
#endif
