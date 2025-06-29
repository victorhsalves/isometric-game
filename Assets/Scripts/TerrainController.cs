using UnityEngine;

public class TerrainController : MonoBehaviour
{
    [SerializeField] private Terrain terrain;
    [SerializeField] private GameObject cellPrefab; // Prefab para representar cada célula
    [SerializeField] private Material cellMaterial; // Material para as células
    [SerializeField] private Material previewMaterial; // Material para preview
    [SerializeField] private Color cellColor = Color.white; // Cor das células
    [SerializeField] private float cellHeight = 0.1f; // Altura das células visuais
    
    private Grid grid;
    private GameObject[,] cellObjects; // Array para armazenar os objetos das células
    private GameObject lastHoveredCell = null; // Última célula que estava sendo hovered

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
                }
                else
                {
                    renderer.material.color = cellColor;
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
    }

    // Método para alternar a visibilidade das células
    public void ToggleCellVisibility(bool visible)
    {
        if (cellObjects == null) return;
        
        foreach (GameObject cell in cellObjects)
        {
            if (cell != null)
            {
                cell.SetActive(visible);
            }
        }
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitCell = hit.collider.gameObject;
            
            // Verificar se o objeto atingido é uma célula da grid
            if (hitCell.name.StartsWith("Cell_"))
            {
                // Se é uma célula diferente da última hovered
                if (hitCell != lastHoveredCell)
                {
                    // Restaurar o material da célula anterior
                    if (lastHoveredCell != null)
                    {
                        RestoreCellMaterial(lastHoveredCell);
                    }
                    
                    // Aplicar preview material na nova célula
                    ApplyPreviewMaterial(hitCell);
                    lastHoveredCell = hitCell;
                }
            }
            else
            {
                // Se não é uma célula, restaurar o material da última célula hovered
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

    private void ApplyPreviewMaterial(GameObject cell)
    {
        Renderer renderer = cell.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = previewMaterial;
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
            else
            {
                renderer.material.color = cellColor;
            }
        }
    }
}