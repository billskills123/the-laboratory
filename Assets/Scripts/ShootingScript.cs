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

    private void Start() {
        canFire = true;
        timer = 0;

        clipAmmoText.text = currentClipSize.ToString();
        maxAmmoText.text = storedAmmo.ToString();
    }

    private void Update() {
        if (gameManager.inGame == true) {
            mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 rotation = mousePos - bulletRotation.transform.position;
            float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            bulletRotation.transform.rotation = Quaternion.Euler(0, 0, rotZ);

            timer += Time.deltaTime;
            if (timer > timeBetweenFiring) {
                canFire = true;
                timer = 0;
            }
        }
    }

    private void OnShoot() {
        if(canFire == true && gameManager.inGame == true && currentClipSize != 0) {
            canFire = false;
            Instantiate(bullet, bulletTransform.position, Quaternion.identity);
            gunSounds[0].Play();
            currentClipSize--;
            clipAmmoText.text = currentClipSize.ToString();

            if (currentClipSize == 0 && storedAmmo > 0) {
                StartCoroutine(ReloadCoroutine());
            }
        }
        else if (canFire == true && gameManager.inGame == true && currentClipSize == 0) {
            gunSounds[2].Play();
        }
    }

    private void OnReload() {
        if (canFire == true && gameManager.inGame == true && storedAmmo > 0 && currentClipSize != clipSize) {
            StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine() {
        canFire = false;
        gunSounds[1].Play();

        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => gunSounds[1].isPlaying == false);

        int ammoDifference = clipSize - currentClipSize;

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