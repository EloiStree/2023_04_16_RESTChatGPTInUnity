using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChatGPTRequest : MonoBehaviour
{

    public string apikey;
    private string endpoint = "https://api.openai.com/v1/chat/completions";

    public string m_respond;
    public string m_error;

    public bool m_useOnStart;
    [System.Serializable]
    public class Message
    {
        public string role = "user";
        public string content = "What is the meaning of 42 ?";
    }

    [System.Serializable]
    public class Payload
    {
        public string model= "gpt-3.5-turbo";
        public List<Message> messages = new List<Message>() { new Message() { role="user", content="What is the meaning of 42 ?"} };
        public double temperature=0.7f;
        public int max_tokens = 7;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }

    [System.Serializable]
    public class ResponseData
    {
        public List<Choice> choices;
    }


    public Coroutine m_sentRequest;
    private void Start()
    {
        if(m_useOnStart)
            SendRequestToChatGPT();
    }
    [ContextMenu("Send Chat GPT request")]
    private void SendRequestToChatGPT()
    {
        if (m_sentRequest != null)
        {
            StopCoroutine(m_sentRequest);
            m_sentRequest = null;
        }

        m_sentRequest = StartCoroutine(SendChatGPTRequest());
    }
    public void SetApiKey(string key)
    {
        apikey = key;
    }
    public Payload payload;
    IEnumerator SendChatGPTRequest()
    {
        //payload.messages = new List<Message>
        //{
        //    new Message { role = "system", content = "You can start the conversation." },
        //    new Message { role = "user", content = "Hello, Chat GPT!" }
        //};

        string jsonPayload = JsonUtility.ToJson(payload,true);
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(byteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apikey);

        m_respond = "";
        m_error = "";
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;
            ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseJson);
            string assistantReply = responseData.choices[0].message.content;
            Debug.Log("Assistant: " + assistantReply);
            m_respond = assistantReply;
        }
        else
        {
            Debug.LogError("Error: " + request.error);
            m_error = request.error;
        }
        request.Dispose();
        
    }
}
