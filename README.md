# SnakeGame_Cube

1. This solution uses 3D cubes to represent snake pieces.
2. On each update, all the snake pieces move.
3. It uses colliders and rigidbody for collision detection. 

## How the snake moves

A snake consists of snake pieces. On every update, the algorithm will iterate through all the snake pieces and update each of them. First, the position of each snake piece is changed by 1 unit towards the direction this individual piece is heading. Then it will iterate from the last piece to the second piece, and update the direction of a snake piece based on the piece in front of it.

## What each script does

**GameManager** controls the game. 
> It takes the size (i.e. the number of rows and columns) of the game board and generate a boader for the game board. 

> It generates all the snakes at the center of the board and initializes them. 

> It has a list to keep track of all the spots available on the board where the apple can be spawned.

> It spawns an apple on the board.

> It is also in charge of displaying some text and contains functions to replay or quit the game. 

> You can also change the number of snakes, initial length of the snakes, the speed of the snakes and the colors of the snakes.

**Snake** controls a snake and its snake pieces. 
> On every update, the snake moves itself piece by piece and update both the physical body and data based on the movement. 

> When the corresponding direction keys are pressed, the snake will update its moving direction based on the input. 

> When the snake eats an apple, it will increase its length by 1 by adding a new snake piece at the tail of the snake. 

> When the snake dies, it will display text.

**CollisionDetector** is responsible for determining what happens when the head of the snake runs into different objects.
