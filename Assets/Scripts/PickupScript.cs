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

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        optionsScript = GameObject.Find("GameManager").GetComponent<OptionsScript>();
        shootingScript = GameObject.Find("PlayerObject").GetComponent<ShootingScript>();
    }

    private void Start() {
        gameManager.AddSound(pickupSound);
        pickupSound.volume = (optionsScript.sfxVolume * (optionsScript.masterVolumeSlider.value / 100)) / 100;
    }

    private void Update() {
        itemLife -= Time.deltaTime;

        if (itemLife <= 0) {
            Destroy(gameObject);
        }
    }

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
            shootingScript.storedAmmo += ammoToAdd;

            if (shootingScript.storedAmmo > shootingScript.maxAmmo) {
                shootingScript.storedAmmo = shootingScript.maxAmmo;
            }

            shootingScript.maxAmmoText.text = shootingScript.storedAmmo.ToString();
        }
        else if (pickupType == "Health") {
            gameManager.playerHealth = 100f;
            gameManager.UpdateHealthSlider("Green");
        }

        yield return new WaitUntil(() => pickupParticles.isPlaying == false);
        Destroy(gameObject);
    }
}