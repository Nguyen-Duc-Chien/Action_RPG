using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Grid-based A* pathfinding for 2D games.
/// Attach to any enemy that needs intelligent obstacle avoidance.
/// Uses Physics2D.OverlapBox to check walkability against the obstacle layer.
/// </summary>
public class AStarPathfinder : MonoBehaviour
{
    [Header("Grid Settings")]
    [Tooltip("Size of each grid cell. Smaller = more precise but slower.")]
    public float cellSize = 0.25f;

    [Tooltip("How many cells to scan in each direction from the enemy. Total grid = (2*gridExtent+1)^2")]
    public int gridExtent = 30;

    [Tooltip("How often (in seconds) to recalculate the path.")]
    public float pathUpdateInterval = 0.3f;

    [Tooltip("Extra clearance space around obstacles. Increase this if the enemy gets stuck on corners.")]
    public float obstaclePadding = 0.3f;

    [Header("References")]
    public LayerMask obstacleLayer;

    // The current calculated path (list of world positions)
    private List<Vector2> currentPath = new List<Vector2>();
    private int currentWaypointIndex = 0;
    private float pathUpdateTimer = 0f;

    // Cache for the grid to avoid per-frame allocations
    private bool[,] walkableGrid;
    private int gridWidth;
    private int gridHeight;
    private Vector2 gridOrigin; // bottom-left corner of the grid in world space

    /// <summary>
    /// Returns true if there is a valid path available.
    /// </summary>
    public bool HasPath => currentPath != null && currentPath.Count > 0 && currentWaypointIndex < currentPath.Count;

    /// <summary>
    /// Returns the next waypoint to move toward.
    /// </summary>
    public Vector2 CurrentWaypoint => HasPath ? currentPath[currentWaypointIndex] : (Vector2)transform.position;

    /// <summary>
    /// Call this from the enemy's Update to get the movement direction via A*.
    /// Returns Vector2.zero if no path or already at destination.
    /// </summary>
    public Vector2 GetDirectionToTarget(Vector2 targetPosition, float arrivedThreshold = 0.3f)
    {
        // Periodically recalculate path
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0f)
        {
            pathUpdateTimer = pathUpdateInterval;
            CalculatePath(transform.position, targetPosition);
        }

        if (!HasPath)
            return Vector2.zero;

        Vector2 waypoint = currentPath[currentWaypointIndex];
        Vector2 myPos = transform.position;
        float distToWaypoint = Vector2.Distance(myPos, waypoint);

