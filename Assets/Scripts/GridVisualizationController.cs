using UnityEngine;
using System.Linq;

[System.Serializable]
public class GridVisualizationController : MonoBehaviour
{
    [Header("Grid Visualization Settings")]
    [SerializeField] private TerrainController terrainController;
    [SerializeField] private bool showGridOnStart = false;
    [SerializeField] private KeyCode toggleKey = KeyCode.B;
    
    [Header("Visual Settings")]
    [SerializeField] private Color gridColor = Color.white;
    [SerializeField] private float gridOpacity = 0.3f;
    [SerializeField] private float cellHeight = 0.1f;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugMode = false;
    
    private bool isGridVisible = false;

    private void Start()
    {
        if (terrainController == null)
        {
            terrainController = FindAnyObjectByType<TerrainController>();
        }
        
        if (terrainController != null)
        {
            // Configurar modo debug
            terrainController.SetDebugMode(enableDebugMode);
            
            // Aguardar próximo frame para garantir que as células foram criadas
            StartCoroutine(InitializeVisibilityState());
        }
        else
        {
            Debug.LogError("TerrainController not found! Please assign it in the inspector or make sure it exists in the scene.");
        }
    }
    
    private System.Collections.IEnumerator InitializeVisibilityState()
    {
        // Aguardar um frame para garantir que as células foram criadas
        yield return null;
        
        // Definir estado inicial correto
        isGridVisible = showGridOnStart;
        
        if (showGridOnStart)
        {
            ShowGrid();
        }
        else
        {
            HideGrid();
        }
        
        Debug.Log($"Grid visibility initialized: {isGridVisible}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            Debug.Log($"Toggle key {toggleKey} pressed");
            ToggleGrid();
        }
        
        // Toggle debug mode with F3
        if (Input.GetKeyDown(KeyCode.F3))
        {
            enableDebugMode = !enableDebugMode;
            if (terrainController != null)
            {
                terrainController.SetDebugMode(enableDebugMode);
            }
            Debug.Log($"Grid Debug mode: {enableDebugMode}");
        }
    }

    [ContextMenu("Show Grid")]
    public void ShowGrid()
    {
        if (terrainController != null)
        {
            terrainController.ToggleCellVisibility(true);
            isGridVisible = true;
            Debug.Log($"Grid visualization enabled - State: {isGridVisible}");
        }
        else
        {
            Debug.LogWarning("Cannot show grid - TerrainController is null");
        }
    }

    [ContextMenu("Hide Grid")]
    public void HideGrid()
    {
        if (terrainController != null)
        {
            terrainController.ToggleCellVisibility(false);
            isGridVisible = false;
            Debug.Log($"Grid visualization disabled - State: {isGridVisible}");
        }
        else
        {
            Debug.LogWarning("Cannot hide grid - TerrainController is null");
        }
    }

    [ContextMenu("Toggle Grid")]
    public void ToggleGrid()
    {
        Debug.Log($"Toggle Grid called - Current state: {isGridVisible}");
        
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
            bool wasVisible = isGridVisible;
            terrainController.RefreshCellVisualization();
            
            // Restaurar estado de visibilidade após o refresh
            if (wasVisible)
            {
                terrainController.ToggleCellVisibility(true);
            }
            else
            {
                terrainController.ToggleCellVisibility(false);
            }
            
            Debug.Log($"Grid visualization refreshed - Visibility restored: {wasVisible}");
        }
        else
        {
            Debug.LogWarning("Cannot refresh grid - TerrainController is null");
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

    // Verificar se tudo está configurado corretamente
    [ContextMenu("Validate Setup")]
    public void ValidateSetup()
    {
        bool hasErrors = false;
        
        if (terrainController == null)
        {
            Debug.LogError("TerrainController is not assigned!");
            hasErrors = true;
        }
        
        if (terrainController != null)
        {
            // Verificar se há grid cells na cena
            GameObject[] gridCells = FindObjectsOfType<GameObject>()
                .Where(obj => obj.name.StartsWith("Cell_")).ToArray();
            if (gridCells.Length == 0)
            {
                Debug.LogWarning("No grid cells found in scene. Try refreshing the grid.");
                hasErrors = true;
            }
            else
            {
                int activeCells = gridCells.Count(cell => cell.activeInHierarchy);
                int inactiveCells = gridCells.Length - activeCells;
                
                Debug.Log($"Found {gridCells.Length} grid cells in scene:");
                Debug.Log($"  - Active: {activeCells}");
                Debug.Log($"  - Inactive: {inactiveCells}");
                Debug.Log($"  - Current visibility state: {isGridVisible}");
            }
        }
        
        if (!hasErrors)
        {
            Debug.Log("Grid setup validation passed!");
        }
    }
    
    // Método para forçar a sincronização do estado de visibilidade
    [ContextMenu("Force Sync Visibility")]
    public void ForceSync()
    {
        if (terrainController != null)
        {
            Debug.Log($"Forcing visibility sync - Target state: {isGridVisible}");
            terrainController.ToggleCellVisibility(isGridVisible);
        }
    }
    
    private void OnGUI()
    {
        if (!enableDebugMode) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 150));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Grid Controls", GUI.skin.label);
        GUILayout.Space(5);
        
        if (GUILayout.Button(isGridVisible ? "Hide Grid" : "Show Grid"))
        {
            ToggleGrid();
        }
        
        if (GUILayout.Button("Refresh Grid"))
        {
            RefreshGrid();
        }
        
        if (GUILayout.Button("Validate Setup"))
        {
            ValidateSetup();
        }
        
        if (GUILayout.Button("Force Sync"))
        {
            ForceSync();
        }
        
        GUILayout.Space(5);
        GUILayout.Label($"Grid Visible: {isGridVisible}");
        GUILayout.Label($"Toggle Key: {toggleKey}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 