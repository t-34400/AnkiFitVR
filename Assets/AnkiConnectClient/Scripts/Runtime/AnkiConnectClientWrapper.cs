using System;
using UnityEngine;

namespace AnkiConnectClient
{
    public static class AnkiConnectClientWrapper
    {
        public static void GetDeckNames(MonoBehaviour context, Action<string[]> onSuccess)
        {
            context.StartCoroutine(AnkiConnectClient.Invoke<AnkiConnectClient.StringListResponse>("deckNames",
                (result) => {
                    onSuccess.Invoke(result.result);
                },
                (error) => {
                    Debug.LogError(error);
                }
            ));
        }

        public static void OpenDeckReview(MonoBehaviour context, string deckName, Action<string[]> onSuccess)
        {
            var parameter = new NameRequestParameter()
            {
                name = deckName
            };
            var parameterJson = JsonUtility.ToJson(parameter);            
             
            context.StartCoroutine(AnkiConnectClient.Invoke<AnkiConnectClient.StringListResponse>("guiDeckReview",
                (result) => {
                    onSuccess.Invoke(result.result);
                },
                (error) => {
                    Debug.LogError(error);
                },
                parameterJson
            ));
        }

        public static void ShowAnswer(MonoBehaviour context, Action<string[]> onSuccess)
        {
            context.StartCoroutine(AnkiConnectClient.Invoke<AnkiConnectClient.StringListResponse>("guiShowAnswer",
                (result) => {
                    onSuccess.Invoke(result.result);
                },
                (error) => {
                    Debug.LogError(error);
                }
            ));
        }

        public static void AnswerCard(MonoBehaviour context, int ease, Action<string[]> onSuccess)
        {
            var parameter = new EaseRequestParameter()
            {
                ease = ease
            };
            var parameterJson = JsonUtility.ToJson(parameter);            

            context.StartCoroutine(AnkiConnectClient.Invoke<AnkiConnectClient.StringListResponse>("guiAnswerCard",
                (result) => {
                    onSuccess.Invoke(result.result);
                },
                (error) => {
                    Debug.LogError(error);
                },
                parameterJson
            ));
        }

        [Serializable]
        private class NameRequestParameter
        {
            public string name;
        }
        [Serializable]
        private class EaseRequestParameter
        {
            public int ease;
        }
    }
}
