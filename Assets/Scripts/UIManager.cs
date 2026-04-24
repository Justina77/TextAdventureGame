using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public enum GameLanguage
    {
        Latvian,
        English
    }

    [Header("Current Language")]
    public GameLanguage currentLanguage = GameLanguage.English;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject npcSelectPanel;
    public GameObject npcGamePanel;

    [Header("Main Menu UI")]
    public TextMeshProUGUI titleText;
    public Button startButton;
    public Button latvianButton;
    public Button englishButton;
    public TextMeshProUGUI startButtonText;
    public TextMeshProUGUI latvianButtonText;
    public TextMeshProUGUI englishButtonText;

    [Header("NPC Select UI")]
    public TextMeshProUGUI descriptionText;
    public Button npcButton1;
    public Button npcButton2;
    public Button npcButton3;
    public Button returnButton;
    public TextMeshProUGUI npcButton1Text;
    public TextMeshProUGUI npcButton2Text;
    public TextMeshProUGUI npcButton3Text;
    public TextMeshProUGUI returnButtonText;

    [Header("NPC Game Placeholder UI")]
    public TextMeshProUGUI npcTitleText;
    public TextMeshProUGUI npcPlaceholderText;
    public Button exitNpcButton;
    public TextMeshProUGUI exitNpcButtonText;

    private int currentNpcId = 0;

    // Здесь позже можно хранить прогресс каждого NPC отдельно.
    // Например: NPC 1 находится на 5-й реплике, NPC 2 на 2-й и т.д.
    private Dictionary<int, int> npcProgress = new Dictionary<int, int>();

    private void Start()
    {
        SetupButtons();
        ApplyLanguage();
        ShowMainMenu();
    }

    private void SetupButtons()
    {
        startButton.onClick.AddListener(ShowNpcSelect);

        latvianButton.onClick.AddListener(() =>
        {
            SetLanguage(GameLanguage.Latvian);
        });

        englishButton.onClick.AddListener(() =>
        {
            SetLanguage(GameLanguage.English);
        });

        returnButton.onClick.AddListener(ShowMainMenu);

        npcButton1.onClick.AddListener(() =>
        {
            OpenNpcGame(1);
        });

        npcButton2.onClick.AddListener(() =>
        {
            OpenNpcGame(2);
        });

        npcButton3.onClick.AddListener(() =>
        {
            OpenNpcGame(3);
        });

        exitNpcButton.onClick.AddListener(ShowNpcSelect);
    }

    private void SetLanguage(GameLanguage language)
    {
        currentLanguage = language;
        ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        bool isLatvian = currentLanguage == GameLanguage.Latvian;

        titleText.text = "Text adventure game";

        startButtonText.text = isLatvian ? "Sākt" : "Start";

        latvianButtonText.text = "Latviešu";
        englishButtonText.text = "English";

        descriptionText.text = isLatvian
            ? "Izvēlies vienu no trim tēliem. Katram tēlam būs atsevišķs dialogs un savs spēles progress."
            : "Choose one of three characters. Each character will have a separate dialogue and its own game progress.";

        npcButton1Text.text = "1";
        npcButton2Text.text = "2";
        npcButton3Text.text = "3";

        returnButtonText.text = isLatvian ? "Atpakaļ" : "Return";

        exitNpcButtonText.text = isLatvian ? "Iziet" : "Exit";

        UpdateNpcPlaceholderText();
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        npcSelectPanel.SetActive(false);
        npcGamePanel.SetActive(false);
    }

    private void ShowNpcSelect()
    {
        mainMenuPanel.SetActive(false);
        npcSelectPanel.SetActive(true);
        npcGamePanel.SetActive(false);
    }

    private void OpenNpcGame(int npcId)
    {
        currentNpcId = npcId;

        if (!npcProgress.ContainsKey(npcId))
        {
            npcProgress.Add(npcId, 0);
        }

        mainMenuPanel.SetActive(false);
        npcSelectPanel.SetActive(false);
        npcGamePanel.SetActive(true);

        UpdateNpcPlaceholderText();
    }

    private void UpdateNpcPlaceholderText()
    {
        if (npcTitleText == null || npcPlaceholderText == null)
        {
            return;
        }

        bool isLatvian = currentLanguage == GameLanguage.Latvian;

        npcTitleText.text = "NPC " + currentNpcId;

        if (currentNpcId == 0)
        {
            npcPlaceholderText.text = isLatvian
                ? "Šeit vēlāk būs dialogs ar izvēlēto NPC."
                : "The selected NPC dialogue will be added here later.";
        }
        else
        {
            int progress = npcProgress.ContainsKey(currentNpcId) ? npcProgress[currentNpcId] : 0;

            npcPlaceholderText.text = isLatvian
                ? $"Šeit vēlāk būs dialogs ar NPC {currentNpcId}.\n\nPašreizējais progress: {progress}"
                : $"The dialogue with NPC {currentNpcId} will be added here later.\n\nCurrent progress: {progress}";
        }
    }
}