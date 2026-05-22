using UnityEngine;

namespace BeachVolley.Core
{
    /// <summary>
    /// Singleton manager that orchestrates the entire game lifecycle.
    /// Persists across scene loads.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // ============================================================
        // SINGLETON
        // ============================================================

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton enforcement: only one instance allowed
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("[GameManager] Duplicate instance detected, destroying.");
                Destroy(gameObject);
                return;
            }

            // Ensure this is a root GameObject (required by DontDestroyOnLoad)
            if (transform.parent != null)
            {
                Debug.LogWarning("[GameManager] Was a child of " + transform.parent.name +
                    ", detaching to root for DontDestroyOnLoad to work properly.");
                transform.SetParent(null);
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("[GameManager] Initialized and persisted across scenes.");
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}