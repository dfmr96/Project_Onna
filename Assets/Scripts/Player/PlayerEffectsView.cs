using System.Collections;
using UnityEngine;

namespace Player
{
    public class PlayerEffectsView : MonoBehaviour
    {
        [Header("Visual Effects")] 
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashDuration = 0.1f;

        // Efecto Visual de Daño - cached components
        private Renderer[] _renderers;
        private Color[][] _originalColors;
        private readonly string[] _colorPropertyNames = { "_BaseColor", "_Color", "_MainColor" };

        public void Initialize()
        {
            CacheRenderers();
        }

        private void CacheRenderers()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _originalColors = new Color[_renderers.Length][];

            for (int i = 0; i < _renderers.Length; i++)
            {
                var mats = _renderers[i].materials;
                _originalColors[i] = new Color[mats.Length];

                for (int j = 0; j < mats.Length; j++)
                {
                    var mat = mats[j];
                    string property = GetColorProperty(mat);

                    if (!string.IsNullOrEmpty(property))
                        _originalColors[i][j] = mat.GetColor(property);
                }
            }
        }

        public void PlayDamageFlash()
        {
            StartCoroutine(FlashCoroutine());
        }

        private IEnumerator FlashCoroutine()
        {
            // Cambia el color
            foreach (var renderer in _renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    string property = GetColorProperty(mat);

                    if (!string.IsNullOrEmpty(property))
                        mat.SetColor(property, flashColor);
                }
            }

            yield return new WaitForSeconds(flashDuration);

            // Restaura colores
            for (int i = 0; i < _renderers.Length; i++)
            {
                var mats = _renderers[i].materials;

                for (int j = 0; j < mats.Length; j++)
                {
                    var mat = mats[j];
                    string property = GetColorProperty(mat);

                    if (!string.IsNullOrEmpty(property))
                        mat.SetColor(property, _originalColors[i][j]);
                }
            }
        }

        private string GetColorProperty(Material mat)
        {
            foreach (var prop in _colorPropertyNames)
            {
                if (mat.HasProperty(prop))
                    return prop;
            }

            return null;
        }

        public void SetFlashColor(Color color)
        {
            flashColor = color;
        }

        public void SetFlashDuration(float duration)
        {
            flashDuration = duration;
        }
    }
}