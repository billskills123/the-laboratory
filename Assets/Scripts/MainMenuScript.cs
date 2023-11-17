using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {
    [Header("Audio Objects")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private FadeAudioScript audioFadeScript;

    [Header("UI Objects")]
    [SerializeField] private Image mainMenu;
    [SerializeField] private Image optionsMenu;
    [SerializeField] private Image controlsMenu;
    [SerializeField] private Image creditsMenu;

    private void Start() {
        StartCoroutine(StartMenu());
    }

    private IEnumerator StartMenu() {
        audioFadeScript.AudioFade("Open", musicAudioSource, 1.0f, PlayerPrefs.GetFloat("MusicVolume") / 100);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(MoveUI("Open", mainMenu, Vector2.zero));
    }

    //Moves UI on and off the screen
    private IEnumerator MoveUI(string lerpType, Image uiObject, Vector2 endPosition) {
        if (lerpType == "Open") {
            endPosition = new Vector2(0, 0);
        } 
        else if (lerpType == "Close") {
            endPosition = new Vector2(0, -1050);
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
    }

    public void EndlessMode() {
        StartCoroutine(LoadEndlessMode());
    }

    //Loads into the endless mode
    private IEnumerator LoadEndlessMode() {
        StartCoroutine(MoveUI("Close", mainMenu, Vector2.zero));
        yield return new WaitUntil(() => mainMenu.GetComponent<RectTransform>().anchoredPosition.y <= -1050);

        audioFadeScript.AudioFade("Close", musicAudioSource, 1.0f, PlayerPrefs.GetFloat("MusicVolume") / 100);
        yield return new WaitUntil(() => musicAudioSource.volume == 0);
        SceneManager.LoadScene("EndlessMode");
    }

    public void OptionsButton() {
        StartCoroutine(OpenOptions());
    }

    //Closes the main menu and opens the option menu
    private IEnumerator OpenOptions() {
        StartCoroutine(MoveUI("Close", mainMenu, Vector2.zero));
        yield return new WaitUntil(() => mainMenu.GetComponent<RectTransform>().anchoredPosition.y <= -1050);

        mainMenu.gameObject.SetActive(false);
        optionsMenu.gameObject.SetActive(true);
        StartCoroutine(MoveUI("Open", optionsMenu, Vector2.zero));
    }

    public void CloseOptionsButton() {
        StartCoroutine(CloseOptions());
    }

    //Closes the options menu and opens the main menu
    private IEnumerator CloseOptions() {
        StartCoroutine(MoveUI("Close", optionsMenu, Vector2.zero));
        yield return new WaitUntil(() => optionsMenu.GetComponent<RectTransform>().anchoredPosition.y <= -1050);

        optionsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        StartCoroutine(MoveUI("Open", mainMenu, Vector2.zero));
    }

    public void ControlsButton() {
        StartCoroutine(OpenControls());
    }

    //Closes the main menu and opens the control menu
    private IEnumerator OpenControls() {
        StartCoroutine(MoveUI("Close", mainMenu, Vector2.zero));
        yield return new WaitUntil(() => mainMenu.GetComponent<RectTransform>().anchoredPosition.y <= -1050);

        mainMenu.gameObject.SetActive(false);
        controlsMenu.gameObject.SetActive(true);
        StartCoroutine(MoveUI("Open", controlsMenu, Vector2.zero));
    }

    public void CloseControlsButton() {
        StartCoroutine(CloseControls());
    }

    //Closes the control menu and opens the main menu
    private IEnumerator CloseControls() {
        StartCoroutine(MoveUI("Close", controlsMenu, Vector2.zero));
        yield return new WaitUntil(() => controlsMenu.GetComponent<RectTransform>().anchoredPosition.y <= -1050);

        controlsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        StartCoroutine(MoveUI("Open", mainMenu, Vector2.zero));
    }

    public void CreditsButton() {
        StartCoroutine(OpenCredits());
    }

    //Closes the main menu and opens the credits menu
    private IEnumerator OpenCredits() {
        StartCoroutine(MoveUI("Close", mainMenu, Vector2.zero));
        yield return new WaitUntil(() => mainMenu.GetComponent<RectTransform>().anchoredPosition.y <= -1050);

        mainMenu.gameObject.SetActive(false);
        creditsMenu.gameObject.SetActive(true);
        StartCoroutine(MoveUI("Open", creditsMenu, Vector2.zero));
    }

    public void CloseCreditsButton() {
        StartCoroutine(CloseCredits());
    }

    //Closes the credits menu and opens the main menu
    private IEnumerator CloseCredits() {
        StartCoroutine(MoveUI("Close", creditsMenu, Vector2.zero));
        yield return new WaitUntil(() => creditsMenu.GetComponent<RectTransform>().anchoredPosition.y <= -1050);

        creditsMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        StartCoroutine(MoveUI("Open", mainMenu, Vector2.zero));
    }

    //Closes the game
    public void CloseGame() {
        PlayerPrefs.Save();
        Application.Quit();
    }
}