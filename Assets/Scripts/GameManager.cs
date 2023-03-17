using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int rows = 30;
    public int cols = 40;
    public float speed = 3f;
    public int numOfSnakes = 4;
    public int startLengthOfSnake = 4;
    public List<Color> snakeColors;

    private List<KeySet> keySets;
    private List<MapPiece> availableMapPieces;
    private int totalSnakes;
    private MoveDirection startDirection;

    [SerializeField] private Camera camera;
    [SerializeField] private Transform wallSpawnParent;
    [SerializeField] private Transform wallPrefab;
    [SerializeField] private Snake snakePrefab;
    [SerializeField] private Transform applePrefab;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private GameObject gameOverText;

    void Start()
    {
        keySets = new List<KeySet>
        {
            new KeySet(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D),
            new KeySet(KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow),
            new KeySet(KeyCode.T, KeyCode.G, KeyCode.F, KeyCode.H),
            new KeySet(KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L)
        };

        startDirection = MoveDirection.Left;
        camera.orthographicSize = rows > cols ? rows / 2 + 1 : cols / 2 + 1;
        InitializeWall();
        InitializeMap();
        InitializeSnakes();
        SpawnApple();
    }

    private void InitializeWall()
    {
        Transform t = Instantiate(wallPrefab, new Vector3(-cols / 2 - 1, 0, 0), Quaternion.identity, wallSpawnParent);
        t.localScale = new Vector3(1, rows + 3, 1);
        t.GetComponent<BoxCollider>().size = new Vector3(0.5f, 1, 1);
        t = Instantiate(wallPrefab, new Vector3(cols / 2 + 1, 0, 0), Quaternion.identity, wallSpawnParent);
        t.localScale = new Vector3(1, rows + 3, 1);
        t.GetComponent<BoxCollider>().size = new Vector3(0.5f, 1, 1);
        t = Instantiate(wallPrefab, new Vector3(0, -rows / 2 - 1, 0), Quaternion.identity, wallSpawnParent);
        t.localScale = new Vector3(cols + 3, 1, 1);
        t.GetComponent<BoxCollider>().size = new Vector3(1, 0.5f, 1);
        t = Instantiate(wallPrefab, new Vector3(0, rows / 2 + 1, 0), Quaternion.identity, wallSpawnParent);
        t.localScale = new Vector3(cols + 3, 1, 1);
        t.GetComponent<BoxCollider>().size = new Vector3(1, 0.5f, 1);
    }

    private void InitializeMap()
    {
        availableMapPieces = new List<MapPiece>();
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                availableMapPieces.Add(new MapPiece(-cols / 2 + c, -rows / 2 + r));
            }
        }
    }

    private void InitializeSnakes()
    {
        //Spawn the snakes on the same column but with one row in between
        for (int i = 0; i < numOfSnakes; i++)
        {
            Snake snake = Instantiate(snakePrefab);
            snake.Initialize(this, 0, numOfSnakes - 1 - 2 * i, startLengthOfSnake, startDirection, snakeColors[i],
                keySets[i], i + 1);
            totalSnakes++;
        }
    }

    public void AddAvailablePiece(float x, float y)
    {
        UpdateAvailablePieces(x, y, true);
    }

    public void RemoveAvailablePiece(float x, float y)
    {
        UpdateAvailablePieces(x, y, false);
    }

    private void UpdateAvailablePieces(float x, float y, bool isAdding)
    {
        MapPiece mp = new MapPiece(x, y);
        if (isAdding)
        {
            availableMapPieces.Add(mp);
        }
        else
        {
            availableMapPieces.Remove(mp);
        }

        if (availableMapPieces.Count == 0)
        {
            gameOverText.SetActive(true);
        }
    }

    public void SpawnApple()
    {
        if (availableMapPieces.Count == 0)
        {
            gameOverText.SetActive(true);
            return;
        }

        int index = Random.Range(0, availableMapPieces.Count);
        availableMapPieces.RemoveAt(index);
        Instantiate(applePrefab, new Vector3(availableMapPieces[index].x, availableMapPieces[index].y, 0),
            Quaternion.identity);
    }

    public void DisplayLostText(int playerNum)
    {
        totalSnakes--;
        gameStateText.text += "\nPlayer" + playerNum + " lost!";

        if (totalSnakes == 0 && gameOverText != null)
        {
            gameOverText.SetActive(true);
        }
    }

    public void ReplayGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}