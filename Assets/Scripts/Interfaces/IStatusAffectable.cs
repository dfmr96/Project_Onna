using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusAffectable
{
    void ApplyStatusEffect(StatusEffect effect);
    bool HasStatusEffect<T>() where T : StatusEffect;

}

