using UnityEngine;
using System.Linq;

public class TerrainPreviewDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private TerrainController terrainController;
    [SerializeField] private bool enableDebugMode = true;
    [SerializeField] private bool showRaycastInfo = false;
    
    [Header("Visual Debug")]
    [SerializeField] private LineRenderer raycastLine;
    [SerializeField] private float raycastDistance = 100f;
    
    private void Start()
    {
        // Encontrar o TerrainController se não foi atribuído
        if (terrainController == null)
        {
            terrainController = FindAnyObjectByType<TerrainController>();
        }
        
        // Configurar modo debug no TerrainController
        if (terrainController != null)
        {
            terrainController.SetDebugMode(enableDebugMode);
        }
        
        // Configurar LineRenderer para visualizar o raycast
        if (raycastLine == null)
        {
            GameObject lineObj = new GameObject("RaycastDebugLine");
            lineObj.transform.SetParent(transform);
            raycastLine = lineObj.AddComponent<LineRenderer>();
            raycastLine.material = new Material(Shader.Find("Sprites/Default"));
            raycastLine.startColor = Color.red;
            raycastLine.endColor = Color.red;
            raycastLine.startWidth = 0.05f;
            raycastLine.endWidth = 0.05f;
            raycastLine.positionCount = 2;
            raycastLine.enabled = false;
        }
    }
    
    private void Update()
    {
        if (showRaycastInfo)
        {
            DebugRaycast();
        }
        
        // Toggle debug mode com a tecla F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            enableDebugMode = !enableDebugMode;
            if (terrainController != null)
            {
                terrainController.SetDebugMode(enableDebugMode);
            }
            Debug.Log($"Debug mode: {enableDebugMode}");
        }
        
        // Toggle raycast visualization com a tecla F2
        if (Input.GetKeyDown(KeyCode.F2))
        {
            showRaycastInfo = !showRaycastInfo;
            Debug.Log($"Raycast debug: {showRaycastInfo}");
        }
    }
    
    private void DebugRaycast()
    {
        Camera currentCamera = Camera.main;
        if (currentCamera == null)
        {
            currentCamera = FindAnyObjectByType<Camera>();
        }
        
        if (currentCamera == null) return;
        
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        // Visualizar o raycast
        Vector3 rayEnd = ray.origin + ray.direction * raycastDistance;
        
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            rayEnd = hit.point;
            raycastLine.startColor = Color.green;
            raycastLine.endColor = Color.green;
            
            if (enableDebugMode)
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            }
        }
        else
        {
            raycastLine.startColor = Color.red;
            raycastLine.endColor = Color.red;
            
            if (enableDebugMode)
            {
                Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);
            }
        }
        
        // Atualizar LineRenderer
        raycastLine.enabled = true;
        raycastLine.SetPosition(0, ray.origin);
        raycastLine.SetPosition(1, rayEnd);
    }
    
    [ContextMenu("Test Raycast")]
    public void TestRaycast()
    {
        Camera currentCamera = Camera.main;
        if (currentCamera == null)
        {
            currentCamera = FindAnyObjectByType<Camera>();
        }
        
        if (currentCamera == null)
        {
            Debug.LogError("No camera found!");
            return;
        }
        
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        Debug.Log($"Mouse position: {Input.mousePosition}");
        Debug.Log($"Ray origin: {ray.origin}, direction: {ray.direction}");
        
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Hit: {hit.collider.gameObject.name} at {hit.point}");
            Debug.Log($"Hit collider type: {hit.collider.GetType().Name}");
            
            // Tentar calcular coordenadas do grid se tiver TerrainController
            if (terrainController != null)
            {
                // Usar reflection para acessar métodos privados para debug
                var method = typeof(TerrainController).GetMethod("GetGridCoordinatesFromWorldPosition", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (method != null)
                {
                    Vector3Int gridPos = (Vector3Int)method.Invoke(terrainController, new object[] { hit.point });
                    Debug.Log($"Grid coordinates: {gridPos}");
                }
            }
        }
        else
        {
            Debug.Log("No hit detected");
        }
    }
    
    [ContextMenu("List All Grid Cells")]
    public void ListAllGridCells()
    {
        GameObject[] gridCells = GameObject.FindObjectsOfType<GameObject>()
            .Where(obj => obj.name.StartsWith("Cell_")).ToArray();
        Debug.Log($"Found {gridCells.Length} grid cells:");
        
        foreach (GameObject cell in gridCells)
        {
            Renderer renderer = cell.GetComponent<Renderer>();
            
            Debug.Log($"Cell: {cell.name}, Renderer: {renderer != null}, Active: {cell.activeInHierarchy}");
        }
    }
    
    [ContextMenu("Check Preview Material")]
    public void CheckPreviewMaterial()
    {
        if (terrainController == null)
        {
            Debug.LogError("TerrainController not found!");
            return;
        }
        
        // Usar reflection para acessar o previewMaterial (field privado)
        var field = typeof(TerrainController).GetField("previewMaterial", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            Material previewMat = (Material)field.GetValue(terrainController);
            if (previewMat != null)
            {
                Debug.Log($"Preview material found: {previewMat.name}");
            }
            else
            {
                Debug.LogError("Preview material is NULL! Please assign a preview material in the TerrainController.");
            }
        }
    }
    
    private void OnGUI()
    {
        if (!enableDebugMode) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Terrain Preview Debug", GUI.skin.label);
        GUILayout.Space(5);
        
        if (GUILayout.Button("Test Raycast"))
        {
            TestRaycast();
        }
        
        if (GUILayout.Button("List Grid Cells"))
        {
            ListAllGridCells();
        }
        
        if (GUILayout.Button("Check Preview Material"))
        {
            CheckPreviewMaterial();
        }
        
        GUILayout.Space(5);
        GUILayout.Label("Controls:");
        GUILayout.Label("F1 - Toggle Debug Mode");
        GUILayout.Label("F2 - Toggle Raycast Visualization");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 