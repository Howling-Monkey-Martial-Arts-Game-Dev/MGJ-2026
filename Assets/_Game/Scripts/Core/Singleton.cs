using UnityEngine;

/// <summary>
/// Scene-scoped singleton. Instance is cleared on destroy so stale
/// references don't linger between scenes.
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}

/// <summary>
/// Persistent singleton. Survives scene loads via DontDestroyOnLoad.
/// Logs an explicit error if destroyed rather than silently nulling Instance,
/// since persistent singletons should never be destroyed
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    private static bool _isQuitting;

    protected override void Awake()
    {
        base.Awake();
        if (Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    protected override void OnDestroy()
    {
        if (_isQuitting)
        {
            base.OnDestroy();
            return;
        }

        if (Instance == this)
        {
            Debug.LogError($"[PersistentSingleton] {typeof(T).Name} was destroyed. Don't do this!");
        }
    }
}