using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using Enemy.Spawn;
using Mutations.Core.Categories;
using Mutations;

public class PlayerControllerEffect : MonoBehaviour
{
    private PlayerModel playerModel;
    private Dictionary<string, Coroutine> activeCoroutines = new();

    //Mutacion Alpha
    public bool AlphaMajorActive { get; private set; }
    private float alphaMajorInvulnerabilityDuration;
    public bool AlphaMinorActive { get; private set; }
    private float alphaMinorInvulnerabilityDuration;

    private void Awake()
    {
        playerModel = GetComponent<PlayerModel>();
        if (playerModel == null)
            Debug.LogError("PlayerControllerEffect requiere PlayerModel en el mismo GameObject.");
    }

    #region MUTACION ALPHA
    public void SetAlphaMajor(bool active, float duration = 0f)
    {
        AlphaMajorActive = active;
        if (active)
            alphaMajorInvulnerabilityDuration = duration;
    }

    public void ApplyAlphaMajorEffect()
    {
        if (AlphaMajorActive)
        {
            ApplyInvulnerability(alphaMajorInvulnerabilityDuration);
            Debug.Log("[Alpha Major] Invulnerabilidad aplicada por orb.");
        }
    }

    public void SetAlphaMinor(bool active, float duration = 0f)
    {
        AlphaMinorActive = active;
        if (active)
            alphaMinorInvulnerabilityDuration = duration;
    }

    public void ApplyAlphaMinorEffect()
    {
        if (AlphaMinorActive)
        {
            ApplyInvulnerability(alphaMinorInvulnerabilityDuration);
            Debug.Log("[Alpha Minor] Invulnerabilidad aplicada por orb.");
        }
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







}
