using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public Snake snake;

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag("SnakeHead"))
        {
            switch (other.tag)
            {
                case "Wall":
                case "SnakeBody":
                case "SnakeHead":
                    Destroy(transform.parent.gameObject);
                    break;
                case "Apple":
                    snake.EatApple();
                    Destroy(other.gameObject);
                    break;
            }
        }
    }
}