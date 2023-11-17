using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour {
    [Header("UI Objects")]
    [SerializeField] private FadeScript canvasFadeScript;
    [SerializeField] private Image pauseMenu;
    [SerializeField] private Image optionsMenu;
    [SerializeField] private GameObject optionsCanvas;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject blackScreen;

    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    [Header("Audio Objects")]
    [SerializeField] private FadeAudioScript audioFadeScript;
    [SerializeField] private AudioSource musicAudioSource;

    public void OpenPauseMenu() {
        StartCoroutine(MovePauseMenu("Open", pauseMenu, Vector2.zero));
    }

    public void ClosePauseMenu() {
        StartCoroutine(ClosePauseMenuCoroutine());
    }

    private IEnumerator ClosePauseMenuCoroutine() {
        StartCoroutine(MovePauseMenu("Close", pauseMenu, Vector2.zero));
        yield return new WaitUntil(() => pauseMenu.GetComponent<RectTransform>().anchoredPosition.y >= 1020);
        gameManager.ClosePauseMenu();
    }

    //Move the pause menu on and off the screen
    private IEnumerator MovePauseMenu(string lerpType, Image uiObject, Vector2 endPosition) {
        if (lerpType == "Open") {
            endPosition = new Vector2(0, 0);
        } 
        else if (lerpType == "Close") {
            endPosition = new Vector2(0, 1020);
        }
        else if (lerpType == "OptionsClose") {
            endPosition = new Vector2(0, 1100);
        }
        else {
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

    public void ExitGame() {
        StartCoroutine(ExitGameCoroutine());
    }

    //Used for fading a black background onto the screen and then returnning back to the main menu
    private IEnumerator ExitGameCoroutine() {
        StartCoroutine(MovePauseMenu("Close", pauseMenu, Vector2.zero));
        yield return new WaitUntil(() => pauseMenu.GetComponent<RectTransform>().anchoredPosition.y >= 1020);

        startCanvas.SetActive(true);
        canvasFadeScript.CanvasFade("Open", blackScreen, 2.5f);
        yield return new WaitUntil(() => blackScreen.GetComponent<CanvasGroup>().alpha >= 1);

        audioFadeScript.AudioFade("Close", musicAudioSource, 1.0f, 1);
        yield return new WaitUntil(() => musicAudioSource.volume == 0);
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenOptionsMenu() {
        StartCoroutine(OpenOptionsMenuCoroutine());
    }

    private IEnumerator OpenOptionsMenuCoroutine() {
        StartCoroutine(MovePauseMenu("Close", pauseMenu, Vector2.zero));
        yield return new WaitUntil(() => pauseMenu.GetComponent<RectTransform>().anchoredPosition.y >= 1020);
        optionsCanvas.SetActive(true);
        StartCoroutine(MovePauseMenu("Open", optionsMenu, Vector2.zero));
    }

    public void CloseOptionsMenu() {
        StartCoroutine(CloseOptionsMenuCoroutine());
    }

    private IEnumerator CloseOptionsMenuCoroutine() {
        StartCoroutine(MovePauseMenu("OptionsClose", optionsMenu, Vector2.zero));
        yield return new WaitUntil(() => optionsMenu.GetComponent<RectTransform>().anchoredPosition.y >= 1100);
        optionsCanvas.SetActive(false);
        StartCoroutine(MovePauseMenu("Open", pauseMenu, Vector2.zero));
    }
}