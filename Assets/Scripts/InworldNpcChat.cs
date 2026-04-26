using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InworldNpcChat : MonoBehaviour
{
    [Header("Backend")]
    public string apiUrl = "http://localhost:3000/api/npc1";
    public string startApiUrl = "http://localhost:3000/api/npc1/start";
    public string backendBaseUrl = "http://localhost:3000";

    [Header("UI")]
    public TextMeshProUGUI chatLogText;
    public TMP_InputField questionInputField;
    public Button sendButton;

    [Header("Audio")]
    public AudioSource npcAudioSource;

    [Header("Scroll")]
    public ScrollRect chatScrollRect;
    public RectTransform chatContent;

    private string sessionId;
    private bool isWaitingForResponse = false;
    private bool hasStarted = false;
    private bool gameEnded = false;
    private bool isLatvian = false;

    [Serializable]
    private class NpcRequest
    {
        public string sessionId;
        public string message;
    }

    [Serializable]
    private class NpcStartRequest
    {
        public string sessionId;
        public string language;
    }

    [Serializable]
    private class NpcResponse
    {
        public string reply;
        public string error;
        public bool gameEnded;
        public string audioUrl;
    }

    private void Awake()
    {
        sessionId = Guid.NewGuid().ToString();

        if (chatLogText != null)
        {
            chatLogText.text = "";
        }

        if (sendButton != null)
        {
            sendButton.onClick.AddListener(SendQuestion);
        }

        if (questionInputField != null)
        {
            questionInputField.onSubmit.AddListener(OnInputSubmit);
        }
    }

    private void OnDestroy()
    {
        if (sendButton != null)
        {
            sendButton.onClick.RemoveListener(SendQuestion);
        }

        if (questionInputField != null)
        {
            questionInputField.onSubmit.RemoveListener(OnInputSubmit);
        }
    }

    private void OnInputSubmit(string text)
    {
        SendQuestion();
    }

    public void SetLanguage(bool latvian)
    {
        isLatvian = latvian;
    }

    public void BeginConversationIfNeeded()
    {
        if (hasStarted)
        {
            return;
        }

        hasStarted = true;
        gameEnded = false;

        if (chatLogText != null)
        {
            chatLogText.text = "";
        }

        StartCoroutine(StartConversationCoroutine());
    }

    public void SendQuestion()
    {
        if (isWaitingForResponse || gameEnded)
        {
            return;
        }

        if (questionInputField == null)
        {
            return;
        }

        string question = questionInputField.text.Trim();

        if (string.IsNullOrEmpty(question))
        {
            return;
        }

        questionInputField.text = "";

        AddToChat(GetTravelerName(), question);

        StartCoroutine(SendQuestionCoroutine(question));
    }

    private IEnumerator StartConversationCoroutine()
    {
        isWaitingForResponse = true;
        SetInputEnabled(false);

        NpcStartRequest requestData = new NpcStartRequest
        {
            sessionId = sessionId,
            language = isLatvian ? "lv" : "en"
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(startApiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            AddToChat("System", "Connection error: " + request.error);
        }
        else
        {
            NpcResponse response = JsonUtility.FromJson<NpcResponse>(request.downloadHandler.text);

            if (response == null)
            {
                AddToChat("System", "Server returned an empty response.");
            }
            else if (!string.IsNullOrEmpty(response.error))
            {
                AddToChat("System", "Server error: " + response.error);
            }
            else
            {
                if (!string.IsNullOrEmpty(response.reply))
                {
                    AddToChat("NPC", response.reply);
                }

                if (!string.IsNullOrEmpty(response.audioUrl))
                {
                    StartCoroutine(PlayNpcAudio(response.audioUrl));
                }

                if (response.gameEnded)
                {
                    EndGame();
                }
            }
        }

        isWaitingForResponse = false;

        if (!gameEnded)
        {
            SetInputEnabled(true);
        }
    }

    private IEnumerator SendQuestionCoroutine(string question)
    {
        isWaitingForResponse = true;
        SetInputEnabled(false);

        NpcRequest requestData = new NpcRequest
        {
            sessionId = sessionId,
            message = question
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            AddToChat("System", "Connection error: " + request.error);
        }
        else
        {
            NpcResponse response = JsonUtility.FromJson<NpcResponse>(request.downloadHandler.text);

            if (response == null)
            {
                AddToChat("System", "Server returned an empty response.");
            }
            else if (!string.IsNullOrEmpty(response.error))
            {
                AddToChat("System", "Server error: " + response.error);
            }
            else
            {
                if (!string.IsNullOrEmpty(response.reply))
                {
                    AddToChat("NPC", response.reply);
                }

                if (!string.IsNullOrEmpty(response.audioUrl))
                {
                    StartCoroutine(PlayNpcAudio(response.audioUrl));
                }

                if (response.gameEnded)
                {
                    EndGame();
                }
            }
        }

        isWaitingForResponse = false;

        if (!gameEnded)
        {
            SetInputEnabled(true);
        }
    }

    private IEnumerator PlayNpcAudio(string audioUrl)
    {
        if (string.IsNullOrEmpty(audioUrl))
        {
            yield break;
        }

        if (npcAudioSource == null)
        {
            AddToChat("System", "Audio Source is not assigned.");
            yield break;
        }

        string fullAudioUrl = audioUrl;

        if (audioUrl.StartsWith("/"))
        {
            fullAudioUrl = backendBaseUrl + audioUrl;
        }

        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(fullAudioUrl, AudioType.MPEG);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            AddToChat("System", "Audio error: " + request.error);
            yield break;
        }

        AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

        if (clip == null)
        {
            AddToChat("System", "Audio clip could not be loaded.");
            yield break;
        }

        npcAudioSource.Stop();
        npcAudioSource.clip = clip;
        npcAudioSource.Play();
    }

    private void AddToChat(string speaker, string message)
    {
        if (chatLogText == null)
        {
            return;
        }

        chatLogText.text += $"\n\n<b>{speaker}:</b> {message}";

        ResizeChatContent();
        StartCoroutine(ScrollToBottom());
    }

    private void ResizeChatContent()
    {
        if (chatLogText == null || chatContent == null)
        {
            return;
        }

        chatLogText.ForceMeshUpdate();

        float preferredHeight = chatLogText.preferredHeight + 120f;
        float minHeight = 400f;

        chatContent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            Mathf.Max(preferredHeight, minHeight)
        );
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null;

        Canvas.ForceUpdateCanvases();

        if (chatScrollRect != null)
        {
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void EndGame()
    {
        gameEnded = true;

        SetInputEnabled(false);

        string endText = isLatvian
            ? "Spēle ir beigusies."
            : "The game has ended.";

        AddToChat("System", endText);
    }

    private void SetInputEnabled(bool enabled)
    {
        if (questionInputField != null)
        {
            questionInputField.interactable = enabled;

            if (enabled)
            {
                questionInputField.ActivateInputField();
            }
        }

        if (sendButton != null)
        {
            sendButton.interactable = enabled;
        }
    }

    private string GetTravelerName()
    {
        return isLatvian ? "Ceļotājs" : "Traveler";
    }
}