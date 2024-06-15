using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.25f;
    [SerializeField] float rayLength = 1.1f; // Adjust this to match your grid size
    [SerializeField] float rayOffset = 0.5f; // Adjust this based on half the cube's height
    [SerializeField] bool roundPositions = true; // Option zum Ein- und Ausschalten des Rundens

    [SerializeField] Transform cube1; // Gelber Würfel
    [SerializeField] Transform cube2; // Blauer Würfel

    Vector3 targetPosition1;
    Vector3 targetPosition2;
    bool moving1;
    bool moving2;
    Vector3 lastDirection1;
    Vector3 lastDirection2;

    [SerializeField] LayerMask collidableMask = 0; // Mask to identify collidable objects

    void Update()
    {
        HandleInput();
        Move();
        DrawDebugRays();
    }

    void HandleInput()
    {
        if (!moving1 && !moving2)
        {
            if (Input.GetKey(KeyCode.W))
            {
                TryMove(Vector3.forward);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                TryMove(Vector3.back);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                TryMove(Vector3.right);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                TryMove(Vector3.left);
            }
        }
    }

    void TryMove(Vector3 direction)
    {
        Vector3 newTargetPosition1 = cube1.position + direction;
        Vector3 newTargetPosition2 = cube2.position + direction;

        if (AreCubesTogether(cube1.position, cube2.position, direction))
        {
            // Wenn die Würfel zusammen sind, übernimmt der vordere Würfel die Kollisionserkennung
            bool canMoveFront = CanMove(cube1, direction);
            bool canMoveBack = CanMove(cube2, direction);

            if (canMoveFront && canMoveBack)
            {
                SetTargetPositions(newTargetPosition1, newTargetPosition2, direction, direction);
            }
            else
            {
                Debug.Log("Beide Würfel sind blockiert.");
            }
        }
        else
        {
            bool canMove1 = CanMove(cube1, direction, cube2.position);
            bool canMove2 = CanMove(cube2, direction, cube1.position);

            if (canMove1)
            {
                targetPosition1 = newTargetPosition1;
                moving1 = true;
                lastDirection1 = direction;
            }
            if (canMove2)
            {
                targetPosition2 = newTargetPosition2;
                moving2 = true;
                lastDirection2 = direction;
            }

            if (!canMove1)
            {
                Debug.Log($"Kollision erkannt: {cube1.name} kann sich nicht bewegen in Richtung {direction}");
            }
            if (!canMove2)
            {
                Debug.Log($"Kollision erkannt: {cube2.name} kann sich nicht bewegen in Richtung {direction}");
            }
        }
    }

    bool AreCubesTogether(Vector3 pos1, Vector3 pos2, Vector3 direction)
    {
        Vector3 diff = pos1 - pos2;
        return (direction == Vector3.forward || direction == Vector3.back) && Mathf.Abs(diff.z) == 1 && Mathf.Abs(diff.x) == 0
            || (direction == Vector3.left || direction == Vector3.right) && Mathf.Abs(diff.x) == 1 && Mathf.Abs(diff.z) == 0;
    }

    bool CanMove(Transform cube, Vector3 direction)
    {
        // Raycast für den gegebenen Würfel
        bool canMove = !Physics.Raycast(cube.position + Vector3.up * rayOffset, direction, rayLength, collidableMask);
        Debug.Log($"{cube.name} kann sich in Richtung {direction}: {canMove}");
        return canMove;
    }

    bool CanMove(Transform cube, Vector3 direction, Vector3 otherCubePosition)
    {
        Vector3 targetPosition = cube.position + direction;

        // Überprüfen, ob der Würfel sich in die Position des anderen Würfels bewegt
        if (targetPosition == otherCubePosition)
        {
            Debug.Log($"{cube.name} kann sich nicht in Richtung {direction} bewegen, weil {otherCubePosition} blockiert ist.");
            return false;
        }

        return CanMove(cube, direction);
    }

    void SetTargetPositions(Vector3 targetPos1, Vector3 targetPos2, Vector3 direction1, Vector3 direction2)
    {
        targetPosition1 = targetPos1;
        targetPosition2 = targetPos2;
        if (direction1 != Vector3.zero)
        {
            moving1 = true;
            lastDirection1 = direction1;
        }
        if (direction2 != Vector3.zero)
        {
            moving2 = true;
            lastDirection2 = direction2;
        }
    }

    void Move()
    {
        if (moving1)
        {
            if (Vector3.Distance(cube1.position, targetPosition1) > 0.1f)
            {
                cube1.position = Vector3.MoveTowards(cube1.position, targetPosition1, moveSpeed * Time.deltaTime);
            }
            else
            {
                cube1.position = roundPositions ? RoundPosition(targetPosition1) : targetPosition1;
                moving1 = false;
                CheckIfGrounded(cube1, ref moving1, ref targetPosition1);
            }
        }

        if (moving2)
        {
            if (Vector3.Distance(cube2.position, targetPosition2) > 0.1f)
            {
                cube2.position = Vector3.MoveTowards(cube2.position, targetPosition2, moveSpeed * Time.deltaTime);
            }
            else
            {
                cube2.position = roundPositions ? RoundPosition(targetPosition2) : targetPosition2;
                moving2 = false;
                CheckIfGrounded(cube2, ref moving2, ref targetPosition2);
            }
        }
    }

    void CheckIfGrounded(Transform cube, ref bool moving, ref Vector3 targetPosition)
    {
        if (!IsGrounded(cube))
        {
            targetPosition = cube.position + Vector3.down;
            moving = true;
        }
    }

    bool IsGrounded(Transform cube)
    {
        // Raycast nach unten, um zu prüfen, ob der Würfel auf dem Boden ist
        bool grounded = Physics.Raycast(cube.position - Vector3.up * rayOffset, Vector3.down, rayLength, collidableMask);
        Debug.Log($"{cube.name} ist auf dem Boden: {grounded}");
        return grounded;
    }

    Vector3 RoundPosition(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
    }

    void DrawDebugRays()
    {
        if (moving1)
        {
            Debug.DrawRay(cube1.position + Vector3.up * rayOffset, lastDirection1 * rayLength, Color.red);
        }

        if (moving2)
        {
            Debug.DrawRay(cube2.position + Vector3.up * rayOffset, lastDirection2 * rayLength, Color.red);
        }

        // Debug-Rays nach unten für Grounded-Check
        Debug.DrawRay(cube1.position - Vector3.up * rayOffset, Vector3.down * rayLength, Color.green);
        Debug.DrawRay(cube2.position - Vector3.up * rayOffset, Vector3.down * rayLength, Color.green);
    }
}
