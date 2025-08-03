using UnityEngine;
using System.Collections;

public class RapidFireUI : MonoBehaviour
{
    [SerializeField] private RectTransform bar; // UI image that shrinks over time
    [SerializeField] private bool hideWhenIdle = true; // hide UI when inactive

    private float fullWidth; // full image width
    private Coroutine timerRoutine;   // Reference to running coroutine

    private void Awake()
    {
        if (bar == null) bar = GetComponent<RectTransform>(); // Auto-assign if not set
        fullWidth = bar.sizeDelta.x; // Cache original width

        if (hideWhenIdle) gameObject.SetActive(false); //  hide on start
    }

    // Call this to start the countdown UI
    public void StartTimer(float duration)
    {
        if (timerRoutine != null) StopCoroutine(timerRoutine); // Stop any existing timer
        gameObject.SetActive(true); // Show UI
        timerRoutine = StartCoroutine(Timer(duration)); // Start new timer
    }

    // shrinking the image over the duration
    private IEnumerator Timer(float duration)
    {
        float time = duration;
        while (time > 0f)
        {
            time -= Time.deltaTime;
            float percentTime = Mathf.Clamp01(time / duration);  // % of time left

            var size = bar.sizeDelta;
            size.x = fullWidth * percentTime;  // Update width
            bar.sizeDelta = size;

            yield return null;
        }

        // Ensure width is 0 at the end
        var finalSize = bar.sizeDelta;
        finalSize.x = 0f;
        bar.sizeDelta = finalSize;

        if (hideWhenIdle) gameObject.SetActive(false); // Hide UI
        timerRoutine = null;
    }
}
