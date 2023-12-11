using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left,
    Right,
    Down,
    Up
}

public class Snake : MonoBehaviour
{
    private enum State
    {
        Alive,
        Dead,
        Winner
    }
    
    # region VARIABLES
    private Vector2Int gridPosition; // Posición 2D de la cabeza
    private Vector2Int startGridPosition;
    private Direction gridMoveDirection; // Dirección de la cabeza

    private float horizontalInput, verticalInput;

    private float gridMoveTimer;
    private float gridMoveTimerMax = 0.5f; // La serpiente se moverá a cada segundo

    private LevelGrid levelGrid;

    private int snakeBodySize; // Cantidad de partes del cuerpo (sin cabeza)
    private List<SnakeMovePosition> snakeMovePositionsList; // Posiciones y direcciones de cada parte (por orden)
    private List<SnakeBodyPart> snakeBodyPartsList;

    private State state;
    public bool canMove = true;
    private GameObject arrowParent;
    private int timesOnWrongDir;

    #endregion

    private void Awake()
    {
        startGridPosition = new Vector2Int(0, 0);
        gridPosition = startGridPosition;

        gridMoveDirection = Direction.Up; // Dirección arriba por defecto
        transform.eulerAngles = Vector3.zero; // Rotación arriba por defecto

        snakeBodySize = 0;
        snakeMovePositionsList = new List<SnakeMovePosition>();
        snakeBodyPartsList = new List<SnakeBodyPart>();

        state = State.Alive;
    }

