using System.Collections.Generic;
using UnityEngine;

public class ClimbingPoint : MonoBehaviour
{
    public bool MountPoint;
    public List<Neighbour> neighbours = new List<Neighbour>();

    void Awake()
    {
        if (neighbours == null)
            neighbours = new List<Neighbour>();

        // Create reverse connections for two-way neighbours.
        foreach (var neighbour in neighbours)
        {
            if (neighbour == null || !neighbour.isPointTwoWay)
                continue;

            if (neighbour.climbingPoint != null)
            {
                neighbour.climbingPoint.CreatePointConnection(
                    this,
                    -neighbour.pointDirection,
                    neighbour.connectionType,
                    neighbour.isPointTwoWay
                );
            }
        }
    }

    public void CreatePointConnection(
        ClimbingPoint climbingPoint,
        Vector2 pointDir,
        ConnectionType connectionType,
        bool isPointTwoWay)
    {
        if (neighbours == null)
            neighbours = new List<Neighbour>();

        if (climbingPoint == null)
            return;

        // Avoid adding the same connection more than once.
        foreach (var existing in neighbours)
        {
            if (existing != null &&
                existing.climbingPoint == climbingPoint &&
                existing.pointDirection == pointDir &&
                existing.connectionType == connectionType)
            {
                return;
            }
        }

        var neighbour = new Neighbour
        {
            climbingPoint = climbingPoint,
            pointDirection = pointDir,
            connectionType = connectionType,
            isPointTwoWay = isPointTwoWay
        };

        neighbours.Add(neighbour);
    }

    public Neighbour GetNeighbour(Vector2 climbDirection)
    {
        if (neighbours == null)
            return null;

        Neighbour neighbour = null;

        if (climbDirection.y != 0)
        {
            foreach (var item in neighbours)
            {
                if (item != null &&
                    item.climbingPoint != null &&
                    item.pointDirection.y == climbDirection.y)
                {
                    neighbour = item;
                    break;
                }
            }
        }

        if (neighbour == null && climbDirection.x != 0)
        {
            foreach (var item in neighbours)
            {
                if (item != null &&
                    item.climbingPoint != null &&
                    item.pointDirection.x == climbDirection.x)
                {
                    neighbour = item;
                    break;
                }
            }
        }

        return neighbour;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.black);

        if (neighbours == null)
            return;

        foreach (var neighbour in neighbours)
        {
            if (neighbour != null && neighbour.climbingPoint != null)
            {
                Debug.DrawLine(
                    transform.position,
                    neighbour.climbingPoint.transform.position,
                    neighbour.isPointTwoWay ? Color.green : Color.black
                );
            }
        }
    }
}

[System.Serializable]
public class Neighbour
{
    public ClimbingPoint climbingPoint;
    public Vector2 pointDirection;
    public ConnectionType connectionType;
    public bool isPointTwoWay = true;
}

public enum ConnectionType
{
    Jump,
    Move
}