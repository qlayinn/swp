using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlaneController : MonoBehaviour
{
    public float forwardSpeed = 12f; // Скорость движения вперед
    public float pitchSpeed = 2f; // Скорость изменения тангажа
    public float fallSpeed = 1f; // Скорость падения
    public float maxPitchAngle = 45f; // Максимальный угол тангажа
    public float fuel = 10f; // Запас топлива
    public float fuelConsumptionRate = 1f; // Расход топлива при подъеме
    public TMP_Text fuelText;
    public TMP_Text scoreText;
    public GameObject gameOverPanel; // Панель "Game Over"
    public TMP_Text gameOverScoreText; // Текст для отображения счета в панели "Game Over"
    public TMP_Text gameOverReasonText; // Текст для отображения причины поражения
    public GameObject ground; // Объект земли
    public GameObject background; // Объект фона
    public GameObject starPrefab; // Префаб звезды
    public GameObject fuelPrefab; // Префаб топлива
    public int numberOfStars = 3; // Количество звезд для создания
    public int numberOfFuel = 3; // Количество топлива для создания

    public Vector3 cameraOffset = new Vector3(20, 4, 7); // Смещение камеры

    private int score = 0;
    private float targetPitchAngle = 0f; // Целевой угол тангажа
    private float timeAlive = 0f; // Время, которое игрок продержался
    private string gameOverReason; // Причина поражения
    private bool gameIsOver = false; // Флаг, обозначающий окончание игры

    private void Start()
    {
        LoadScore();
        transform.position = new Vector3(0, 0, -30); // Позиция самолета при старте
        transform.rotation = Quaternion.Euler(targetPitchAngle, 0, 0);
        Camera.main.transform.position = transform.position + cameraOffset;
        Camera.main.transform.rotation = Quaternion.Euler(10, -90, 0);
        Time.timeScale = 1; // Убедиться, что время идет

        gameOverPanel.SetActive(false); // Отключить панель "Game Over" в начале

        SpawnRandomObjects(starPrefab, numberOfStars); // Создание звезд
        SpawnRandomObjects(fuelPrefab, numberOfFuel); // Создание топлива
    }



    private void Update()
    {
        if (gameIsOver) return; // Не выполнять код, если игра окончена

        // Увеличение времени выживания
        timeAlive += Time.deltaTime;

        // Движение вперед
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Падение самолета под углом 45 градусов, если не нажата клавиша
        if (fuel <= 0 || (!Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0)))
        {
            targetPitchAngle = 45f;
        }
        else if (fuel > 0 && (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)))
        {
            targetPitchAngle = -45f;
            fuel -= fuelConsumptionRate * Time.deltaTime;
        }

        // Плавное изменение угла тангажа
        float currentPitchAngle = transform.rotation.eulerAngles.x;
        currentPitchAngle = Mathf.LerpAngle(currentPitchAngle, targetPitchAngle, pitchSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentPitchAngle, 0, 0);

        Vector3 position = transform.position;
        position.x = 0;
        transform.position = position;

        // Проверка на столкновение с землей
        if (transform.position.y <=-20)
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

        // Движение фона вместе с самолетом
        background.transform.position = new Vector3(transform.position.x, background.transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameIsOver) return; // Не выполнять код, если игра окончена

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
        /*else if (other.CompareTag("Rock"))
        {
            gameOverReason = "Collided with an obstacle";
            GameOver();
        }*/
    }

    private void GameOver()
    {
        gameIsOver = true;
        Debug.Log("Game Over");
        SaveScore();
        gameOverPanel.SetActive(true);
        gameOverScoreText.text = score > PlayerPrefs.GetInt("HighScore", 0) ? "New High Score: " + Mathf.FloorToInt(timeAlive).ToString() + " seconds" : "Time Survived: " + Mathf.FloorToInt(timeAlive).ToString() + " seconds";
        //gameOverReasonText.text = "Reason: " + gameOverReason;
        Time.timeScale = 0; // Остановить время
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
            scoreText.text = "Score: " + Mathf.FloorToInt(timeAlive).ToString(); // Отображение времени выживания в секундах
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Возобновить время
        gameIsOver = false;
        // Перезагрузить сцену или сбросить все параметры к начальному состоянию
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1; // Возобновить время
        gameIsOver = false;
        // Перейти в главное меню (загрузить сцену главного меню)
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

