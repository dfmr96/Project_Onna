#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AuraData), true)]
public class AuraDataEditor : Editor
{
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        // Forzar repaint cuando se selecciona
        SceneView.RepaintAll();
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        // Limpiar cuando se deselecciona
        SceneView.RepaintAll();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        AuraData auraData = (AuraData)target;
        if (auraData == null) return;

        // Buscar el player en la escena (buscar por tipo sin namespace)
        var players = FindObjectsOfType<MonoBehaviour>();
        Transform playerTransform = null;

        foreach (var component in players)
        {
            if (component.GetType().Name == "PlayerModel")
            {
                playerTransform = component.transform;
                break;
            }
        }

        // Si no hay player, dibujar en el origen como referencia
        Vector3 position = playerTransform != null ? playerTransform.position : Vector3.zero;

        // Usar Handles.zTest para que siempre se vea
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        // Dibujar el círculo del rango del aura con línea más gruesa
        Handles.color = new Color(auraData.auraColor.r, auraData.auraColor.g, auraData.auraColor.b, 0.8f);
        Handles.DrawWireDisc(position, Vector3.up, auraData.radius);

        // Dibujar círculos adicionales para mejor visibilidad
        Handles.color = new Color(auraData.auraColor.r, auraData.auraColor.g, auraData.auraColor.b, 0.3f);
        Handles.DrawWireDisc(position, Vector3.up, auraData.radius * 0.5f);
        Handles.DrawWireDisc(position, Vector3.up, auraData.radius * 0.75f);

        // Dibujar un círculo relleno semi-transparente
        Handles.color = new Color(auraData.auraColor.r, auraData.auraColor.g, auraData.auraColor.b, 0.15f);
        Handles.DrawSolidDisc(position, Vector3.up, auraData.radius);

        // Dibujar label con info y estilo más visible
        GUIStyle labelStyle = new GUIStyle(EditorStyles.whiteLargeLabel);
        labelStyle.normal.textColor = auraData.auraColor;
        labelStyle.fontSize = 14;
        labelStyle.fontStyle = FontStyle.Bold;

        string label = $"🎯 {auraData.auraId}\nRadius: {auraData.radius:F1}m | Tick: {auraData.tickRate:F1}s";
        if (playerTransform == null)
            label += "\n⚠️ No PlayerModel found";

        Handles.Label(position + Vector3.up * 2f, label, labelStyle);

        // Dibujar línea hacia arriba para mejor visibilidad
        Handles.color = new Color(auraData.auraColor.r, auraData.auraColor.g, auraData.auraColor.b, 0.5f);
        Handles.DrawLine(position, position + Vector3.up * 2f);
    }

    // Override del Inspector para mostrar instrucciones
    public override void OnInspectorGUI()
    {
        // Actualizar el objeto serializado antes de mostrar el inspector
        serializedObject.Update();

        // Dibujar el inspector por defecto
        DrawDefaultInspector();

        // Separador visual
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(5);

        // HelpBox con instrucciones
        EditorGUILayout.HelpBox(
            "💡 VISUALIZACIÓN DE AURA\n\n" +
            "Con este asset seleccionado, verás el rango del aura dibujado en la Scene View alrededor del Player.\n\n" +
            "Si no lo ves:\n" +
            "• Asegurate de tener Gizmos habilitados en Scene View\n" +
            "• Verifica que haya un PlayerModel en la escena\n" +
            "• Ajusta el radius arriba y observa el cambio en tiempo real",
            MessageType.Info);

        EditorGUILayout.Space(5);

        // Botón de refresh con estilo mejorado
        if (GUILayout.Button("🔄 Refrescar Scene View", GUILayout.Height(30)))
        {
            Debug.Log("[AuraDataEditor] Refrescando Scene View...");
            SceneView.RepaintAll();
        }

        EditorGUILayout.Space(5);

        // Información de debug
        AuraData auraData = (AuraData)target;
        if (auraData != null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Debug Info:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Aura ID: {auraData.auraId}");
            EditorGUILayout.LabelField($"Radius: {auraData.radius:F2}m");
            EditorGUILayout.LabelField($"Tick Rate: {auraData.tickRate:F2}s");

            // Buscar player
            var players = FindObjectsOfType<MonoBehaviour>();
            bool playerFound = false;
            foreach (var component in players)
            {
                if (component.GetType().Name == "PlayerModel")
                {
                    playerFound = true;
                    EditorGUILayout.LabelField("Player: ✓ Found", EditorStyles.boldLabel);
                    break;
                }
            }
            if (!playerFound)
            {
                EditorGUILayout.LabelField("Player: ✗ Not Found", EditorStyles.boldLabel);
            }

            EditorGUILayout.EndVertical();
        }

        // Aplicar cambios modificados
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
