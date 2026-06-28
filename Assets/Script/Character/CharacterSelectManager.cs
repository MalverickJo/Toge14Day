using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    public CharacterRoleData[] roles;    
    public CharacterCardUI[] cards;      

    private void Start()
    {
        for (int i = 0; i < roles.Length; i++)
        {
            cards[i].Setup(roles[i]);
        }
    }
}
