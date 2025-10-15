using UnityEngine;

public static class CursorHelper
{
    public static void Show()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void Hide()
    {
        Cursor.visible = false;
    }
}
