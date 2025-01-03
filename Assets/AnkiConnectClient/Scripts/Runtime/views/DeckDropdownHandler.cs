#nullable enable

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AnkiConnectClient
{
    public class DropdownHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdownMenu = default!;
        [SerializeField] private Button actionButton = default!;
        [SerializeField] private float updateInterval = 1f;

        private bool hasReceivedData = false;

        private List<string> CurrentDeckNames { get; set; } = new List<string>();

        void Start()
        {
            SetInitialDropdownState();

            actionButton.onClick.AddListener(OnButtonClick);

            InvokeRepeating(nameof(RequestDeckNames), 0f, updateInterval);
        }

        private void SetInitialDropdownState()
        {
            dropdownMenu.ClearOptions();
            dropdownMenu.AddOptions(new List<string> { "Loading..." });
            dropdownMenu.interactable = false;
        }

        private void RequestDeckNames()
        {
            AnkiConnectClientWrapper.GetDeckNames(this, UpdateDropdownMenu);
        }

        private void UpdateDropdownMenu(string[] strings)
        {
            var stringList = strings.ToList();

            if (CurrentDeckNames.SequenceEqual(stringList))
            {
                return;
            }
            CurrentDeckNames = stringList;

            hasReceivedData = true;

            dropdownMenu.ClearOptions();

            if (stringList.Count > 0)
            {
                dropdownMenu.AddOptions(stringList);
                dropdownMenu.interactable = true;
            }
            else
            {
                dropdownMenu.AddOptions(new List<string> { "No data available" });
                dropdownMenu.interactable = false;
            }
        }

        private void OnButtonClick()
        {
            if (!hasReceivedData || dropdownMenu.options.Count == 0)
            {
                Debug.LogWarning("Data has not been loaded yet!");
                return;
            }

            var deckName = dropdownMenu.options[dropdownMenu.value].text;

            AnkiConnectClientWrapper.OpenDeckReview(this, deckName, (result) => { });
        }
    }
}