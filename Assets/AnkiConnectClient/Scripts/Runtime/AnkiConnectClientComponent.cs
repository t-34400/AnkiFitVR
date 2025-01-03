#nullable enable

using System;
using UnityEngine;

namespace AnkiConnectClient
{
    class AnkiConnectClientComponent : MonoBehaviour
    {
        public void GetDeckNames(Action<string[]> onSuccess) => AnkiConnectClientWrapper.GetDeckNames(this, onSuccess);
        public void OpenDeckReview(string deckName, Action<string[]> onSuccess) => AnkiConnectClientWrapper.OpenDeckReview(this, deckName, onSuccess);
        public void ShowAnswer(Action<string[]> onSuccess) => AnkiConnectClientWrapper.ShowAnswer(this, onSuccess);
        public void AnswerCard(int ease, Action<string[]> onSuccess) => AnkiConnectClientWrapper.AnswerCard(this, ease, onSuccess);

        [ContextMenu("Show Answer")]
        public void ShowAnswer() => ShowAnswer((result) => { });
        [ContextMenu("Answer Card: Again")]
        public void AnswerCardAgain() => AnswerCard(1, (result) => { });
        [ContextMenu("Answer Card: Good")]
        public void AnswerCardGood() => AnswerCard(3, (result) => { });
    }
}