        // If we've reached the current waypoint, advance to the next
        if (distToWaypoint < arrivedThreshold)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= currentPath.Count)
                return Vector2.zero;

            waypoint = currentPath[currentWaypointIndex];
        }

        return (waypoint - myPos).normalized;
    }

    /// <summary>
    /// Force an immediate path recalculation (useful when target changes suddenly).
    /// </summary>
    public void ForcePath(Vector2 targetPosition)
    {
        pathUpdateTimer = pathUpdateInterval;
        CalculatePath(transform.position, targetPosition);
    }

    // ───────────────────── A* CORE ─────────────────────

    private void CalculatePath(Vector2 startWorld, Vector2 endWorld)
    {
        BuildGrid(startWorld);

        Vector2Int startCell = WorldToGrid(startWorld);
        Vector2Int endCell = WorldToGrid(endWorld);

        // Clamp to grid bounds
        startCell = ClampToGrid(startCell);
        endCell = ClampToGrid(endCell);

        // If start or end is not walkable, find nearest walkable cell
        if (!IsWalkable(startCell))
            startCell = FindNearestWalkable(startCell);
        if (!IsWalkable(endCell))
            endCell = FindNearestWalkable(endCell);

        // Run A*
        List<Vector2Int> cellPath = FindPath(startCell, endCell);

        currentPath.Clear();
        currentWaypointIndex = 0;

        if (cellPath != null)
        {
            // Convert grid cells to world positions, then smooth
            for (int i = 0; i < cellPath.Count; i++)
            {
                currentPath.Add(GridToWorld(cellPath[i]));
            }

            // Simplify the path by removing unnecessary waypoints using line-of-sight
            currentPath = SmoothPath(currentPath);
        }
    }

    private void BuildGrid(Vector2 center)
    {
        gridWidth = gridExtent * 2 + 1;
        gridHeight = gridExtent * 2 + 1;
        gridOrigin = center - new Vector2(gridExtent * cellSize, gridExtent * cellSize);

        if (walkableGrid == null || walkableGrid.GetLength(0) != gridWidth)
            walkableGrid = new bool[gridWidth, gridHeight];

        float checkRadius = (cellSize * 0.5f) + obstaclePadding;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPos = GridToWorld(new Vector2Int(x, y));
                Collider2D hit = Physics2D.OverlapCircle(worldPos, checkRadius, obstacleLayer);
                walkableGrid[x, y] = (hit == null);
            }
        }
    }

    private Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt((worldPos.x - gridOrigin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPos.y - gridOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    private Vector2 GridToWorld(Vector2Int gridPos)
    {
        float x = gridOrigin.x + gridPos.x * cellSize;
        float y = gridOrigin.y + gridPos.y * cellSize;
        return new Vector2(x, y);
    }

    private Vector2Int ClampToGrid(Vector2Int cell)
    {
        cell.x = Mathf.Clamp(cell.x, 0, gridWidth - 1);
        cell.y = Mathf.Clamp(cell.y, 0, gridHeight - 1);
        return cell;
    }

    private bool IsWalkable(Vector2Int cell)
    {
        if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
            return false;
        return walkableGrid[cell.x, cell.y];
    }

    private Vector2Int FindNearestWalkable(Vector2Int cell)
    {
        // BFS outward to find nearest walkable cell
        for (int radius = 1; radius <= gridExtent; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius) continue; // only check border
                    Vector2Int check = new Vector2Int(cell.x + dx, cell.y + dy);
                    if (IsWalkable(check)) return check;
                }
            }
        }
        return cell;
    }

    // ───────────────────── A* ALGORITHM ─────────────────────

    private List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        if (start == end)
            return new List<Vector2Int> { start };

        // Using a simple list-based open set (fine for small grids)
        List<Node> openList = new List<Node>();
        HashSet<long> closedSet = new HashSet<long>();
        Dictionary<long, Node> allNodes = new Dictionary<long, Node>();

        Node startNode = new Node(start, 0, Heuristic(start, end), null);
        openList.Add(startNode);
        allNodes[CellKey(start)] = startNode;

        // 8-directional neighbors
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // up
            new Vector2Int(0, -1),  // down
            new Vector2Int(1, 0),   // right
            new Vector2Int(-1, 0),  // left
            new Vector2Int(1, 1),   // up-right
            new Vector2Int(-1, 1),  // up-left
            new Vector2Int(1, -1),  // down-right
            new Vector2Int(-1, -1)  // down-left
        };

        int maxIterations = gridWidth * gridHeight; // safety limit
        int iterations = 0;

        while (openList.Count > 0 && iterations < maxIterations)
        {
            iterations++;

            // Find node with lowest f cost
            Node current = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F < current.F ||
                    (openList[i].F == current.F && openList[i].H < current.H))
                {
                    current = openList[i];
                }
            }

            openList.Remove(current);
            closedSet.Add(CellKey(current.Position));

            // Goal reached
            if (current.Position == end)
            {
                return ReconstructPath(current);
            }

            // Check all neighbors
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int neighborPos = current.Position + directions[i];

                if (!IsWalkable(neighborPos)) continue;
                if (closedSet.Contains(CellKey(neighborPos))) continue;

                // For diagonal movement, ensure both adjacent cells are walkable
                // to prevent cutting corners
                if (directions[i].x != 0 && directions[i].y != 0)
                {
                    if (!IsWalkable(new Vector2Int(current.Position.x + directions[i].x, current.Position.y)) ||
                        !IsWalkable(new Vector2Int(current.Position.x, current.Position.y + directions[i].y)))
                    {
                        continue;
                    }
                }

                // Diagonal cost = 1.414, straight cost = 1.0
                float moveCost = (directions[i].x != 0 && directions[i].y != 0) ? 1.414f : 1f;
                float newG = current.G + moveCost;

                long neighborKey = CellKey(neighborPos);
                Node neighborNode;

                if (allNodes.TryGetValue(neighborKey, out neighborNode))
                {
                    // Already seen - update if this path is better
                    if (newG < neighborNode.G)
                    {
                        neighborNode.G = newG;
                        neighborNode.Parent = current;
                    }
                }
                else
                {
                    // New node
                    neighborNode = new Node(neighborPos, newG, Heuristic(neighborPos, end), current);
                    allNodes[neighborKey] = neighborNode;
                    openList.Add(neighborNode);
                }
            }
        }

        // No path found
        return null;
    }

    private float Heuristic(Vector2Int a, Vector2Int b)
    {
        // Chebyshev distance (suitable for 8-directional movement)
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return 1f * (dx + dy) + (1.414f - 2f) * Mathf.Min(dx, dy);
    }

    private long CellKey(Vector2Int cell)
    {
        // Combine x and y into a single long key for fast hashing
        return ((long)cell.x << 32) | (long)(cell.y & 0xFFFFFFFF);
    }

    private List<Vector2Int> ReconstructPath(Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = endNode;
        while (current != null)
        {
            path.Add(current.Position);
            current = current.Parent;
        }
        path.Reverse();
        return path;
    }

    // ───────────────────── PATH SMOOTHING ─────────────────────

    /// <summary>
    /// Removes unnecessary intermediate waypoints using line-of-sight checks.
    /// This makes movement look more natural instead of following a rigid grid.
    /// </summary>
    private List<Vector2> SmoothPath(List<Vector2> rawPath)
    {
        if (rawPath.Count <= 2) return rawPath;

        List<Vector2> smoothed = new List<Vector2>();
        smoothed.Add(rawPath[0]);

        int current = 0;
        while (current < rawPath.Count - 1)
        {
            int furthest = current + 1;
            for (int check = rawPath.Count - 1; check > current + 1; check--)
            {
                if (HasLineOfSight(rawPath[current], rawPath[check]))
                {
                    furthest = check;
                    break;
                }
            }
            smoothed.Add(rawPath[furthest]);
            current = furthest;
        }

        return smoothed;
    }

    private bool HasLineOfSight(Vector2 a, Vector2 b)
    {
        Vector2 direction = b - a;
        float distance = direction.magnitude;
        
        // Use CircleCast instead of Raycast to account for the character's body width.
        // This prevents the smoothed path from cutting corners too closely and getting stuck.
        float castRadius = cellSize * 0.4f;
        RaycastHit2D hit = Physics2D.CircleCast(a, castRadius, direction.normalized, distance, obstacleLayer);
        
        return hit.collider == null;
    }

    // ───────────────────── DEBUG VISUALIZATION ─────────────────────

    private void OnDrawGizmosSelected()
    {
        if (currentPath == null || currentPath.Count == 0) return;

        // Draw path
        Gizmos.color = Color.cyan;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
        }

        // Draw waypoints
        Gizmos.color = Color.magenta;
        for (int i = 0; i < currentPath.Count; i++)
        {
            Gizmos.DrawSphere(currentPath[i], 0.15f);
        }

        // Draw current waypoint larger
        if (HasPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(currentPath[currentWaypointIndex], 0.25f);
        }
    }

    // ───────────────────── NODE CLASS ─────────────────────

    private class Node
    {
        public Vector2Int Position;
        public float G; // cost from start
        public float H; // heuristic (estimated cost to end)
        public float F => G + H;
        public Node Parent;

        public Node(Vector2Int position, float g, float h, Node parent)
        {
            Position = position;
            G = g;
            H = h;
            Parent = parent;
        }
    }
}
