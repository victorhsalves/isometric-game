using UnityEngine;

[System.Serializable]
public class GridVisualizationController : MonoBehaviour
{
    [Header("Grid Visualization Settings")]
    [SerializeField] private TerrainController terrainController;
    [SerializeField] private bool showGridOnStart = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.G;
    
    [Header("Visual Settings")]
    [SerializeField] private Color gridColor = Color.white;
    [SerializeField] private float gridOpacity = 0.3f;
    [SerializeField] private float cellHeight = 0.1f;
    
    private bool isGridVisible = false;

    private void Start()
    {
        if (terrainController == null)
        {
            terrainController = FindAnyObjectByType<TerrainController>();
        }
        
        if (showGridOnStart)
        {
            ShowGrid();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleGrid();
        }
    }

    [ContextMenu("Show Grid")]
    public void ShowGrid()
    {
        if (terrainController != null)
        {
            terrainController.ToggleCellVisibility(true);
            isGridVisible = true;
            Debug.Log("Grid visualization enabled");
        }
    }

    [ContextMenu("Hide Grid")]
    public void HideGrid()
    {
        if (terrainController != null)
        {
            terrainController.ToggleCellVisibility(false);
            isGridVisible = false;
            Debug.Log("Grid visualization disabled");
        }
    }

    [ContextMenu("Toggle Grid")]
    public void ToggleGrid()
    {
        if (isGridVisible)
        {
            HideGrid();
        }
        else
        {
            ShowGrid();
        }
    }

    [ContextMenu("Refresh Grid")]
    public void RefreshGrid()
    {
        if (terrainController != null)
        {
            terrainController.RefreshCellVisualization();
            Debug.Log("Grid visualization refreshed");
        }
    }

    // Métodos para configuração via Inspector
    public void SetGridColor(Color color)
    {
        gridColor = color;
        if (terrainController != null)
        {
            terrainController.RefreshCellVisualization();
        }
    }

    public void SetGridOpacity(float opacity)
    {
        gridOpacity = Mathf.Clamp01(opacity);
        if (terrainController != null)
        {
            terrainController.RefreshCellVisualization();
        }
    }

    public void SetCellHeight(float height)
    {
        cellHeight = Mathf.Max(0.01f, height);
        if (terrainController != null)
        {
            terrainController.RefreshCellVisualization();
        }
    }
} 