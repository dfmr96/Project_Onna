using Player.Stats.Meta;
using UnityEngine;

namespace Player.Stats
{
    public class PlayerStatsDebugger : MonoBehaviour
    {
        [SerializeField] private StatReferences statRefs;
        private PlayerModel player;
        private bool showDebugger = true;
        private GUIStyle emptyStyle;

        private void Start()
        {
            player = FindObjectOfType<PlayerModel>();
            emptyStyle = new GUIStyle();
            emptyStyle.normal.background = Texture2D.whiteTexture; // Necesitamos un fondo sólido para aplicar el color
        }

        private void OnGUI()
        {
            if (player == null || player.StatContext == null) return;

            // Botón flotante para mostrar/ocultar
            if (GUI.Button(new Rect(10, 10, 100, 25), showDebugger ? "Ocultar Stats" : "Mostrar Stats"))
            {
                showDebugger = !showDebugger;
            }

            // Si está colapsado, no dibujamos nada más
            if (!showDebugger) return;

            string mode = GameModeSelector.SelectedMode == GameMode.Run ? "[RUN]" : "[HUB]";
            GUILayout.BeginArea(new Rect(10, 40, 380, 320), $"🧪 Stats Debugger {mode}", GUI.skin.window);

            DrawStat("Max Vital", statRefs.maxVitalTime);
            DrawStat("Passive Drain Rate", statRefs.passiveDrainRate);
            DrawStat("Damage Resistance", statRefs.damageResistance);
            DrawStat("Speed", statRefs.movementSpeed);
            DrawStat("Damage", statRefs.damage);
            DrawStat("Overheat Cooldown", statRefs.overheatCooldown);
            DrawStat("Attack Range", statRefs.attackRange);
            DrawStat("Max Ammo", statRefs.maxAmmo);

            GUILayout.EndArea();
            
            // Si no queremos mostrar la barra fuera de Run, podemos envolverla en una condición
            if (player != null)
            {
                // Posición de la barra (cambia el rect según dónde quieras ubicarla)
                Rect healthBarRect = new Rect(Screen.width - 210, 10, 200, 25);

                // Fondo de la barra
                GUI.color = Color.gray;
                GUI.Box(healthBarRect, GUIContent.none);

                // Calcula el fill
                float normalizedHealth = Mathf.Clamp01(player.CurrentHealth / player.MaxHealth);

                // Calcula el ancho según la vida actual
                float filledWidth = healthBarRect.width * normalizedHealth;

                // Dibuja la barra de vida en verde (puedes elegir otro color)
                GUI.color = Color.green;
                GUI.Box(new Rect(healthBarRect.x, healthBarRect.y, filledWidth, healthBarRect.height), GUIContent.none, emptyStyle);


                // Texto encima de la barra
                GUI.color = Color.black;
                string healthText = $"{player.CurrentHealth:0}/{player.MaxHealth:0}";
                GUI.Label(healthBarRect, $"❤️ {healthText}");
                
                // Texto debajo de la barra de vida indicando el estado de Passive Drain
                GUI.color = player.DevMode ? Color.yellow : Color.green;
                string passiveDrainStatus = player.DevMode ? "⚠️ Passive Drain DESACTIVADO (DevMode)" : "✅ Passive Drain ACTIVO";
                Rect passiveDrainRect = new Rect(healthBarRect.x, healthBarRect.y + healthBarRect.height + 5, 300, 25);
                GUI.Label(passiveDrainRect, passiveDrainStatus);

            }
        }

        private void DrawStat(string label, StatDefinition def)
        {
            if (def == null || player.StatContext == null) return;

            float baseVal = 0f;
            float metaVal = 0f;
            float runtimeBonus = 0f;

            // Contexto: Run
            if (player.StatContext.Runtime != null)
            {
                baseVal = player.StatContext.Runtime.GetBaseValue(def);
                metaVal = player.StatContext.Meta?.Get(def) ?? 0f;
                runtimeBonus = player.StatContext.Runtime.GetBonusValue(def);
            }
            // Contexto: Hub con MetaStatReader
            else if (player.StatContext.Source is MetaStatReader reader)
            {
                baseVal = reader.GetBase(def);
                metaVal = reader.GetMeta(def);
            }
            // Fallback mínimo
            float total = player.StatContext.Source.Get(def);

            GUILayout.BeginHorizontal();

            GUILayout.Label($"{label}:", GUILayout.Width(110));

            GUI.contentColor = Color.white;
            GUILayout.Label($"B:{baseVal:0.##}", GUILayout.Width(50));

            GUI.contentColor = new Color(0.5f, 0.8f, 1f); // celeste claro
            GUILayout.Label($"M:{metaVal:0.##}", GUILayout.Width(50));

            if (GameModeSelector.SelectedMode == GameMode.Run)
            {
                GUI.contentColor = Color.green;
                GUILayout.Label($"R:{runtimeBonus:0.##}", GUILayout.Width(50));
            }

            GUI.contentColor = Color.cyan;
            GUILayout.Label($"= {total:0.##}", GUILayout.Width(60));

            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();
        }
    }
}
