using System.Collections;
using UnityEngine;

public class PickupScript : MonoBehaviour {
    [Header("Pickup Information")]
    [SerializeField] private string pickupType;
    [SerializeField] private float itemLife;

    [Header("Pickup Attributes")]
    [SerializeField] private int ammoToAdd;

    [Header("Additional Scripts")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private OptionsScript optionsScript;
    [SerializeField] private ShootingScript shootingScript;

    [Header("Misc Objects")]
    [SerializeField] private ParticleSystem pickupParticles;
    [SerializeField] private AudioSource pickupSound;
    [SerializeField] private SpriteRenderer spriteRenderer;

    //Set up default variables
    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        optionsScript = GameObject.Find("GameManager").GetComponent<OptionsScript>();
        shootingScript = GameObject.Find("PlayerObject").GetComponent<ShootingScript>();
    }

    //Set up audio
    private void Start() {
        gameManager.AddSound(pickupSound);
        pickupSound.volume = (optionsScript.sfxVolume * (optionsScript.masterVolumeSlider.value / 100)) / 100;
    }

    //Decrease the items life over time
    private void Update() {
        itemLife -= Time.deltaTime;

        if (itemLife <= 0) {
            Destroy(gameObject);
        }
    }

    //Detect if the player has collided with the object
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            StartCoroutine(HitCoroutine());
        }
    }

    private IEnumerator HitCoroutine() {
        spriteRenderer.enabled = false;
        pickupSound.Play();
        pickupParticles.Play();

        if (pickupType == "Ammo") {
            shootingScript.storedAmmo += ammoToAdd; //Add ammo

            //If the ammo is above the maximum ammo reset it back down to max ammo
            if (shootingScript.storedAmmo > shootingScript.maxAmmo) {
                shootingScript.storedAmmo = shootingScript.maxAmmo;
            }

            shootingScript.maxAmmoText.text = shootingScript.storedAmmo.ToString();
        }
        else if (pickupType == "Health") {
            gameManager.playerHealth = 100f; //Reset the health back to full
            gameManager.UpdateHealthSlider("Green"); //Flash a green filter on the screen
        }

        yield return new WaitUntil(() => pickupParticles.isPlaying == false);
        Destroy(gameObject);
    }
}