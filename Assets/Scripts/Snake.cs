using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct KeySet
{
    public KeyCode up;
    public KeyCode down;
    public KeyCode left;
    public KeyCode right;

    public KeySet(KeyCode up = KeyCode.W, KeyCode down = KeyCode.S, KeyCode left = KeyCode.A, KeyCode right = KeyCode.D)
    {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }
}

public enum MoveDirection
{
    Up,
    Down,
    Left,
    Right
}

public class MapPiece : IEquatable<MapPiece>
{
    public float x { get; protected set; }
    public float y { get; protected set; }

    public MapPiece(float newX = 0, float newY = 0)
    {
        x = newX;
        y = newY;
    }

    public bool Equals(MapPiece other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return x.Equals(other.x) && y.Equals(other.y);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((MapPiece)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }
}

public class SnakePiece : MapPiece
{
    public SnakePiece(MoveDirection newDirection = MoveDirection.Right, float newX = 0, float newY = 0)
    {
        direction = newDirection;
        x = newX;
        y = newY;
    }

    public MoveDirection direction { get; private set; }

    public void Move()
    {
        switch (direction)
        {
            case MoveDirection.Up:
                y += 1;
                break;
            case MoveDirection.Down:
                y -= 1;
                break;
            case MoveDirection.Left:
                x -= 1;
                break;
            case MoveDirection.Right:
                x += 1;
                break;
            default:
                Debug.LogError("Something is wrong with the direction when the grid moves!");
                break;
        }
    }

    public void SetDirection(MoveDirection newDirection)
    {
        direction = newDirection;
    }
}

public class Snake : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform snakePiecePrefab;
    [SerializeField] private Transform snakeParent;

    private List<Transform> snakePiecesBody;
    private List<SnakePiece> snakePiecesData;
    private MoveDirection currentDirection;
    private Color snakeColor;
    private KeySet keySet;
    private bool isDirectionLocked;
    private int playerNum;

    void Start()
    {
        StartCoroutine(UpdateSnake());
    }

    // Update is called once per frame
    void Update()
    {
        ChangeDirection();
    }

    public void Initialize(GameManager gm, float headX, float headY, int length, MoveDirection direction, Color color,
        KeySet ks, int playerNo)
    {
        gameManager = gm;
        currentDirection = direction;
        snakeColor = color;
        keySet = ks;
        playerNum = playerNo;
        isDirectionLocked = false;
        snakeParent = transform;
        snakePiecesBody = new List<Transform>();
        snakePiecesData = new List<SnakePiece>();

        for (int i = 0; i < length; i++)
        {
            Transform t = Instantiate(snakePiecePrefab, new Vector3(headX + i, headY, 0), Quaternion.identity,
                snakeParent);
            t.GetComponent<MeshRenderer>().material.color = snakeColor;
            t.GetComponent<CollisionDetector>().snake = this;
            snakePiecesBody.Add(t.transform);
            snakePiecesData.Add(new SnakePiece(currentDirection, headX + i, headY));
            gameManager.RemoveAvailablePiece(t.position.x, t.position.y);
        }

        snakePiecesBody[0].tag = "SnakeHead";
    }

    private IEnumerator UpdateSnake()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / gameManager.speed);

            MoveSnake();
        }
    }

    private void MoveSnake()
    {
        for (int i = 0; i < snakePiecesBody.Count; i++)
        {
            snakePiecesData[i].Move();
            snakePiecesBody[i].position = new Vector3(snakePiecesData[i].x, snakePiecesData[i].y, 0);
        }

        for (int i = snakePiecesData.Count - 1; i > 0; i--)
        {
            snakePiecesData[i].SetDirection(snakePiecesData[i - 1].direction);
        }

        gameManager.RemoveAvailablePiece(snakePiecesData[0].x, snakePiecesData[0].y);
        gameManager.AddAvailablePiece(snakePiecesData.Last().x, snakePiecesData.Last().y);

        isDirectionLocked = false;
    }

    private void AddSnakePiece()
    {
        SnakePiece currentTail = snakePiecesData.Last();
        Vector3 newTailPos = new Vector3();
        switch (currentTail.direction)
        {
            case MoveDirection.Up:
                newTailPos = new Vector3(currentTail.x, currentTail.y - 1, 0);
                break;
            case MoveDirection.Down:
                newTailPos = new Vector3(currentTail.x, currentTail.y + 1, 0);
                break;
            case MoveDirection.Right:
                newTailPos = new Vector3(currentTail.x - 1, currentTail.y, 0);
                break;
            case MoveDirection.Left:
                newTailPos = new Vector3(currentTail.x + 1, currentTail.y, 0);
                break;
        }

        Transform t = Instantiate(snakePiecePrefab, newTailPos, Quaternion.identity, snakeParent);
        t.GetComponent<MeshRenderer>().material.color = snakeColor;
        t.GetComponent<CollisionDetector>().snake = this;
        t.tag = "SnakeBody";
        snakePiecesBody.Add(t.transform);

        SnakePiece newTail = new SnakePiece(currentTail.direction, newTailPos.x, newTailPos.y);
        snakePiecesData.Add(newTail);

        gameManager.RemoveAvailablePiece(newTailPos.x, newTailPos.y);
    }

    //Change to new direction unless it's opposite to current direction 
    private void ChangeDirection()
    {
        if (!isDirectionLocked)
        {
            if (Input.GetKeyDown(keySet.up) && currentDirection != MoveDirection.Up &&
                currentDirection != MoveDirection.Down)
            {
                currentDirection = MoveDirection.Up;
                isDirectionLocked = true;
            }

            if (Input.GetKeyDown(keySet.down) && currentDirection != MoveDirection.Down &&
                currentDirection != MoveDirection.Up)
            {
                currentDirection = MoveDirection.Down;
                isDirectionLocked = true;
            }

            if (Input.GetKeyDown(keySet.left) && currentDirection != MoveDirection.Left &&
                currentDirection != MoveDirection.Right)
            {
                currentDirection = MoveDirection.Left;
                isDirectionLocked = true;
            }

            if (Input.GetKeyDown(keySet.right) && currentDirection != MoveDirection.Right &&
                currentDirection != MoveDirection.Left)
            {
                currentDirection = MoveDirection.Right;
                isDirectionLocked = true;
            }
        }

        snakePiecesData[0].SetDirection(currentDirection);
    }

    public void EatApple()
    {
        AddSnakePiece();
        gameManager.SpawnApple();
    }

    private void OnDestroy()
    {
        gameManager.DisplayLostText(playerNum);
    }
}