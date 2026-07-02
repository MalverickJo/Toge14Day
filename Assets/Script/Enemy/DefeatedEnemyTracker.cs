using UnityEngine;
using System.Collections.Generic;

public class DefeatedEnemyTracker : MonoBehaviour
{
    public static DefeatedEnemyTracker Instance;
    private HashSet<string> defeatedIds = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void MarkDefeated(string id)
    {
        if (Instance != null)
            Instance.defeatedIds.Add(id);
    }

    public static bool IsDefeated(string id)
    {
        return Instance != null && Instance.defeatedIds.Contains(id);
    }

    public static void ClearAll()
    {
        Instance?.defeatedIds.Clear();
    }
}