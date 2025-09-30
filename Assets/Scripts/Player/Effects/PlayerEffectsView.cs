using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerEffectsView : MonoBehaviour
    {
        [Header("Material Flash")]
        [SerializeField] private Material damageMaterial;  
        [SerializeField] private float flashDuration = 0.1f;

        private Renderer[] _renderers;
        private Material[][] _originalMaterials;
        private Coroutine flashCoroutine;

        [Header("Dash Effects")]
        [SerializeField] private ParticleSystem dashStartParticlesPrefab;
        [SerializeField] private GameObject dashTrailPrefab;

        private GameObject activeTrail;


        public void Initialize()
        {
            CacheRenderers();
        }

        private void CacheRenderers()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _originalMaterials = new Material[_renderers.Length][];

            for (int i = 0; i < _renderers.Length; i++)
            {
                var mats = _renderers[i].materials;
                _originalMaterials[i] = new Material[mats.Length];
                for (int j = 0; j < mats.Length; j++)
                {
                    _originalMaterials[i][j] = mats[j];
                }
            }
        }

        public void PlayDamageFlash()
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                RestoreOriginalMaterials();
            }
            flashCoroutine = StartCoroutine(FlashCoroutine());
        }

        public void PlayDashStart(Vector3 position, Transform parent)
        {
            if (dashStartParticlesPrefab != null)
            {
                ParticleSystem ps = Instantiate(dashStartParticlesPrefab, position, Quaternion.identity);
                ps.Play();
                Destroy(ps.gameObject, 2f);
            }

            // instanciamos el trail y lo parentamos al player
            if (dashTrailPrefab != null)
            {
                activeTrail = Instantiate(dashTrailPrefab, position, Quaternion.identity, parent);
            }
        }

        public void PlayDashEnd(Vector3 position)
        {
            if (activeTrail != null)
            {
                activeTrail.transform.parent = null;
                Destroy(activeTrail, 1f); // tiempo suficiente para que se desvanezca
                activeTrail = null;
            }
        }

        public void PlayDashTrail(Vector3 startPos, Vector3 endPos)
        {
            if (dashTrailPrefab == null) return;

            GameObject trail = Instantiate(dashTrailPrefab);
            LineRenderer lr = trail.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.positionCount = 2;
                lr.SetPosition(0, startPos);
                lr.SetPosition(1, endPos);
            }

            Destroy(trail, 1.5f); // lo limpiamos después
        }

        private IEnumerator FlashCoroutine()
        {
            // Aplicar el mismo material de daño a todos los slots
            for (int i = 0; i < _renderers.Length; i++)
            {
                var mats = _renderers[i].materials;
                for (int j = 0; j < mats.Length; j++)
                {
                    mats[j] = damageMaterial;
                }
                _renderers[i].materials = mats;
            }

            yield return new WaitForSeconds(flashDuration);

            RestoreOriginalMaterials();
            flashCoroutine = null;
        }

        private void RestoreOriginalMaterials()
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].materials = _originalMaterials[i];
            }
        }
    }
}
