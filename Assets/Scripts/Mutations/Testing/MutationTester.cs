using System.Collections;
using NaughtyAttributes;
using Player;
using Player.Weapon;
using UnityEngine;

namespace Mutations.Testing
{
    public class MutationTester : MonoBehaviour
    {
        private PlayerModel player;
        [Header("Mutations")] public UpgradeEffect oxigenoOnna;
        public UpgradeEffect blindajeOseo;
        public UpgradeEffect miradaDelUmbral;
        public UpgradeEffect segundaAlma;
        public UpgradeEffect tacticaDeGuerra;
        public UpgradeEffect sangreMaldita;

        private void Start()
        {
            player = FindObjectOfType<PlayerModel>();
            if (player != null)
                Debug.Log("✅ PlayerModel obtenido.");
            else
                Debug.LogWarning("⛔ No se encontró PlayerModel en la escena.");
        }


        [Button("🧪 Oxígeno ONИA")]
        private void TestOxigenoOnna()
        {
            ApplyMutation(oxigenoOnna);
        }

        [Button("🧪 Blindaje Óseo")]
        private void TestBlindajeOseo()
        {
            ApplyMutation(blindajeOseo);
        }

        [Button("🧪 Mirada del Umbral")]
        private void TestMiradaDelUmbral()
        {
            ApplyMutation(miradaDelUmbral);
        }

        [Button("🧪 Segunda Alma")]
        private void TestSegundaAlma()
        {
            ApplyMutation(segundaAlma);
        }

        [Button("🧪 Táctica de Guerra")]
        private void TestTacticaDeGuerra()
        {
            ApplyMutation(tacticaDeGuerra);
        }

        [Button("🧪 Sangre Maldita")]
        private void TestSangreMaldita()
        {
            ApplyMutation(sangreMaldita);
        }

        private void ApplyMutation(UpgradeEffect effect)
        {
            if (player == null || player.StatContext.Runtime == null)
            {
                Debug.LogWarning("⛔ PlayerModel o RuntimeStats no válidos.");
                return;
            }

            Debug.Log($"✅ Aplicando mutación: {effect.GetType().Name}");
            effect.Apply(player.StatContext.Runtime);

            float val = player.StatContext.Runtime.Get(player.StatRefs.movementSpeed);
            Debug.Log($"🔍 Valor actual de movementSpeed tras aplicar: {val}");
        }
    
        [Button("🧪 Test Damage (con resistencia)")]
        private void TestDamageWithResistance()
        {
            var player = FindObjectOfType<PlayerModel>();
            if (player != null)
            {
                float testDamage = 10f;
                player.ApplyDamage(testDamage, applyResistance: true);
                Debug.Log($"🧪 Damage aplicado con resistencia: {testDamage}");
            }
            else
            {
                Debug.LogWarning("⛔ PlayerModel no encontrado en escena.");
            }
        }
    
        [Button("🧪 Test Damage (sin resistencia)")]
        private void TestDamageNoResistance()
        {
            var player = FindObjectOfType<PlayerModel>();
            if (player != null)
            {
                float testDamage = 10f;
                player.ApplyDamage(testDamage, applyResistance: false);
                Debug.Log($"🧪 Damage aplicado sin resistencia: {testDamage}");
            }
        }
    
        [Button("🧪 Test CoolingCooldown con tiempo")]
        private void TestCoolingCooldownTimed()
        {
            StartCoroutine(TestCoolingCooldownCoroutine());
        }

        private IEnumerator TestCoolingCooldownCoroutine()
        {
            var weapon = FindObjectOfType<WeaponController>();
            if (weapon == null)
            {
                Debug.LogWarning("⛔ No se encontró WeaponController.");
                yield break;
            }

            float startTime = Time.time;

            Debug.Log("🧪 Iniciando CoolingCooldown...");
            weapon.SendMessage("StartCoolingCooldown", SendMessageOptions.DontRequireReceiver);

            float expectedCooldown = weapon.Settings.CoolingCooldown;
            yield return new WaitForSeconds(expectedCooldown + 0.1f);

            float elapsed = Time.time - startTime;
            Debug.Log($"✅ CoolingCooldown completado. Duración real: {elapsed:F2} segundos (esperado: {expectedCooldown:F2})");
        }
    
        [Button("🧪 Test OverheatCooldown con tiempo")]
        private void TestOverheatCooldownTimed()
        {
            StartCoroutine(TestOverheatCooldownCoroutine());
        }

        private IEnumerator TestOverheatCooldownCoroutine()
        {
            var weapon = FindObjectOfType<WeaponController>();
            if (weapon == null)
            {
                Debug.LogWarning("⛔ No se encontró WeaponController.");
                yield break;
            }

            float startTime = Time.time;

            Debug.Log("🧪 Iniciando OverheatCooldown...");
            weapon.SendMessage("StartOverheatCooldown", SendMessageOptions.DontRequireReceiver);

            float expectedCooldown = weapon.Settings.OverheatCooldown;
            yield return new WaitForSeconds(expectedCooldown + 0.1f);

            float elapsed = Time.time - startTime;
            Debug.Log($"✅ OverheatCooldown completado. Duración real: {elapsed:F2} segundos (esperado: {expectedCooldown:F2})");
        }


    }
}