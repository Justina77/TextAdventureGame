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

    public TextMeshProUGUI npcButton1Text;
    public TextMeshProUGUI npcButton2Text;

    public TextMeshProUGUI npcButton1LabelText;
    public TextMeshProUGUI npcButton2LabelText;

    public Button returnButton;
    public TextMeshProUGUI returnButtonText;

    [Header("NPC Game UI")]
    public TextMeshProUGUI npcTitleText;
    public TextMeshProUGUI npcPlaceholderText;

    [Header("NPC 1 - AI NPC")]
    public GameObject npcChatObject;
    public InworldNpcChat npc1Chat;

    [Header("NPC 2 - Scripted NPC")]
    public GameObject scriptedNpcObject;
    public ScriptedNpcDialogue npc2ScriptedDialogue;

    [Header("NPC Game Buttons")]
    public Button exitNpcButton;
    public TextMeshProUGUI exitNpcButtonText;

    public TextMeshProUGUI sendButtonText;

    private int currentNpcId = 0;

    private void Start()
    {
        SetupButtons();
        ApplyLanguage();
        ShowMainMenu();
    }

    private void SetupButtons()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(ShowNpcSelect);
        }

        if (latvianButton != null)
        {
            latvianButton.onClick.AddListener(() =>
            {
                SetLanguage(GameLanguage.Latvian);
            });
        }

        if (englishButton != null)
        {
            englishButton.onClick.AddListener(() =>
            {
                SetLanguage(GameLanguage.English);
            });
        }

        if (returnButton != null)
        {
            returnButton.onClick.AddListener(ShowMainMenu);
        }

        if (npcButton1 != null)
        {
            npcButton1.onClick.AddListener(() =>
            {
                OpenNpcGame(1);
            });
        }

        if (npcButton2 != null)
        {
            npcButton2.onClick.AddListener(() =>
            {
                OpenNpcGame(2);
            });
        }

        if (exitNpcButton != null)
        {
            exitNpcButton.onClick.AddListener(ShowNpcSelect);
        }
    }

    private void SetLanguage(GameLanguage language)
    {
        currentLanguage = language;
        ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        bool isLatvian = currentLanguage == GameLanguage.Latvian;

        if (titleText != null)
        {
            titleText.text = "Text adventure game";
        }

        if (startButtonText != null)
        {
            startButtonText.text = isLatvian ? "Sākt" : "Start";
        }

        if (latvianButtonText != null)
        {
            latvianButtonText.text = "Latviešu";
        }

        if (englishButtonText != null)
        {
            englishButtonText.text = "English";
        }

        if (descriptionText != null)
        {
            descriptionText.text = isLatvian
                ? "Tu esi ceļotājs teksta piedzīvojumu spēlē. Tu un nespēlējamais personāžs (NPC) atrodaties spēles pasaulē. Tavs mērķis ir nogalināt pūķi.\n\n" +
                  "Katrā solī tev jāuzdod jautājumi nespēlējamajam personāžam, lai iegūtu informāciju par to, kā nogalināt pūķi. Uzdod jaunus jautājumus, balstoties uz pašreizējo novērojumu un atbildēm uz iepriekšējiem jautājumiem.\n\n" +
                  "Zemāk ir pieejami divi dažādi nespēlējamie personāži. Tu vari izvēlēties jebkuru no tiem jebkurā secībā."
                : "You are a traveler in a text adventure game. You and the NPC are both in the game. Your goal is to kill the dragon.\n\n" +
                  "For each step, you should ask questions to the NPC in order to get information on how to kill the dragon. Ask a new set of questions based on the current observation and answers given to the previous set of questions.\n\n" +
                  "Below are two different NPCs. You can choose either of them in any order.";
        }

        if (npcButton1Text != null)
        {
            npcButton1Text.text = "1";
        }

        if (npcButton2Text != null)
        {
            npcButton2Text.text = "2";
        }

        if (npcButton1LabelText != null)
        {
            npcButton1LabelText.text = GetNpcDisplayName(1);
        }

        if (npcButton2LabelText != null)
        {
            npcButton2LabelText.text = GetNpcDisplayName(2);
        }

        if (returnButtonText != null)
        {
            returnButtonText.text = isLatvian ? "Atpakaļ" : "Return";
        }

        if (exitNpcButtonText != null)
        {
            exitNpcButtonText.text = isLatvian ? "Iziet" : "Exit";
        }

        if (sendButtonText != null)
        {
            sendButtonText.text = isLatvian ? "Sūtīt" : "Send";
        }

        if (npc1Chat != null)
        {
            npc1Chat.SetLanguage(isLatvian);
        }

        if (npc2ScriptedDialogue != null)
        {
            npc2ScriptedDialogue.SetLanguage(isLatvian);
        }

        UpdateNpcScreen();
    }

    private void ShowMainMenu()
    {
        currentNpcId = 0;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (npcSelectPanel != null)
        {
            npcSelectPanel.SetActive(false);
        }

        if (npcGamePanel != null)
        {
            npcGamePanel.SetActive(false);
        }
    }

    private void ShowNpcSelect()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (npcSelectPanel != null)
        {
            npcSelectPanel.SetActive(true);
        }

        if (npcGamePanel != null)
        {
            npcGamePanel.SetActive(false);
        }
    }

    private void OpenNpcGame(int npcId)
    {
        currentNpcId = npcId;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (npcSelectPanel != null)
        {
            npcSelectPanel.SetActive(false);
        }

        if (npcGamePanel != null)
        {
            npcGamePanel.SetActive(true);
        }

        UpdateNpcScreen();
    }

    private void UpdateNpcScreen()
    {
        bool isLatvian = currentLanguage == GameLanguage.Latvian;

        if (npcTitleText == null)
        {
            return;
        }

        if (currentNpcId == 0)
        {
            npcTitleText.text = isLatvian
                ? "Nespēlējamais personāžs"
                : "NPC";

            if (npcPlaceholderText != null)
            {
                npcPlaceholderText.gameObject.SetActive(true);
                npcPlaceholderText.text = isLatvian
                    ? "Izvēlies nespēlējamo personāžu."
                    : "Choose an NPC.";
            }

            if (npcChatObject != null)
            {
                npcChatObject.SetActive(false);
            }

            if (scriptedNpcObject != null)
            {
                scriptedNpcObject.SetActive(false);
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

            if (scriptedNpcObject != null)
            {
                scriptedNpcObject.SetActive(false);
            }

            if (npc1Chat != null)
            {
                npc1Chat.SetLanguage(isLatvian);
                npc1Chat.BeginConversationIfNeeded();
            }
        }
        else if (currentNpcId == 2)
        {
            if (npcPlaceholderText != null)
            {
                npcPlaceholderText.gameObject.SetActive(false);
            }

            if (npcChatObject != null)
            {
                npcChatObject.SetActive(false);
            }

            if (scriptedNpcObject != null)
            {
                scriptedNpcObject.SetActive(true);
            }

            if (npc2ScriptedDialogue != null)
            {
                npc2ScriptedDialogue.SetLanguage(isLatvian);
                npc2ScriptedDialogue.BeginConversationIfNeeded();
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

            default:
                return isLatvian
                    ? "Nespēlējamais personāžs"
                    : "NPC";
        }
    }
}