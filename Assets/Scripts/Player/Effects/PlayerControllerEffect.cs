using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy.Spawn;
using Mutations.Core.Categories;
using Mutations;

public class PlayerControllerEffect : MonoBehaviour, IOrbCollectable, IHealable
{
    private PlayerModel playerModel;
    private Dictionary<string, Coroutine> activeCoroutines = new();

    //Mutacion Alpha Nervous
    public bool AlphaMajorActive { get; private set; }
    private float alphaMajorInvulnerabilityDuration;
    public bool AlphaMinorActive { get; private set; }
    private float alphaMinorInvulnerabilityDuration;

    //Mutaciones Neutrons Nervous
    private bool neutronsActive;
    private float extraVitalTime;

    private void Awake()
    {
        playerModel = GetComponent<PlayerModel>();
        if (playerModel == null)
            Debug.LogError("PlayerControllerEffect requiere PlayerModel en el mismo GameObject.");
    }

    #region INTERFACES
    public void OnOrbCollected()
    {
        //Mutacion Alpha Mayor
        if (AlphaMajorActive)
        {
            ApplyInvulnerability(alphaMajorInvulnerabilityDuration);
            Debug.Log("[Alpha Major] Invulnerabilidad aplicada por orb.");
        }

        //Mutacion Alpha Menor
        if (AlphaMinorActive)
        {
            ApplyInvulnerability(alphaMinorInvulnerabilityDuration);
            Debug.Log("[Alpha Minor] Mini-Invulnerabilidad aplicada por orb.");
        }

        //Mutaciones Nervous
        if (neutronsActive && extraVitalTime > 0f)
        {
            RecoverTime(extraVitalTime);
            Debug.Log($"[Neutrons] +{extraVitalTime}s añadidos al tiempo vital.");
        }

    }
    #endregion

    public void RecoverTime(float timeRecovered)
    {
        playerModel.RecoverTime(timeRecovered);
    }

    #region MUTACION ALPHA
    public void SetAlphaMajor(bool active, float duration = 0f)
    {
        AlphaMajorActive = active;
        if (active)
            alphaMajorInvulnerabilityDuration = duration;
    }

    public void SetAlphaMinor(bool active, float duration = 0f)
    {
        AlphaMinorActive = active;
        if (active)
            alphaMinorInvulnerabilityDuration = duration;
    }

    public void ApplyInvulnerability(float duration)
    {
        if (activeCoroutines.ContainsKey("invulnerability"))
        {
            StopCoroutine(activeCoroutines["invulnerability"]);
            activeCoroutines.Remove("invulnerability");
        }

        playerModel.SetInvulnerable(true);
        Coroutine c = StartCoroutine(InvulnerabilityCoroutine(duration));
        activeCoroutines.Add("invulnerability", c);
    }

    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerModel.SetInvulnerable(false);
        activeCoroutines.Remove("invulnerability");
        Debug.Log("[PlayerControllerEffect] Invulnerabilidad terminada");
    }

    #endregion


    #region MUTACION NERVOUS
    public void SetNeutronsEffect(bool active, float extraTime = 0f)
    {
        neutronsActive = active;
        extraVitalTime = extraTime;
    }

    #endregion



}
