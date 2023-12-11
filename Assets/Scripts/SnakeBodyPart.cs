using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBodyPart
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