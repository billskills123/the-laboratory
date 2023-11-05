using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour {
    [Header("Fade Settings")]
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private bool fadeIn = false;
    [SerializeField] private float fadeSpeed;
    private GameObject player;

    //Sets up the settings to start the fade
    public void CanvasFade(string fadeType, GameObject fadeObject, float speed) {
        if (fadeType == "Open") {
            fadeOut = false;
            fadeIn = true;
            player = fadeObject;
            fadeSpeed = speed;
        }
        else if (fadeType == "Close") {
            fadeIn = false;
            fadeOut = true;
            player = fadeObject;
            fadeSpeed = speed;
        }
    }

    private void Update() {
        //Fades the player canvas In
        if (fadeIn == true && fadeOut == false) {
            if (player.GetComponent<CanvasGroup>().alpha <= 1) {
                player.GetComponent<CanvasGroup>().alpha += Time.deltaTime * fadeSpeed;

                if (player.GetComponent<CanvasGroup>().alpha == 1) {
                    fadeIn = false;
                }
            }
        }

        //Fades the player canvas out
        if (fadeOut == true && fadeIn == false) {
            if (player.GetComponent<CanvasGroup>().alpha >= 0) {
                player.GetComponent<CanvasGroup>().alpha -= Time.deltaTime * fadeSpeed;

                if (player.GetComponent<CanvasGroup>().alpha == 0) {
                    fadeOut = false;
                }
            }
        }
    }
}