using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathMenuScript : MonoBehaviour {
    [Header("UI Objects")]
    [SerializeField] private Image deathUI;

    [Header("Audio Objects")]
    [SerializeField] private FadeAudioScript audioFadeScript;
    [SerializeField] private AudioSource musicAudioSource;

    public void ReturnToMenu() {
        StartCoroutine(ReturnToMenuCoroutine());
    }

    //Returns back to the main menu
    private IEnumerator ReturnToMenuCoroutine() {
        StartCoroutine(MoveDeathUI("Close", deathUI, Vector2.zero));
        yield return new WaitUntil(() => deathUI.GetComponent<RectTransform>().anchoredPosition.y >= 1020);

        audioFadeScript.AudioFade("Close", musicAudioSource, 1.0f, 1);
        yield return new WaitUntil(() => musicAudioSource.volume == 0);
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame() {
        StartCoroutine(RestartGameCoroutine());
    }

    //Restarts the game
    private IEnumerator RestartGameCoroutine() {
        StartCoroutine(MoveDeathUI("Close", deathUI, Vector2.zero));
        yield return new WaitUntil(() => deathUI.GetComponent<RectTransform>().anchoredPosition.y >= 1020);

        audioFadeScript.AudioFade("Close", musicAudioSource, 1.0f, 1);
        yield return new WaitUntil(() => musicAudioSource.volume == 0);
        SceneManager.LoadScene("EndlessMode");
    }

    public void OpenDeathMenu(Image deathUI) {
        StartCoroutine(MoveDeathUI("Open", deathUI, Vector2.zero));
    }

    public void CloseDeathMenu(Image deathUI) {
        StartCoroutine(MoveDeathUI("Close", deathUI, Vector2.zero));
    }

    //Used for moving the death UI on and off the screen
    private IEnumerator MoveDeathUI(string lerpType, Image uiObject, Vector2 endPosition) {
        if (lerpType == "Open") {
            endPosition = new Vector2(0, 0);
        } else if (lerpType == "Close") {
            endPosition = new Vector2(0, 1020);
        } else {
            Debug.LogError("Error. lerpType not set up properly");
        }

        //Create Variables
        Vector2 startPosition = uiObject.rectTransform.anchoredPosition;
        float time = 0;

        //Lerp to actually move the UI
        while (time < 1.0f) {
            uiObject.rectTransform.anchoredPosition = LerpLibrary.UILerp(startPosition, endPosition, LerpLibrary.InOutBackEase(time));
            time += Time.deltaTime;
            yield return null;
        }

        uiObject.rectTransform.anchoredPosition = endPosition;
        yield return new WaitUntil(() => uiObject.rectTransform.anchoredPosition == endPosition);
    }
}