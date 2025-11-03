using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class ApiClient
{
    public static string BaseUrl = "http://localhost:5298"; // dev

    static UnityWebRequest MakeJsonRequest(string url, string method, string jsonBody = null)
    {
        var req = new UnityWebRequest(url, method);
        if (jsonBody != null)
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("Accept", "application/json");
        req.timeout = 10; // detik
        return req;
    }

    public static IEnumerator PostJson<TReq, TRes>(string path, TReq payload, System.Action<TRes> onOk, System.Action<string> onErr)
    {
        var url = $"{BaseUrl}{path}";
        var json = JsonUtility.ToJson(payload);
        using var req = MakeJsonRequest(url, UnityWebRequest.kHttpVerbPOST, json);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success && req.responseCode >= 200 && req.responseCode < 300)
        {
            var res = JsonUtility.FromJson<TRes>(req.downloadHandler.text);
            onOk?.Invoke(res);
        }
        else onErr?.Invoke($"{req.responseCode} {req.error} {req.downloadHandler.text}");
    }

    public static IEnumerator PutJson<TReq>(string path, TReq payload, System.Action onOk, System.Action<string> onErr)
    {
        var url = $"{BaseUrl}{path}";
        var json = JsonUtility.ToJson(payload);
        using var req = MakeJsonRequest(url, UnityWebRequest.kHttpVerbPUT, json);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success && req.responseCode >= 200 && req.responseCode < 300)
            onOk?.Invoke();
        else onErr?.Invoke($"{req.responseCode} {req.error} {req.downloadHandler.text}");
    }

    public static IEnumerator GetJson<TRes>(string path, System.Action<TRes> onOk, System.Action<string> onErr)
    {
        var url = $"{BaseUrl}{path}";
        using var req = MakeJsonRequest(url, UnityWebRequest.kHttpVerbGET);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success && req.responseCode >= 200 && req.responseCode < 300)
        {
            var res = JsonUtility.FromJson<TRes>(req.downloadHandler.text);
            onOk?.Invoke(res);
        }
        else onErr?.Invoke($"{req.responseCode} {req.error} {req.downloadHandler.text}");
    }

    public static IEnumerator Delete(string path, System.Action onOk, System.Action<string> onErr)
    {
        var url = $"{BaseUrl}{path}";
        using var req = MakeJsonRequest(url, UnityWebRequest.kHttpVerbDELETE);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success && req.responseCode >= 200 && req.responseCode < 300)
            onOk?.Invoke();
        else onErr?.Invoke($"{req.responseCode} {req.error} {req.downloadHandler.text}");
    }
}
