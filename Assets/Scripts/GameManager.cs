using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    [Header("Player Attributes")]
    [SerializeField] private GameObject player;
    [SerializeField] private ParticleSystem playerParticles;
    public float playerHealth;
    public int score;

    [Header("Start UI")]
    [SerializeField] private FadeScript startCanvasFadeScript;
    [SerializeField] private GameObject startBackground;
    [SerializeField] private GameObject startText;
    [SerializeField] private GameObject startCanvas;

    [Header("Main UI")]
    [SerializeField] private FadeScript mainCanvasFadeScript;
    [SerializeField] private GameObject damageFilter;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject scoreUI;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject healthSliderFill; //Gets set inactive when the player dies

    [Header("Death UI")]
    [SerializeField] private GameObject deathCanvas;
    [SerializeField] private GameObject deathCanvasBackground;
    [SerializeField] private FadeScript deathCanvasFadeScript;
    [SerializeField] private Image deathUI;
    [SerializeField] private TMP_Text deathScoreText;
    [SerializeField] private TMP_Text deathTimerText;
    [SerializeField] private DeathMenuScript deathMenuScript;

    [Header("Player Death UI")]
    [SerializeField] private GameObject playerDeathCanvas;
    [SerializeField] private FadeScript playerDeathCanvasFadeScript;
    [SerializeField] private GameObject deathBackground;
    [SerializeField] private GameObject deathBlackBackground;

    [Header("Pause UI")]
    [SerializeField] private PauseMenuScript pauseMenuScript;
    [SerializeField] private GameObject pauseMenuCanvas;

    [Header("Audio Objects")]
    [SerializeField] private FadeAudioScript audioFadeScript;
    [SerializeField] private AudioSource gameMusic;
    [SerializeField] private OptionsScript optionsScript;

    [Header("Enemy Settings")]
    public List<GameObject> enemyList = new List<GameObject>();
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemyParent;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private int activeEnemies;
    [SerializeField] private Vector3 enemySpawnLocation;
    public float enemiesActive;

    [Header("Game Settings")]
    public bool inGame;
    [SerializeField] private bool timerEnabled;
    [SerializeField] private float timer;

    private void Start() {
        StartCoroutine(LoadEndlessMode());
    }

    private IEnumerator LoadEndlessMode() {
        yield return new WaitForSeconds(1.5f);
        audioFadeScript.AudioFade("Open", gameMusic, 2.5f, PlayerPrefs.GetFloat("MusicVolume") / 100);
        startCanvasFadeScript.CanvasFade("Close", startBackground, 2.5f);
        yield return new WaitUntil(() => startBackground.GetComponent<CanvasGroup>().alpha == 0);
        yield return new WaitForSeconds(2f);

        startCanvasFadeScript.CanvasFade("Close", startText, 2.5f);
        yield return new WaitUntil(() => startText.GetComponent<CanvasGroup>().alpha == 0);

        startCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        mainCanvasFadeScript.CanvasFade("Open", mainCanvas, 2.5f);

        inGame = true;
        timerEnabled = true;
        timer = 0.0f;
    }

    public void Update() {
        SpawnEnemies();

        if (timerEnabled) {
            timer += Time.deltaTime;
        }

        if (Keyboard.current.escapeKey.isPressed == true && inGame == true) {
            StartCoroutine(OpenPauseMenuCoroutine());
        }
    }

    public void UpdateHealthSlider(string colorText) {
        healthSlider.value = playerHealth;
        StartCoroutine(FlashDamageFilter(colorText));
    }

    private IEnumerator FlashDamageFilter(string colorText) {
        damageFilter.SetActive(true);

        if (colorText == "Green") {
            damageFilter.GetComponent<Image>().color = new Color(0, 255, 0, 0.05f);
        }
        else if(colorText == "Red") {
            damageFilter.GetComponent<Image>().color = new Color(255, 0, 0, 0.05f);
        }
        mainCanvasFadeScript.CanvasFade("Open", damageFilter, 3.0f);
        yield return new WaitUntil(() => damageFilter.GetComponent<CanvasGroup>().alpha == 1);
        mainCanvasFadeScript.CanvasFade("Close", damageFilter, 3.0f);
        yield return new WaitUntil(() => damageFilter.GetComponent<CanvasGroup>().alpha == 0);
        damageFilter.SetActive(false);
    }

    public void AddScore() {
        score++;
        scoreUI.GetComponent<TMP_Text>().text = "Score: " + score.ToString();
    }

    public void PlayerDeath() {
        StartCoroutine(PlayerDeathCoroutine());
    }

    private IEnumerator PlayerDeathCoroutine() {
        healthSliderFill.SetActive(false);
        inGame = false;
        timerEnabled = false;

        StartCoroutine(CloseMainUI());
        playerParticles.Play();
        yield return new WaitUntil(() => playerParticles.isPlaying == false);

        foreach (var enemy in enemyList) {
            Destroy(enemy);
            enemiesActive--;
        }
        enemyList.Clear();
    }

    private IEnumerator CloseMainUI() {
        mainCanvasFadeScript.CanvasFade("Close", mainCanvas, 5f);
        yield return new WaitUntil(() => mainCanvas.GetComponent<CanvasGroup>().alpha == 0);
        mainCanvas.SetActive(false);

        StartCoroutine(PlayerDeathUI());
    }

    private IEnumerator PlayerDeathUI() {
        playerDeathCanvas.SetActive(true);
        playerDeathCanvasFadeScript.CanvasFade("Open", deathBlackBackground, 5f);
        yield return new WaitUntil(() => deathBlackBackground.GetComponent<CanvasGroup>().alpha == 1);

        playerDeathCanvasFadeScript.CanvasFade("Open", deathBackground, 2.5f);
        yield return new WaitUntil(() => deathBackground.GetComponent<CanvasGroup>().alpha == 1);

        playerDeathCanvasFadeScript.CanvasFade("Close", deathBackground, 1f);
        yield return new WaitUntil(() => deathBackground.GetComponent<CanvasGroup>().alpha == 0);
        deathCanvas.SetActive(true);
        playerDeathCanvas.SetActive(false);

        OpenDeathUI();
    }

    private void OpenDeathUI() {
        deathScoreText.text = "Score: " + score;
        float minutes = Mathf.FloorToInt(timer / 60);
        float seconds = Mathf.FloorToInt(timer % 60);

        if (minutes == 0) {
            deathTimerText.text = "Time Taken: " + seconds + " seconds";
        }
        else if (minutes == 1) {
            deathTimerText.text = "Time Taken: " + minutes + " minute " + seconds + " seconds";
        }
        else {
            deathTimerText.text = "Time Taken: " + minutes + " minutes " + seconds + " seconds";
        }

        deathMenuScript.OpenDeathMenu(deathUI);
    }

    private void SpawnEnemies() {
        if (enemiesActive < enemiesToSpawn && inGame == true) {
            enemiesActive++;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy() {
        GameObject tempEnemy = Instantiate(enemyPrefab, new Vector3(player.transform.position.x + Random.Range(-30f, 30f), player.transform.position.y + Random.Range(-30f, 30f), player.transform.position.z), Quaternion.identity, enemyParent.transform);
        enemyList.Add(tempEnemy);

        if (Vector3.Distance(tempEnemy.transform.position, player.transform.position) < 10f){
            Destroy(tempEnemy);
            enemiesActive--;
            enemyList.Remove(tempEnemy);
        }
    }

    public void AddSound(AudioSource audioSource) {
        optionsScript.sfxSources.Add(audioSource);
    }

    public void RemoveSound(AudioSource audioSource) {
        optionsScript.sfxSources.Remove(audioSource);
    }

    private IEnumerator OpenPauseMenuCoroutine() {
        inGame = false;
        timerEnabled = false;

        mainCanvasFadeScript.CanvasFade("Close", mainCanvas, 2.5f);
        yield return new WaitUntil(() => mainCanvas.GetComponent<CanvasGroup>().alpha == 0);
        mainCanvas.SetActive(false);

        pauseMenuCanvas.SetActive(true);
        pauseMenuScript.OpenPauseMenu();
    }

    public void ClosePauseMenu() {
        StartCoroutine(ClosePauseMenuCoroutine());
    }

    public IEnumerator ClosePauseMenuCoroutine() {
        pauseMenuCanvas.SetActive(false);
        mainCanvas.SetActive(true);

        mainCanvasFadeScript.CanvasFade("Open", mainCanvas, 2.5f);
        yield return new WaitUntil(() => mainCanvas.GetComponent<CanvasGroup>().alpha == 0);
        inGame = true;
        timerEnabled = true;
    }
}