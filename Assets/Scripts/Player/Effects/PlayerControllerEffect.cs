using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy.Spawn;
using Mutations.Core.Categories;
using Mutations;
using Mutations.Core;

public class PlayerControllerEffect : MonoBehaviour, IOrbCollectable, IHealable
{
    private PlayerModel playerModel;

    private Dictionary<string, Coroutine> activeCoroutines = new();

    private List<BulletModifierSO> activeBulletModifiers = new List<BulletModifierSO>();


    //Mutacion Alpha Nervous
    public bool AlphaMajorActive { get; private set; }
    private float alphaMajorInvulnerabilityDuration;
    public bool AlphaMinorActive { get; private set; }
    private float alphaMinorInvulnerabilityDuration;

    //Mutaciones Neutrons Nervous
    private bool neutronsActive;
    private float extraVitalTime;

    //Mutaciones Nentrons Muscular
    private float enemyKilled = 0;



    private void Awake()
    {
        playerModel = GetComponent<PlayerModel>();
        if (playerModel == null)
            Debug.LogError("PlayerControllerEffect requiere PlayerModel en el mismo GameObject.");
    }

    public List<RadiationEffect> GetActiveMutations()
    {
        List<RadiationEffect> active = new List<RadiationEffect>();

        foreach (var system in MutationManager.Instance.Systems)
        {
            if (system.MajorSlot.ActiveEffect != null)
                active.Add(system.MajorSlot.ActiveEffect);

            if (system.MinorSlot.ActiveEffect != null)
                active.Add(system.MinorSlot.ActiveEffect);
        }

        return active;
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


    public void RecoverTime(float timeRecovered)
    {
        playerModel.RecoverTime(timeRecovered);
    }

    #endregion





    #region BALUBIS

    public void AddBulletModifier(BulletModifierSO modifier)
    {
        if (!activeBulletModifiers.Contains(modifier))
            activeBulletModifiers.Add(modifier);
    }

    public void RemoveBulletModifier(BulletModifierSO modifier)
    {
        activeBulletModifiers.Remove(modifier);
    }

    public List<BulletModifierSO> GetActiveBulletModifiers()
    {
        return activeBulletModifiers;
    }

    #endregion






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





    #region MUTACION NEUTRONS
    public void SetNeutronsEffect(bool active, float extraTime = 0f)
    {
        neutronsActive = active;
        extraVitalTime = extraTime;
    }

    #endregion


    #region MUTACION MUSCULAR
    public void SetMuscularNeutronsMajor() => DeathManager.Instance.OnEnemyDeath += ApplyMuscularNeutronsMajor;
    public void SetMuscularNeutronsMinor() => DeathManager.Instance.OnEnemyDeath += ApplyMuscularNeutronsMinor;

    public void ApplyMuscularNeutronsMajor()
    {
        enemyKilled++;
        if (enemyKilled >= 9)
            playerModel.RecoverTime(10);
    }
    public void ApplyMuscularNeutronsMinor()
    {
        
    }

    #endregion

}
