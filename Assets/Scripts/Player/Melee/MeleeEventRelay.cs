using System.Collections.Generic;
using UnityEngine;

namespace Player.Melee
{
    public class MeleeEventRelay : MonoBehaviour
    {
        [SerializeField] private MeleeController meleeController; 
        [SerializeField] private GameObject slashEffectPrefab;
        [SerializeField] private Transform slashSpawnPoint;

        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> hitSounds = new List<AudioClip>();
        [SerializeField, Range(0f, 1f)] private float pitchVariation = 0.1f;


        private int comboStep = 0;

        // En uso en Animation Events

        public void PlayAttackSound()
        {
            if (audioSource == null || hitSounds.Count == 0) return;
            AudioClip clip = hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)];
            audioSource.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            audioSource.PlayOneShot(clip);
        }

        public void ExecuteDamage()
        {
            if (meleeController != null)
            {
                meleeController.ExecuteDamage();
                comboStep = meleeController.ComboStep;
            }

            if (slashEffectPrefab != null && slashSpawnPoint != null)
            {
                // 🔹 Instanciar VFX alineado con la dirección del player
                Quaternion rotation = Quaternion.LookRotation(transform.forward) * slashSpawnPoint.rotation;
                var slashVFX = Instantiate(slashEffectPrefab, slashSpawnPoint.position, rotation);

                // 🔹 Alternar espejo según combo
                if (comboStep % 2 != 0) // impar → normal
                {
                    slashVFX.transform.localScale = Vector3.one;
                }
                else // par → espejo
                {
                    Vector3 scale = slashVFX.transform.localScale;
                    slashVFX.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                    Debug.Log("Combo espejado");
                }

                // 🔹 Destruir VFX al terminar el ParticleSystem
                var ps = slashVFX.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                    Destroy(slashVFX, ps.main.duration);
                }
                else
                {
                    Destroy(slashVFX, 1.5f);
                }
            }
        }
    }
}
