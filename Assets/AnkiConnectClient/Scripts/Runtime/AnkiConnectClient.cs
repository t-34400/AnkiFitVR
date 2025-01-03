using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace AnkiConnectClient
{
    public static class AnkiConnectClient
    {
        public static string CreateRequest(string action, string parameterJson = null)
        {
            if (string.IsNullOrEmpty(parameterJson))
            {
                var request = new NoParameterRequest
                {
                    action = action,
                    version = 6
                };

                return JsonUtility.ToJson(request);
            }
            else
            {
                var request = new Request
                {
                    action = action,
                    version = 6
                };

                var requestJson = JsonUtility.ToJson(request);

                requestJson = requestJson.Replace(Request.PARAMS_PLACEHOLDER_KEY, "params");
                requestJson = requestJson.Replace('"' + Request.PARAMS_PLACEHOLDER_VALUE + '"', parameterJson);

                return requestJson;
            }
        }

        public static IEnumerator Invoke<T>(string action, Action<T> onSuccess, Action<string> onError, string parameterJson = null) where T : ResponseBase
        {
            string requestJson = CreateRequest(action, parameterJson);
            Debug.Log($"Request: {requestJson}");

            using (UnityWebRequest webRequest = new UnityWebRequest("http://127.0.0.1:8765", "POST"))
            {
                byte[] jsonToSend = new UTF8Encoding().GetBytes(requestJson);
                webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    onError?.Invoke("Error: " + webRequest.error);
                    yield break;
                }

                string responseText = webRequest.downloadHandler.text;
                Debug.Log($"Response: {responseText}");

                try
                {
                    var errorResponse = JsonUtility.FromJson<ErrorResponse>(responseText);
                    if (!string.IsNullOrEmpty(errorResponse?.error))
                    {
                        onError?.Invoke("Error: " + (errorResponse?.error ?? "Unknown"));
                        yield break;
                    }

                    var response = JsonUtility.FromJson<T>(responseText);
                    if (response == null)
                    {
                        onError?.Invoke("Error: " + "Invalid format response");
                        yield break;
                    }

                    onSuccess?.Invoke(response);
                }
                catch (Exception e)
                {
                    onError?.Invoke("Exception: " + e.Message);
                }
            }
        }

        [Serializable]
        private class NoParameterRequest
        {
            public string action;
            public int version;
        }

        [Serializable]
        private class Request
        {
            public const string PARAMS_PLACEHOLDER_KEY = nameof(_Params_Placeholder);
            public const string PARAMS_PLACEHOLDER_VALUE = "PARAMS_PLACEHOLDER_VALUE";

            public string action;
            public int version;

            [NonSerialized]
            public string parameterJson;
            [SerializeField]
            private string _Params_Placeholder = PARAMS_PLACEHOLDER_VALUE;
        }

        [Serializable]
        public class ErrorResponse
        {
            public string error;
        }

        [Serializable]
        public class ResponseBase
        {
        }

        [Serializable]
        public class StringResponse : ResponseBase
        {
            public string result;
        }

        [Serializable]
        public class StringListResponse : ResponseBase
        {
            public string[] result;
        }
    }
}
