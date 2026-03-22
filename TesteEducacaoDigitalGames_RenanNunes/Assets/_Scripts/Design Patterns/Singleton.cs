using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : Component
{
    public static T Instance { get; private set; }

    [SerializeField]
    protected bool _isGlobal = true;

    protected virtual void Awake()
    {
        int instances = FindObjectsByType<T>(FindObjectsSortMode.None).Length;

        if (Instance != null && Instance != this as T && instances > 1)
        {
            Debug.Log("Destroying repeated singleton in scene: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        if (_isGlobal)
        {
            DontDestroyOnLoad(transform.root.gameObject);
            DontDestroyOnLoad(gameObject);
        }
        Instance = this as T;
    }
}
