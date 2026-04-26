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

    public TextMeshProUGUI npcButton1Text;
    public TextMeshProUGUI npcButton2Text;
    public TextMeshProUGUI npcButton3Text;

    public TextMeshProUGUI npcButton1LabelText;
    public TextMeshProUGUI npcButton2LabelText;
    public TextMeshProUGUI npcButton3LabelText;

    public Button returnButton;
    public TextMeshProUGUI returnButtonText;

    [Header("NPC Game UI")]
    public TextMeshProUGUI npcTitleText;
    public TextMeshProUGUI npcPlaceholderText;

    public GameObject npcChatObject;

    public Button exitNpcButton;
    public TextMeshProUGUI exitNpcButtonText;

    public TextMeshProUGUI sendButtonText;

    private int currentNpcId = 0;

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
            ? "Tu esi ceļotājs teksta piedzīvojumu spēlē. Tu un nespēlējamais personāžs (NPC) atrodaties spēles pasaulē. Tavs mērķis ir nogalināt pūķi.\n\n" +
              "Katrā solī tev jāuzdod jautājumi nespēlējamajam personāžam, lai iegūtu informāciju par to, kā nogalināt pūķi. Uzdod jaunus jautājumus, balstoties uz pašreizējo novērojumu un atbildēm uz iepriekšējiem jautājumiem, ievērojot šādus noteikumus:\n\n" +
              "1. Uzdod līdzīgus un papildjautājumus tiem iepriekšējiem jautājumiem, uz kuriem atbilde bija \"jā\".\n" +
              "2. Izvairies uzdot līdzīgus un papildjautājumus tiem iepriekšējiem jautājumiem, uz kuriem atbilde bija \"nē\".\n\n" +
              "Zemāk ir pieejami trīs dažādi nespēlējamie personāži. Tu vari izvēlēties jebkuru no tiem jebkurā secībā."
            : "You are a traveler in a text adventure game. You and the NPC are both in the game. Your goal is to kill the dragon.\n\n" +
              "For each step, you should ask questions to the NPC in order to get information on how to kill the dragon. Ask a new set of questions based on the current observation and answers given to the previous set of questions according to the following rules:\n\n" +
              "1. Ask similar and follow-up questions to previous questions that have a \"yes\" answer.\n" +
              "2. Avoid asking similar and follow-up questions to previous questions that have a \"no\" answer.\n\n" +
              "Below are three different NPCs. You can choose any of them in any order.";

        npcButton1Text.text = "1";
        npcButton2Text.text = "2";
        npcButton3Text.text = "3";

        npcButton1LabelText.text = GetNpcDisplayName(1);
        npcButton2LabelText.text = GetNpcDisplayName(2);
        npcButton3LabelText.text = GetNpcDisplayName(3);

        returnButtonText.text = isLatvian ? "Atpakaļ" : "Return";
        exitNpcButtonText.text = isLatvian ? "Iziet" : "Exit";

        if (sendButtonText != null)
        {
            sendButtonText.text = isLatvian ? "Sūtīt" : "Send";
        }

        UpdateNpcScreen();
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

        UpdateNpcScreen();
    }

    private void UpdateNpcScreen()
    {
        if (npcTitleText == null)
        {
            return;
        }

        bool isLatvian = currentLanguage == GameLanguage.Latvian;

        if (currentNpcId == 0)
        {
            npcTitleText.text = isLatvian
                ? "Nespēlējamais personāžs"
                : "NPC";

            if (npcPlaceholderText != null)
            {
                npcPlaceholderText.gameObject.SetActive(true);
                npcPlaceholderText.text = isLatvian
                    ? "Šeit vēlāk būs dialogs ar izvēlēto nespēlējamo personāžu."
                    : "The selected NPC dialogue will be added here later.";
            }

            if (npcChatObject != null)
            {
                npcChatObject.SetActive(false);
            }

            return;
        }

        npcTitleText.text = GetNpcDisplayName(currentNpcId);

        if (currentNpcId == 1)
        {
            if (npcPlaceholderText != null)
            {
                npcPlaceholderText.gameObject.SetActive(false);
            }

            if (npcChatObject != null)
            {
                npcChatObject.SetActive(true);
            }
        }
        else
        {
            if (npcChatObject != null)
            {
                npcChatObject.SetActive(false);
            }

            if (npcPlaceholderText != null)
            {
                int progress = npcProgress.ContainsKey(currentNpcId) ? npcProgress[currentNpcId] : 0;

                npcPlaceholderText.gameObject.SetActive(true);

                npcPlaceholderText.text = isLatvian
                    ? $"Šeit vēlāk būs dialogs ar šo tēlu.\n\nPašreizējais progress: {progress}"
                    : $"The dialogue with this character will be added here later.\n\nCurrent progress: {progress}";
            }
        }
    }

    private string GetNpcDisplayName(int npcId)
    {
        bool isLatvian = currentLanguage == GameLanguage.Latvian;

        switch (npcId)
        {
            case 1:
                return isLatvian
                    ? "Mākslīgā intelekta nespēlējamais personāžs"
                    : "AI NPC";

            case 2:
                return isLatvian
                    ? "Nespēlējamais personāžs"
                    : "NPC";

            case 3:
                return isLatvian
                    ? "??? nespēlējamais personāžs"
                    : "??? NPC";

            default:
                return isLatvian
                    ? "Nespēlējamais personāžs"
                    : "NPC";
        }
    }
}