    private void Start()
    {
        arrowParent = GameAssets.Instance.arrowParent;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleMoveDirection();
                HandleGridMovement();
                break;
            case State.Dead:
                break;
            case State.Winner:
                GameManager.Instance.Winner();
                break;
        }
    }

    public void Setup(LevelGrid levelGrid)
    {
        // levelGrid de snake = levelGrid que viene por parámetro
        this.levelGrid = levelGrid;
    }

    private void HandleGridMovement() // Relativo al movimiento en 2D
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax; // Se reinicia el temporizador
            
            SoundManager.PlaySound(SoundManager.Sound.SnakeMove);
            
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionsList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionsList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionsList.Insert(0, snakeMovePosition);

            // Relación entre enum Direction y vectores left, right, down y up
            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Left:
                    gridMoveDirectionVector = new Vector2Int(-1, 0);
                    break;
                case Direction.Right:
                    gridMoveDirectionVector = new Vector2Int(1, 0);
                    break;
                case Direction.Down:
                    gridMoveDirectionVector = new Vector2Int(0, -1);
                    break;
                case Direction.Up:
                    gridMoveDirectionVector = new Vector2Int(0, 1);
                    break;
            }
            
            gridPosition += gridMoveDirectionVector; // Mueve la posición 2D de la cabeza de la serpiente
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            
            // ¿He comido comida?
            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                // El cuerpo crece
                snakeBodySize++;
                CreateBodyPart();
                SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }
            
            if (snakeMovePositionsList.Count > snakeBodySize)
            {
                snakeMovePositionsList.
                    RemoveAt(snakeMovePositionsList.Count - 1);
            }
            if(snakeMovePositionsList.Count > 80)
            {
                state = State.Winner;
            }
            // Comprobamos el Game Over aquí porque tenemos la posición de la cabeza y la lista snakeMovePositionsList actualizadas para poder comprobar la muerte
            foreach (SnakeMovePosition movePosition in snakeMovePositionsList)
            {
                if (gridPosition == movePosition.GetGridPosition()) // Posición de la cabeza coincide con alguna parte del cuerpo
                {
                    // GAME OVER
                    state = State.Dead;
                    GameManager.Instance.SnakeDied();
                }
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
            canMove = true;
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector));
            UpdateBodyParts();
            GameManager.Instance.HidePossibledirections();
        }
    }

    private void HandleMoveDirection()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (canMove)
        {
            // Cambio dirección hacia arriba
            if (verticalInput > 0) // Si he pulsado hacia arriba (W o Flecha Arriba)
            {
                if (gridMoveDirection != Direction.Down) // Si iba en horizontal
                {
                    canMove = false;
                    // Cambio la dirección hacia arriba (0,1)
                    gridMoveDirection = Direction.Up;
                }
                else
                {
                    IsWrongDirection(Direction.Down); 
                    /*
                     * canMove = false;
                    //IsWrongDirection(Direction.Down);
                    timesOnWrongDir++; //if the player have pressed 3 times a wrong direction, the snake will move to a random direction

                    Debug.Log(timesOnWrongDir);

                    if (timesOnWrongDir > 3)
                    {
                        timesOnWrongDir = 0;
                        if ((gridMoveDirection == Direction.Right) || (gridMoveDirection == Direction.Left))
                        {
                            gridMoveDirection = (Direction)Random.Range(2, 4);
                        }
                        else if ((gridMoveDirection == Direction.Up) || (gridMoveDirection == Direction.Down))
                        {
                            gridMoveDirection = (Direction)Random.Range(0, 2);
                        }
                        Debug.Log("THE SNAKE HAS TAKEN A RANDOM DIRECTION  " + gridMoveDirection);
                    }*/
                }
            }

            // Cambio dirección hacia abajo
            // Input es abajo?
            if (verticalInput < 0)
            {
                // Mi dirección hasta ahora era horizontal
                if (gridMoveDirection != Direction.Up)
                {
                    canMove = false;
                    //gridMoveDirection = Direction.Down;
                    //Changes(gridMoveDirection);
                    gridMoveDirection = Direction.Down;
                }
                else
                {
                    
                    //IsWrongDirection(Direction.Up);
                }
            }

            // Cambio dirección hacia derecha
            if (horizontalInput > 0)
            {
                if (gridMoveDirection != Direction.Left)
                {
                    canMove = false;
                    gridMoveDirection = Direction.Right;
                }
                else
                {
                    IsWrongDirection(Direction.Left);
                }


            }

            // Cambio dirección hacia izquierda
            if (horizontalInput < 0)
            {
                if (gridMoveDirection != Direction.Right)
                {
                    canMove = false;

                    gridMoveDirection = Direction.Left;
                }
                else
                {
                    //RandomDirection(gridMoveDirection);
                    IsWrongDirection(Direction.Right);
                }

            }
            //Vector3 gridposition3D = new Vector3(gridPosition.x, gridPosition.y, 0);

            //arrowParent.transform.position= Vector3.MoveTowards(arrowParent.transform.position, gridposition3D, 0.1f);
            //    transform.position = Vector3.MoveTowards(transform.position, targ.transform.position, .03);
        }
    }
    private void Changes(Direction gridMoveDirection)
    {
        gridMoveDirection = Direction.Down;
    }
    private float GetAngleFromVector(Vector2Int direction)
    {
        float degrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (degrees < 0)
        {
            degrees += 360;
        }

        return degrees - 90;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public List<Vector2Int> GetFullSnakeBodyGridPosition()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionsList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private void CreateBodyPart()
    {
        snakeBodyPartsList.Add(new SnakeBodyPart(snakeBodySize));
    }

    private void UpdateBodyParts()
    {
        for (int i = 0; i < snakeBodyPartsList.Count; i++)
        {
            snakeBodyPartsList[i].SetMovePosition(snakeMovePositionsList[i]);
        }
    }

    private void IsWrongDirection(Direction wrongDirection) { 
        canMove = false;
        RandomDirection(gridMoveDirection);
        GameManager.Instance.ShowPossibleDirections(wrongDirection);
        SoundManager.PlaySound(SoundManager.Sound.WrongDirection);
    }

    private void RandomDirection(Direction gridMoveDir) 
    {
        timesOnWrongDir++; //if the player have pressed 3 times a wrong direction, the snake will move to a random direction

        Debug.Log(timesOnWrongDir);

        if(timesOnWrongDir > 3)
        {
            timesOnWrongDir = 0;
            if((gridMoveDir == Direction.Right) || (gridMoveDir == Direction.Left))
            {
                gridMoveDir = (Direction)Random.Range(2, 4);
            }
            else if ((gridMoveDir == Direction.Up) || (gridMoveDir == Direction.Down))
            {
                gridMoveDir = (Direction)Random.Range(0, 2);
            }
            Debug.Log("THE SNAKE HAS TAKEN A RANDOM DIRECTION  " + gridMoveDir);
        }
    }

    public void ChangeState(int n)
    {
        state = (State)n;
    }
}
