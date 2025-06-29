using UnityEngine;

public class TerrainController : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject cellPrefab; // Prefab para representar cada célula
    [SerializeField] private Material cellMaterial; // Material para as células
    [SerializeField] private Material previewMaterial; // Material para preview
    [SerializeField] private Color cellColor = Color.white; // Cor das células
    [SerializeField] private float cellHeight = 0.1f; // Altura das células visuais
    [SerializeField] private bool enableDebugLogs = false; // Para debug do sistema de preview
    
    private Grid grid;
    private GameObject[,] cellObjects; // Array para armazenar os objetos das células
    private GameObject lastHoveredCell = null; // Última célula que estava sendo hovered
    private Material originalCellMaterial; // Material original da célula para restauração

    private void Start()
    {
        grid = terrain.GetComponent<Grid>();
        if (grid != null)
        {
            CreateCellVisualization();
        }
        else
        {
            Debug.LogError("Grid component not found on terrain!");
        }
    }

    private void Update()
    {
        HandleCellPreview();
    }

    private void CreateCellVisualization()
    {
        // Obter as dimensões do terreno
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;
        
        // Calcular o número de células baseado no tamanho do terreno e tamanho da célula
        Vector3 cellSize = grid.cellSize;
        int cellCountX = Mathf.RoundToInt(terrainSize.x / cellSize.x);
        int cellCountZ = Mathf.RoundToInt(terrainSize.z / cellSize.z);
        
        cellObjects = new GameObject[cellCountX, cellCountZ];
        
        // Criar visualização para cada célula
        for (int x = 0; x < cellCountX; x++)
        {
            for (int z = 0; z < cellCountZ; z++)
            {
                CreateCellVisual(x, z, cellSize);
            }
        }
        
        Debug.Log($"Created {cellCountX * cellCountZ} cell visuals");
    }

    private void CreateCellVisual(int x, int z, Vector3 cellSize)
    {
        // Calcular a posição da célula no mundo
        Vector3 worldPosition = terrain.transform.position + new Vector3(
            x * cellSize.x + cellSize.x * 0.5f,
            0,
            z * cellSize.z + cellSize.z * 0.5f
        );
        
        // Obter a altura do terreno naquela posição
        float terrainHeight = terrain.SampleHeight(worldPosition);
        worldPosition.y = terrainHeight + cellHeight * 0.5f;
        
        // Criar o objeto da célula
        GameObject cellObject;
        
        if (cellPrefab != null)
        {
            // Usar o prefab fornecido
            cellObject = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform);
        }
        else
        {
            // Criar um cubo simples como fallback
            cellObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cellObject.transform.SetParent(transform);
            cellObject.transform.position = worldPosition;
            cellObject.transform.localScale = new Vector3(cellSize.x * 0.9f, cellHeight, cellSize.z * 0.9f);
            
            // Aplicar material e cor
            Renderer renderer = cellObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (cellMaterial != null)
                {
                    renderer.material = cellMaterial;
                    originalCellMaterial = cellMaterial;
                }
                else
                {
                    renderer.material.color = cellColor;
                    originalCellMaterial = renderer.material;
                }
            }
            else
            {
                Debug.LogWarning($"Renderer component not found on cell object {cellObject.name}");
            }
        }
        
        // Nomear o objeto para identificação
        cellObject.name = $"Cell_{x}_{z}";
        
        // Armazenar referência
        cellObjects[x, z] = cellObject;
        
        if (enableDebugLogs && (x == 0 && z == 0))
            Debug.Log($"Created cell {cellObject.name} at position {worldPosition}");
    }

    // Método para alternar a visibilidade das células
    public void ToggleCellVisibility(bool visible)
    {
        if (cellObjects == null) 
        {
            if (enableDebugLogs)
                Debug.LogWarning("ToggleCellVisibility called but cellObjects is null");
            return;
        }
        
        int totalCells = 0;
        int affectedCells = 0;
        
        foreach (GameObject cell in cellObjects)
        {
            totalCells++;
            if (cell != null)
            {
                cell.SetActive(visible);
                affectedCells++;
            }
        }
        
        if (enableDebugLogs)
            Debug.Log($"ToggleCellVisibility({visible}): {affectedCells}/{totalCells} cells affected");
    }

    // Método para destruir todas as células visuais
    public void ClearCellVisualization()
    {
        if (cellObjects == null) return;
        
        foreach (GameObject cell in cellObjects)
        {
            if (cell != null)
            {
                DestroyImmediate(cell);
            }
        }
        
        cellObjects = null;
    }

    // Método para recriar a visualização
    public void RefreshCellVisualization()
    {
        ClearCellVisualization();
        CreateCellVisualization();
    }

    private void HandleCellPreview()
    {
        if (cellObjects == null || previewMaterial == null) return;

        // Verificar se existe uma câmera
        Camera currentCamera = Camera.main;
        if (currentCamera == null)
        {
            currentCamera = FindAnyObjectByType<Camera>();
            if (currentCamera == null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("No camera found for cell preview");
                return;
            }
        }

        // Fazer raycast contra o terreno para obter a posição no mundo
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            // Converter a posição do hit para coordenadas do grid
            Vector3 worldPosition = hit.point;
            Vector3Int gridPosition = GetGridCoordinatesFromWorldPosition(worldPosition);
            
            if (enableDebugLogs)
                Debug.Log($"Mouse world pos: {worldPosition}, Grid pos: {gridPosition}");
            
            // Verificar se as coordenadas do grid são válidas
            if (IsValidGridPosition(gridPosition.x, gridPosition.z))
            {
                GameObject currentCell = cellObjects[gridPosition.x, gridPosition.z];
                
                // Se é uma célula diferente da última hovered
                if (currentCell != lastHoveredCell)
                {
                    // Restaurar o material da célula anterior
                    if (lastHoveredCell != null)
                    {
                        RestoreCellMaterial(lastHoveredCell);
                    }
                    
                    // Aplicar preview material na nova célula
                    if (currentCell != null)
                    {
                        ApplyPreviewMaterial(currentCell);
                        lastHoveredCell = currentCell;
                        
                        if (enableDebugLogs)
                            Debug.Log($"Applied preview to grid cell [{gridPosition.x}, {gridPosition.z}]: {currentCell.name}");
                    }
                }
            }
            else
            {
                // Se as coordenadas não são válidas, restaurar o material da última célula hovered
                if (lastHoveredCell != null)
                {
                    RestoreCellMaterial(lastHoveredCell);
                    lastHoveredCell = null;
                }
            }
        }
        else
        {
            // Se não há hit, restaurar o material da última célula hovered
            if (lastHoveredCell != null)
            {
                RestoreCellMaterial(lastHoveredCell);
                lastHoveredCell = null;
            }
        }
    }

    private Vector3Int GetGridCoordinatesFromWorldPosition(Vector3 worldPosition)
    {
        // Converter posição do mundo para posição relativa ao terreno
        Vector3 terrainLocalPosition = worldPosition - terrain.transform.position;
        
        // Calcular as coordenadas do grid baseado no tamanho da célula
        Vector3 cellSize = grid.cellSize;
        int x = Mathf.FloorToInt(terrainLocalPosition.x / cellSize.x);
        int z = Mathf.FloorToInt(terrainLocalPosition.z / cellSize.z);
        
        return new Vector3Int(x, 0, z);
    }

    private bool IsValidGridPosition(int x, int z)
    {
        if (cellObjects == null) return false;
        
        int maxX = cellObjects.GetLength(0);
        int maxZ = cellObjects.GetLength(1);
        
        return x >= 0 && x < maxX && z >= 0 && z < maxZ;
    }

    private void ApplyPreviewMaterial(GameObject cell)
    {
        Renderer renderer = cell.GetComponent<Renderer>();
        if (renderer != null && previewMaterial != null)
        {
            // Salvar o material original se ainda não foi salvo
            if (originalCellMaterial == null)
            {
                originalCellMaterial = renderer.material;
            }
            
            renderer.material = previewMaterial;
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogWarning($"Cannot apply preview material - Renderer: {renderer != null}, PreviewMaterial: {previewMaterial != null}");
        }
    }

    private void RestoreCellMaterial(GameObject cell)
    {
        Renderer renderer = cell.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (cellMaterial != null)
            {
                renderer.material = cellMaterial;
            }
            else if (originalCellMaterial != null)
            {
                renderer.material = originalCellMaterial;
            }
            else
            {
                renderer.material.color = cellColor;
            }
        }
    }

    // Método público para ativar/desativar debug logs
    public void SetDebugMode(bool enable)
    {
        enableDebugLogs = enable;
    }
}