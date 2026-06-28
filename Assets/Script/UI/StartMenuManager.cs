using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    public GameObject startMenuPanel;
    public GameObject characterSelectPanel;

    private void Start()
    {
        startMenuPanel.SetActive(true);
        characterSelectPanel.SetActive(false);
    }

    public void OnStartButton()
    {
        startMenuPanel.SetActive(false);
        characterSelectPanel.SetActive(true);
    }

    public void OnBackButton()
    {
        characterSelectPanel.SetActive(false);
        startMenuPanel.SetActive(true);
    }
}
