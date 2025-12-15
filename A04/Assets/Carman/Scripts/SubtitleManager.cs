using TMPro;
using UnityEngine;
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance;

    [SerializeField] private TextMeshProUGUI subtitleText;

    private Coroutine subtitleCoroutine;

    private void Awake()
    {
        Instance = this;
        subtitleText.text = "";
    }

    public void ShowSubtitle(string text, float duration)
    {
        if (subtitleCoroutine != null)
            StopCoroutine(subtitleCoroutine);

        subtitleCoroutine = StartCoroutine(SubtitleRoutine(text, duration));
    }

    public void ClearSubtitle()
    {
        if (subtitleCoroutine != null)
            StopCoroutine(subtitleCoroutine);

        subtitleText.text = "";
    }

    private IEnumerator SubtitleRoutine(string text, float duration)
    {
        subtitleText.text = text;
        yield return new WaitForSeconds(duration);
        subtitleText.text = "";
    }
}
