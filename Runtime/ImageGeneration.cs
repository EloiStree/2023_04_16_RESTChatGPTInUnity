using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageGeneration : MonoBehaviour
{
    // Replace with your actual API key
     string apiKey = "YOUR_API_KEY";
    private string endpoint = "https://api.openai.com/v1/images/generations";
    public Payload payload;
    public string m_message;
    public string m_error;

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class Payload
    {
        [TextArea(0,5)]
        public string prompt = "A cute baby sea otter";
        public int n = 2;
        public string size = "1024x1024";
    }

    [System.Serializable]
    public class ImageGenerationPrompt
    {
        public string content;
        public string role;
    }

    [System.Serializable]
    public class ImageGenerationResponse
    {
        public string created;
        public ImageGenerationData [] data;
    }

    [System.Serializable]
    public class ImageGenerationData
    {
        public string url;
        public string filename;
        public string format;
    }
    public bool m_useOnStart;

    private void Start()
    {
        if(m_useOnStart)
            StartCoroutine(GenerateImage());
    }

    public void SetApiKey(string key) {
        apiKey = key;
    }

    public ImageGenerationResponse responseData;
    IEnumerator GenerateImage()
    {
       
        string jsonPayload = JsonUtility.ToJson(payload);
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(byteData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);
        m_message = "";
        m_error = "";
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseJson = request.downloadHandler.text;
            m_message = responseJson;
            Debug.Log("Image URLJSON: " + responseJson);
             responseData = JsonUtility.FromJson<ImageGenerationResponse>(responseJson);
                Debug.Log("Image URL: " + responseData.data[0].url);
        }
        else
        {
            m_error = request.error;
            Debug.LogError("Error: " + request.error);
        }
    }
}
