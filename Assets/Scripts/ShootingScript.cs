using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingScript : MonoBehaviour {
    [Header("Objects")]
    [SerializeField] private GameObject bulletRotation;
    [SerializeField] private Transform bulletTransform;
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioSource[] gunSounds;

    [Header("Gun Settings")]
    [SerializeField] private float timeBetweenFiring;
    public int maxAmmo;
    public int storedAmmo;
    [SerializeField] private int clipSize;
    [SerializeField] private int currentClipSize;

    [Header("UI Objects")]
    [SerializeField] private TMP_Text clipAmmoText;
    public TMP_Text maxAmmoText;

    private Vector3 mousePos;
    private float timer;
    private bool canFire;

    //Set up default values
    private void Start() {
        canFire = true;
        timer = 0;

        clipAmmoText.text = currentClipSize.ToString();
        maxAmmoText.text = storedAmmo.ToString();
    }

    //Aim the gun towards the mouse
    private void Update() {
        if (gameManager.inGame == true) {
            mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 rotation = mousePos - bulletRotation.transform.position;
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            bulletRotation.transform.rotation = Quaternion.Euler(0, 0, rotZ);

            //Timer used for ensuring there is a delay between player shots
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring) {
                canFire = true;
                timer = 0;
            }
        }
    }

    //Called when the player presses the shoot button
    private void OnShoot() {
        if(canFire == true && gameManager.inGame == true && currentClipSize != 0) {
            canFire = false;
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);
            gunSounds[0].Play();
            currentClipSize--;
            clipAmmoText.text = currentClipSize.ToString();

            if (currentClipSize == 0 && storedAmmo > 0) {
                StartCoroutine(ReloadCoroutine()); //Automatically reload the gun if the last bullet was fired and there is available ammo
            }
        }
        else if (canFire == true && gameManager.inGame == true && currentClipSize == 0) {
            gunSounds[2].Play(); //Plays a blank fire sound if the current clip is empty
        }
    }

    private void OnReload() {
        if (canFire == true && gameManager.inGame == true && storedAmmo > 0 && currentClipSize != clipSize) {
            StartCoroutine(ReloadCoroutine()); //Reload the gun only if there is available ammo and the clip is not full
        }
    }

    private IEnumerator ReloadCoroutine() {
        canFire = false;
        gunSounds[1].Play();

        yield return new WaitForSeconds(0.5f); //Adds a little delay into the reloading process -- Makes sure reloads aren't instant
        yield return new WaitUntil(() => gunSounds[1].isPlaying == false);

        int ammoDifference = clipSize - currentClipSize;

        //Either completely reload the gun if possible if not use all remaining bullets
        if (ammoDifference > 0 && storedAmmo >= ammoDifference) {
            storedAmmo -= ammoDifference;
            currentClipSize += ammoDifference;
        }
        else if (ammoDifference > 0 && storedAmmo < ammoDifference) {
            currentClipSize += storedAmmo;
            storedAmmo = 0;
        }

        clipAmmoText.text = currentClipSize.ToString();
        maxAmmoText.text = storedAmmo.ToString();

        canFire = true;
    }
}