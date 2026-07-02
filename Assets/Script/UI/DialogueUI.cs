using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;

    private string[] lines;
    private int currentLine;
    private System.Action onComplete;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
        nextButton.onClick.AddListener(ShowNextLine);
    }

    public void Show(string npcName, string[] dialogueLines, System.Action onCompleteCallback = null)
    {
        nameText.text = npcName;
        lines = dialogueLines;
        currentLine = 0;
        onComplete = onCompleteCallback;

        panel.SetActive(true);
        Time.timeScale = 0f;

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (currentLine < lines.Length)
        {
            dialogueText.text = lines[currentLine];

            var btnText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
                btnText.text = (currentLine == lines.Length - 1) ? "End" : "Next";
        }
    }

    void ShowNextLine()
    {
        currentLine++;

        if (currentLine >= lines.Length)
            Hide();
        else
            ShowCurrentLine();
    }

    public void Hide()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        onComplete?.Invoke();
        onComplete = null;
    }
}