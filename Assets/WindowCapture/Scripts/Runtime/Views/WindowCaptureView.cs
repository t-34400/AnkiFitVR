#nullable enable

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace WindowCapture
{
    public class WindowCaptureView : MonoBehaviour
    {
        [SerializeField] private RawImage targetImage = default!;
        [SerializeField] private string targetProcessName = "powershell";
        [SerializeField] private float updateInterval = 0.1f;
        [SerializeField] private float targetHeight = 200f;

        private Texture2D? targetTexture;
        private SynchronizationContext? mainContext;

        private WindowCaptureManager? captureManager;

        private bool IsCapturing { get; set; } = false;

        void Start()
        {
            mainContext = SynchronizationContext.Current;
            InvokeRepeating(nameof(CaptureWindow), 0f, updateInterval);
        }

        private async void CaptureWindow()
        {
            if (IsCapturing || mainContext == null)
            {
                return;
            }
            IsCapturing = true;

            try
            {
                if (captureManager == null || !captureManager.IsValidWindow)
                {
                    captureManager = new WindowCaptureManager(targetProcessName);
                }

                var convertFunc = await Task.Run(() =>
                {
                    return captureManager.CaptureWindow();
                });

                mainContext.Post(_ =>
                {
                    var texture = convertFunc(targetTexture);

                    if (targetImage != null)
                    {
                        targetImage.texture = texture;
                        targetImage.rectTransform.sizeDelta = new Vector2(
                            targetHeight * texture.width / texture.height,
                            targetHeight
                        );
                    }
                    
                    Debug.Log($"Captured window: ({texture.width}, {texture.height})");
                }, null);

                IsCapturing = false;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);

                captureManager = null;

                IsCapturing = false;
            }
        }        
    }
}
