using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Singleton")]
    [SerializeField]
    private bool _isPersistent = false;

    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();

            if (_isPersistent)
                DontDestroyOnLoad(this);
        }
        else
        {
            Debug.LogWarning($"There appears to be more than one {typeof(T)} instance in the hierarchy! Since this component is a singleton, " +
                $"consider only having one instance of it in the hierarchy.");
        }
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }
}
