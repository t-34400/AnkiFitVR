#nullable enable

using UnityEngine;
using UnityEngine.Events;

namespace WindowCapture
{
    public class WindowNameExtractor : MonoBehaviour
    {
        [SerializeField] private float updateInterval = 1f;
        [SerializeField] private string targetProcessName = "powershell";
        [SerializeField] private UnityEvent<string> onWindowNameExtracted = default!;

        private string LatestWindowName { get; set; } = string.Empty;

        void Start()
        {
            InvokeRepeating(nameof(CaptureWindow), 0f, updateInterval);
        }

        private void CaptureWindow()
        {
            var windowNames = WindowLister.GetWindowNames();
            foreach (var (windowName, processName) in windowNames)
            {
                if (processName == targetProcessName)
                {
                    if (LatestWindowName == windowName)
                    {
                        return;
                    }
                    LatestWindowName = windowName;
                    
                    onWindowNameExtracted.Invoke(windowName);
                    return;
                }
            }
        }
    }
}
