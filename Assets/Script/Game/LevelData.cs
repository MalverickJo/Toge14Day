using UnityEngine;

public class LevelData : MonoBehaviour
{
    public static LevelData Instance;

    public int partyLevel = 1;
    public int currentEXP = 0;
    public int maxLevel = 20;

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
    public int GetEXPToNextLevel(int level)
    {
        return 100 * level;
    }

    public void AddEXP(int amount)
    {
        if (partyLevel >= maxLevel) return;

        currentEXP += amount;
        Debug.Log($"EXP: {currentEXP}/{GetEXPToNextLevel(partyLevel)}");

        while (currentEXP >= GetEXPToNextLevel(partyLevel) && partyLevel < maxLevel)
        {
            currentEXP -= GetEXPToNextLevel(partyLevel);
            LevelUp();
        }
    }

    void LevelUp()
    {
        partyLevel++;
        Debug.Log($"Level Up! Party sekarang level {partyLevel}");
        Character[] characters = FindObjectsOfType<Character>();
        foreach (var c in characters)
            c.OnLevelUp(partyLevel);
    }

    public void Reset()
    {
        partyLevel = 1;
        currentEXP = 0;
    }
}
