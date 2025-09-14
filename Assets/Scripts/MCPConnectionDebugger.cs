using UnityEngine;

public class MCPConnectionDebugger : MonoBehaviour
{
    [Header("MCP Connection Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private Color debugColor = Color.cyan;
    
    void Awake()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> MCPConnectionDebugger iniciado en Awake()");
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> Tiempo de inicio: {System.DateTime.Now:HH:mm:ss}");
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> Escena actual: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
        }
    }
    
    void Start()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> ✅ Start() ejecutado - Unity MCP Connection CONFIRMADA");
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> FPS Target: {Application.targetFrameRate}");
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> Platform: {Application.platform}");
            
            // Mostrar información adicional del sistema
            LogSystemInfo();
        }
    }
    
    void Update()
    {
        // Debug cada 5 segundos para confirmar que el script sigue activo
        if (enableDebugLogs && Time.time % 5f < Time.deltaTime)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> ⏰ Tick cada 5s - Tiempo: {Time.time:F1}s, FPS: {(1.0f / Time.unscaledDeltaTime):F0}");
        }
    }
    
    private void LogSystemInfo()
    {
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> === INFORMACIÓN DEL SISTEMA ===");
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> Unity Version: {Application.unityVersion}");
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> Device Name: {SystemInfo.deviceName}");
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> GPU: {SystemInfo.graphicsDeviceName}");
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> RAM: {SystemInfo.systemMemorySize} MB");
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> === FIN INFORMACIÓN ===");
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> Aplicación {(hasFocus ? "ENFOCADA" : "DESENFOCADA")}");
        }
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> Aplicación {(pauseStatus ? "PAUSADA" : "REANUDADA")}");
        }
    }
    
    void OnDestroy()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>[MCP DEBUG]</color> ❌ MCPConnectionDebugger destruido - Tiempo total: {Time.time:F1}s");
        }
    }
}