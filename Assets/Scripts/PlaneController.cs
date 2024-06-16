using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaneController : MonoBehaviour
{
    public float forwardSpeed = 12f;
    public float pitchSpeed = 2f;
    public float fallSpeed = 1f;
    public float maxPitchAngle = 45f;
    public float fuel = 10f;
    public float fuelConsumptionRate = 1f;
    public TMP_Text fuelText;
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public TMP_Text gameOverScoreText;
    public TMP_Text gameOverReasonText;
    public GameObject ground;
    public GameObject background;
    public GameObject starPrefab;
    public GameObject fuelPrefab;
    public int numberOfStars = 3;
    public int numberOfFuel = 3;

    public Vector3 cameraOffset = new Vector3(20, 4, 7);

    private int score = 0;
    private float targetPitchAngle = 0f;
    private float timeAlive = 0f;
    private string gameOverReason;
    private bool gameIsOver = false;

    private void Start()
    {
        LoadScore();
        transform.position = new Vector3(0, 0, -30);
        transform.rotation = Quaternion.Euler(targetPitchAngle, 0, 0);
        Camera.main.transform.position = transform.position + cameraOffset;
        Camera.main.transform.rotation = Quaternion.Euler(10, -90, 0);
        Time.timeScale = 1;

        gameOverPanel.SetActive(false);

        SpawnRandomObjects(starPrefab, numberOfStars);
        SpawnRandomObjects(fuelPrefab, numberOfFuel);
    }

    private void Update()
    {
        if (gameIsOver) return;

        timeAlive += Time.deltaTime;
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (fuel <= 0 || (!Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0)))
        {
            targetPitchAngle = 45f;
        }
        else if (fuel > 0 && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)))
        {
            targetPitchAngle = -45f;
            fuel -= fuelConsumptionRate * Time.deltaTime;
        }

        float currentPitchAngle = transform.rotation.eulerAngles.x;
        currentPitchAngle = Mathf.LerpAngle(currentPitchAngle, targetPitchAngle, pitchSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentPitchAngle, 0, 0);

        Vector3 position = transform.position;
        position.x = 0;
        transform.position = position;

        if (transform.position.y <= -20)
        {
            Debug.Log("Ground");
            GameOver();
        }
        else if (fuel <= 0)
        {
            Debug.Log("Fuel");
            GameOver();
        }

        UpdateUI();
    }

    private void LateUpdate()
    {
        if (!gameIsOver)
        {
            FollowCamera();
        }
    }

    private void FollowCamera()
    {
        Camera.main.transform.position = transform.position + cameraOffset;
        Camera.main.transform.LookAt(transform.position);
        background.transform.position = new Vector3(transform.position.x, background.transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameIsOver) return;

        if (other.CompareTag("NordStar"))
        {
            fuel = Mathf.Min(fuel + 2f, 10f);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Fuel"))
        {
            score += 10;
            Destroy(other.gameObject);
        }
    }

    private void GameOver()
    {
        gameIsOver = true;
        Debug.Log("Game Over");
        SaveScore();
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = score > PlayerPrefs.GetInt("HighScore", 0) ? "New High Score: " + Mathf.FloorToInt(timeAlive).ToString() + " seconds" : "Time Survived: " + Mathf.FloorToInt(timeAlive).ToString() + " seconds";
        Time.timeScale = 0;
    }

    private void SaveScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (Mathf.FloorToInt(timeAlive) > highScore)
        {
            PlayerPrefs.SetInt("HighScore", Mathf.FloorToInt(timeAlive));
            PlayerPrefs.Save();
        }
    }

    private void LoadScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (scoreText != null)
        {
            scoreText.text = "High Score: " + highScore.ToString();
        }
    }

    private void UpdateUI()
    {
        if (fuelText != null)
        {
            fuelText.text = "Fuel: " + fuel.ToString("F0");
        }
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Mathf.FloorToInt(timeAlive).ToString();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        gameIsOver = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1;
        gameIsOver = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    private void SpawnRandomObjects(GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            float randomY = Random.Range(-20f, 50f);
            float randomZ = Random.Range(10f, 150f);
            Vector3 randomPosition = new Vector3(0, randomY, randomZ);
            Instantiate(prefab, randomPosition, Quaternion.identity);
        }
    }
}


