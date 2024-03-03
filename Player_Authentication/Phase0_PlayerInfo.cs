using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInfo : MonoBehaviour
{
    private string token;

    public IEnumerator GetMethod()
    {
        string apiKey = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGI1OjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhYg";  // to authenticate the API
        string endPoint = "http://20.15.114.131:8080/api/login";

        using (UnityWebRequest req = UnityWebRequest.Post(endPoint, new WWWForm()))
        {
            string JsonReq = "{\"apiKey\":\"" + apiKey + "\"}";
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonReq);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string jsonRes = req.downloadHandler.text;
                TokenResponse res = JsonUtility.FromJson<TokenResponse>(jsonRes);
                token = res.token;
                Debug.Log("Token successfully feteched : " + token);
            }
            else
            {
                Debug.LogError("Failed to fetch token: " + req.error);
            }
        }
    }
    private class TokenResponse
    {
        public string token;
    }
    
    public IEnumerator FetchProfile()
    { 
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("Token can not be founded.");
            yield break; 
        }

        string url = "http://20.15.114.131:8080/api/user/profile/view";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            req.SetRequestHeader("Authorization", "Bearer " + token);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string textResponse = req.downloadHandler.text;
                Debug.Log("Profile: " + textResponse);

            }
            else
            {
                Debug.LogError("Failed... !! : " + req.error);
            }
        }
    }

    public void StartGame()
    {
        Debug.Log(" Starting the Game ");
        StartCoroutine(GetCoroutine());
    }

    private IEnumerator GetCoroutine()
    {
        yield return StartCoroutine(GetMethod());
        StartCoroutine(FetchProfile());
    }

    
}