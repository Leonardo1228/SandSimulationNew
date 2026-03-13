using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 50;
    public int height = 30;
    public float updateTime;
    public GameObject SandPrefab;

    private bool[,] grid;
    private bool[,] nextGrid;
    private GameObject[,] SandObjects;

    private float timer;

    void Start()
    {
        grid = new bool[width, height];
        nextGrid = new bool[width, height];
        SandObjects = new GameObject[width, height];

        GenerateGrid();
        CreateFloor();
        CreateWalls();
        UpdateVisuals();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateTime)
        {
            StepSand();
            UpdateVisuals();
            timer = 0f;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            SpawnSand();
        }
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cell = Instantiate(SandPrefab, new Vector3(x, y, 0), Quaternion.identity);
                cell.transform.parent = transform;
                SandObjects[x, y] = cell;
            }
        }
    }

    void CreateFloor()
    {
        for (int x = 0; x < width; x++)
        {
            grid[x, 0] = true;
        }
    }

    void CreateWalls()
    {
        for (int y = 0; y < height; y++)
        {
            grid[0, y] = true;
            grid[width - 1, y] = true;
        }
    }

    void StepSand()
    {
        System.Array.Clear(nextGrid, 0, nextGrid.Length);

        // Colisiones de muros limitantes y el piso
        for (int x = 0; x < width; x++)
            nextGrid[x, 0] = true;

        for (int y = 0; y < height; y++)
        {
            nextGrid[0, y] = true;
            nextGrid[width - 1, y] = true;
        }

        for (int y = height - 1; y >= 1; y--)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (!grid[x, y])
                    continue;

                int newX = x;
                int newY = y;

                if (!grid[x, y - 1] && !nextGrid[x, y - 1])
                {
                    newY = y - 1;
                }
                else
                {
                    bool leftFirst = Random.value > 0.5f;

                    if (leftFirst)
                    {
                        if (!grid[x - 1, y - 1] && !nextGrid[x - 1, y - 1])
                        {
                            newX = x - 1;
                            newY = y - 1;
                        }
                        else if (!grid[x + 1, y - 1] && !nextGrid[x + 1, y - 1])
                        {
                            newX = x + 1;
                            newY = y - 1;
                        }
                    }
                    else
                    {
                        if (!grid[x + 1, y - 1] && !nextGrid[x + 1, y - 1])
                        {
                            newX = x + 1;
                            newY = y - 1;
                        }
                        else if (!grid[x - 1, y - 1] && !nextGrid[x - 1, y - 1])
                        {
                            newX = x - 1;
                            newY = y - 1;
                        }
                    }
                }

                nextGrid[newX, newY] = true;
            }
        }

        var temp = grid;
        grid = nextGrid;
        nextGrid = temp;
    }

    void SpawnSand()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        int x = Mathf.RoundToInt(worldPos.x);
        int y = Mathf.RoundToInt(worldPos.y);

        x = Mathf.Clamp(x, 1, width - 2);
        y = Mathf.Clamp(y, 1, height - 1);

        grid[x, y] = true;
    }

    void UpdateVisuals()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var rend = SandObjects[x, y].GetComponent<SpriteRenderer>();

                if (y == 0 || x == 0 || x == width - 1)
                {
                    rend.color = Color.gray; // Color de la paredes y piso
                }
                else
                {
                    rend.color = grid[x, y] ? new Color(0.9f, 0.8f, 0.3f) : Color.white;
                }
            }
        }
    }
}
