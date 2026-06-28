using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset firstSceneAsset;
#endif
    [SerializeField] private string firstScene;

    private const string ReturnSceneKey = "EditorBootstrap_ReturnScene";

    private void Start()
    {
        string targetScene = firstScene;

#if UNITY_EDITOR
        string returnScene = PlayerPrefs.GetString(ReturnSceneKey, string.Empty);
        if (!string.IsNullOrEmpty(returnScene))
        {
            targetScene = returnScene;
            PlayerPrefs.DeleteKey(ReturnSceneKey);
        }
#endif

        SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (firstSceneAsset != null)
        {
            firstScene = firstSceneAsset.name;
        }
#endif
    }
}