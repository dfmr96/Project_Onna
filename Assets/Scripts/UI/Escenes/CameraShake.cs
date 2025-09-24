using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        if (vcam != null)
            noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            else Debug.LogWarning("CinemachineVirtualCamera no encontrado en CameraShake.");
    }

    public void Shake(float amplitude, float frequency, float duration)
    {
        if (noise == null) return;

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
        shakeTimer = duration;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.unscaledDeltaTime; // ⚡ funciona con el juego pausado
            if (shakeTimer <= 0 && noise != null)
            {
                // reset
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }
        }
    }
}
