using Player.Stats.Meta;
using UnityEngine;

namespace Player.Stats
{
    public class PlayerStatsDebugger : MonoBehaviour
    {
        [SerializeField] private StatReferences statRefs;
        private PlayerModel player;
        private bool showDebugger = true;

        private void Start()
        {
            player = FindObjectOfType<PlayerModel>();
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
