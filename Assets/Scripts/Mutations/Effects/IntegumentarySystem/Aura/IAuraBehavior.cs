using UnityEngine;

public interface IAuraBehavior
{
    void OnAuraTick(Vector3 origin, float radius, LayerMask mask);
}