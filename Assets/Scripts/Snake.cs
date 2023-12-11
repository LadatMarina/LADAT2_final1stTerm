using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        Down,
        Up
    }

    private enum State
    {
        Alive,
        Dead
    }
    
    private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition; // Posición 2D de la SnakeBodyPart
        private Transform transform;

        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyPartGameObject = new GameObject("Snake Body",
                typeof(SpriteRenderer));
            SpriteRenderer snakeBodyPartSpriteRenderer = snakeBodyPartGameObject.GetComponent<SpriteRenderer>();
            snakeBodyPartSpriteRenderer.sprite = 
                GameAssets.Instance.snakeBodySprite;
            snakeBodyPartSpriteRenderer.sortingOrder = -bodyIndex;
            transform = snakeBodyPartGameObject.transform;
        }
        
        public void SetMovePosition(SnakeMovePosition snakeMovePosition)
        {
            // Posición (gridPosition)
            this.snakeMovePosition = snakeMovePosition; // Posición 2D y la dirección de la SnakeBodyPart
            Vector2Int gridPosition = snakeMovePosition.GetGridPosition();
            transform.position = new Vector3(gridPosition.x, 
                gridPosition.y, 0); // Posición 3D del G.O.
            
            // Dirección (direction)
            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Left: // Currently Going Left
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: // Previously Going Left
                            angle = 90;
                            break;
                        case Direction.Down: // Previously Going Down
                            angle = -45;
                            break;
                        case Direction.Up: // Previously Going Up
                            angle = 45;
                            break;
                    }
                    break;
                case Direction.Right: // Currently Going Right
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: // Previously Going Right
                            angle = -90;
                            break;
                        case Direction.Down: // Previously Going Down
                            angle = 45;
                            break;
                        case Direction.Up: // Previously Going Up
                            angle = -45;
                            break;
                    }
                    break;
                case Direction.Up: // Currently Going Up
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: // Previously Going Up
                            angle = 0;
                            break;
                        case Direction.Left: // Previously Going Left
                            angle = 45;
                            break;
                        case Direction.Right: // Previously Going Right
                            angle = -45;
                            break;
                    }
                    break;
                case Direction.Down: // Currently Going Down
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default: // Previously Going Down
                            angle = 180;
                            break;
                        case Direction.Left: // Previously Going Left
                            angle = -45;
                            break;
                        case Direction.Right: // Previously Going Right
                            angle = 45;
                            break;
                    }
                    break;
            } 

            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }
    
    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            return previousSnakeMovePosition.GetDirection();
        }
        
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
            }
            
            if (snakeMovePositionsList.Count > snakeBodySize)
            {
                snakeMovePositionsList.
                    RemoveAt(snakeMovePositionsList.Count - 1);
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
                    canMove = false;
                    Debug.Log(gridMoveDirection.ToString() + "WRONG DIR");
                    //OnWrongDir();
                    //cridar evento de quan no pot anar a sa direcció chosen
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
                    gridMoveDirection = Direction.Down;

                }
                else
                {
                    canMove = false;
                    Debug.Log(gridMoveDirection.ToString() + "WRONG DIR");

                    //OnWrongDir();
                    //gridMoveDirection = (Direction)Random.Range(0, 2);
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
                    canMove = false;
                    Debug.Log(gridMoveDirection.ToString() + "WRONG DIR"); ;

                    //OnWrongDir();
                    //gridMoveDirection = (Direction)Random.Range(2, 4);
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
                    canMove = false;
                    Debug.Log(gridMoveDirection.ToString() + "WRONG DIR");
                    //OnWrongDir();
                    //gridMoveDirection = (Direction)Random.Range(2, 4);
                }

            }
        }
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
}
