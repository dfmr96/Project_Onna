using UnityEngine;

public interface IPushable
{
    /// <summary>
    /// Applies a pushback force to the entity.
    /// </summary>
    /// <param name="direction">Normalized direction vector</param>
    /// <param name="force">Force magnitude</param>
    void ApplyPushback(Vector3 direction, float force);
}
