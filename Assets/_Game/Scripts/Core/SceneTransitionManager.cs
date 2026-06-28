using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent singleton. The only class permitted to call SceneManager.LoadScene.
/// </summary>
/// 
public class SceneTransitionManager : PersistentSingleton<SceneTransitionManager>
{
    [Header("Scene Names")]
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset lobbySceneAsset;
    [SerializeField] private UnityEditor.SceneAsset gameSceneAsset;
#endif
    [SerializeField] private string lobbyScene;
    [SerializeField] private string gameScene;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (lobbySceneAsset != null) lobbyScene = lobbySceneAsset.name;
        if (gameSceneAsset != null) gameScene = gameSceneAsset.name;
#endif
    }

    public void GoToGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene(lobbyScene);
    }
}