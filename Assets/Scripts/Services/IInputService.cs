using UnityEngine;

namespace Services
{
    public interface IInputService
    {
        Vector3 CameraRelativeInput { get; }
        Vector2 RawInput { get; }

        Vector2 GetCurrentInput();
        Vector3 GetCameraRelativeInput();
        bool HasInput();
    }
}