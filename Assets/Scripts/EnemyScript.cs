using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class EnemyScript : MonoBehaviour {
    [Header("Enemy Settings")]
    [SerializeField] private float enemyHealth;
    [SerializeField] private float enemyAttackCooldown;
    [SerializeField] private bool canRemoveHealth;

    [Header("Enemy Movement")]
    [SerializeField] private float maxRotateSpeed;
    [SerializeField] private float enemySpeed;
    private float angle;
    private float currentVelocityFloat;

    [Header("Enemy Objects")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject enemySprite;
    [SerializeField] private AudioSource deathAudioSource;
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private ParticleSystem enemyParticles;

    [Header("Misc Objects")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private OptionsScript optionsScript;
    [SerializeField] private GameObject player;

    [Header("Pickup Objects")]
    [SerializeField] private GameObject pickupsParent;
    [SerializeField] private GameObject ammoPickup;
    [SerializeField] private GameObject healthPickup;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            StartCoroutine(RemovePlayerHealth());
        }
        else if (collision.gameObject.CompareTag("Bullet")) {
            if (enemyHealth > 0) {
                enemyHealth -= 50f;

                if(enemyHealth == 0) {
                    deathAudioSource.Play();
                    StartCoroutine(Death());
                } 
                else {
                    enemy.GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            StartCoroutine(RemovePlayerHealth());
        }
    }

    private IEnumerator RemovePlayerHealth() {
        if (gameManager.playerHealth > 0 && canRemoveHealth == true) {
            player.GetComponent<AudioSource>().Play();
            canRemoveHealth = false;
            gameManager.playerHealth -= 20f;
            gameManager.UpdateHealthSlider("Red");

            if (gameManager.playerHealth == 0) {
                gameManager.PlayerDeath();
            }

            yield return new WaitForSeconds(enemyAttackCooldown);
            canRemoveHealth = true;
        }
    }

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        optionsScript = GameObject.Find("GameManager").GetComponent<OptionsScript>();
        player = GameObject.Find("Player");
        pickupsParent = GameObject.Find("Pickups");
    }

    private void Start() {
        gameManager.AddSound(hitAudioSource);
        gameManager.AddSound(deathAudioSource);

        hitAudioSource.volume = (optionsScript.sfxVolume * (optionsScript.masterVolumeSlider.value / 100)) / 100;
        deathAudioSource.volume = (optionsScript.sfxVolume * (optionsScript.masterVolumeSlider.value / 100)) / 100;
    }

    private void Update() {
        if (enemyHealth != 0 && gameManager.inGame == true) {
            EnemyMovement();
            EnemyRotation();
        }
    }

    private void EnemyMovement() {
        float step = enemySpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
    }

    private void EnemyRotation() {
        Vector3 playerPosition = player.transform.position;
        Vector3 direction = playerPosition - transform.position;
        float targetAngle = Vector2.SignedAngle(Vector2.right, direction);
        angle = Mathf.SmoothDampAngle(angle, targetAngle, ref currentVelocityFloat, 0.3f, maxRotateSpeed);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private IEnumerator Death() {
        enemyParticles.Play();

        enemy.GetComponent<Collider2D>().enabled = false;
        enemySprite.GetComponent<SpriteRenderer>().enabled = false;

        int randomNumber = Random.Range(1, 10);

        if (randomNumber <= 3) {
            Instantiate(ammoPickup, enemy.transform.position, Quaternion.identity, pickupsParent.transform);
        }
        else if (randomNumber >= 4 && randomNumber <= 7) {
            Instantiate(healthPickup, enemy.transform.position, Quaternion.identity, pickupsParent.transform);
        }

        gameManager.AddScore();
        gameManager.enemiesActive--;
        gameManager.RemoveSound(hitAudioSource);
        gameManager.RemoveSound(deathAudioSource);

        yield return new WaitUntil(() => enemyParticles.isPlaying == false);

        Destroy(gameObject);
        gameManager.enemyList.Remove(enemy);
    }
}