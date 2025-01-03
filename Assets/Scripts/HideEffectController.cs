#nullable enable

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HideEffectController : MonoBehaviour
{
    [SerializeField] private Image targetImage = default!;
    [SerializeField] private float startHideTime = 0.8f;
    [SerializeField] private float transitionTime = 0.2f;

    [ContextMenu("Show")]
    public void Show()
    {
        var currentColor = targetImage.color;
        targetImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);

        StopAllCoroutines();
        StartCoroutine(Hide());        
    }

    private void Start()
    {
        var currentColor = targetImage.color;
        targetImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
    }

    private IEnumerator Hide()
    {
        yield return new WaitForSeconds(startHideTime);

        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / transitionTime;
            var currentColor = targetImage.color;
            targetImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1 - t);
            yield return null;
        }
    }
}
