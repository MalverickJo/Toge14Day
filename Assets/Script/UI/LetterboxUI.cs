using UnityEngine;

using System.Collections;

public class LetterboxUI : MonoBehaviour
{
    public static LetterboxUI Instance;

    public GameObject topBar;
    public GameObject bottomBar;

    private void Awake()
    {
        Instance = this;
        topBar.SetActive(false);
        bottomBar.SetActive(false);
    }

    public void Show()
    {
        topBar.SetActive(true);
        bottomBar.SetActive(true);
    }

    public void Hide()
    {
        topBar.SetActive(false);
        bottomBar.SetActive(false);
    }
}