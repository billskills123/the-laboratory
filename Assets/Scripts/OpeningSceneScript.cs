using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OpeningSceneScript : MonoBehaviour {
    [Header("Objects")]
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private FadeScript canvasFadeScript;
    [SerializeField] private FadeAudioScript audioFadeScript;
    private bool loaded = false;
    private bool funcCalled = false;

    //Fade in the music
    void Start() {
        audioFadeScript.AudioFade("Open", audioSource, 2.0f, PlayerPrefs.GetFloat("MusicVolume") / 100);
        StartCoroutine(OpenCloseLogo());
    }

    //Fade the logo onto the screen, fade it out, and then fade in the title
    private IEnumerator OpenCloseLogo() {
        yield return new WaitForSeconds(0.75f);
        canvasFadeScript.CanvasFade("Open", logo, 1.0f);

        yield return new WaitUntil(() => logo.GetComponent<CanvasGroup>().alpha == 1);
        yield return new WaitForSeconds(1.5f);

        canvasFadeScript.CanvasFade("Close", logo, 1.0f);
        yield return new WaitUntil(() => logo.GetComponent<CanvasGroup>().alpha == 0);
        yield return new WaitForSeconds(0.75f);

        logo.SetActive(false);
        titleScreen.SetActive(true);

        canvasFadeScript.CanvasFade("Open", titleScreen, 1.0f);
        yield return new WaitUntil(() => titleScreen.GetComponent<CanvasGroup>().alpha == 1);
        loaded = true;
    }

    //Check if a player has pressed any key
    private void Update() {
        if (Keyboard.current.anyKey.isPressed && loaded == true && funcCalled == false || Mouse.current.IsPressed() && loaded == true && funcCalled == false) {
            funcCalled = true; //Stops repeatedly spamming the function
            StartCoroutine(LoadMainMenu());
        }
    }

    //Loads in the main menu
    private IEnumerator LoadMainMenu() {
        canvasFadeScript.CanvasFade("Close", titleScreen, 1.0f);
        audioFadeScript.AudioFade("Close", audioSource, 1.0f, 1);
        yield return new WaitUntil(() => titleScreen.GetComponent<CanvasGroup>().alpha == 0 && audioSource.volume == 0);
        SceneManager.LoadScene("MainMenu");
    }
}