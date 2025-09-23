using UnityEngine;

public class UI_MainMenu_Eye_Folow : MonoBehaviour
{
    [Header("Elementos del ojo")]
    public RectTransform sclera;     
    public RectTransform pupil1;     
    public RectTransform pupil2;     

    [Header("Canvas")]
    public Canvas canvas;

    [Header("Movimiento")]
    public float speed = 10f;
    public float pupilIntensity = 30f;
    public float scleraFollowFactor = 0.2f; 
    public float pixelsPerUnit = 1f;

    [Header("Rotación")]
    public float maxRotation = 15f;

    private RectTransform parentRect;
    private Vector2 scleraInitialPos;

    void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        parentRect = GetComponent<RectTransform>();
        scleraInitialPos = sclera.anchoredPosition;
    }

    void Update()
    {
        if (canvas != null && pupil1 != null && pupil2 != null && sclera != null)
        {
            EyesAimUI();
        }
    }

    void EyesAimUI()
    {
        Vector2 localMousePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localMousePos
        );

        // --- Pupila 1 ---
        Vector2 pupilOffset = Vector2.ClampMagnitude(localMousePos, pupilIntensity);
        Vector2 targetPupil1Pos = Vector2.Lerp(pupil1.anchoredPosition, pupilOffset, speed * Time.deltaTime);

        targetPupil1Pos.x = Mathf.Round(targetPupil1Pos.x * pixelsPerUnit) / pixelsPerUnit;
        targetPupil1Pos.y = Mathf.Round(targetPupil1Pos.y * pixelsPerUnit) / pixelsPerUnit;

        pupil1.anchoredPosition = targetPupil1Pos;

        if (pupilOffset.sqrMagnitude > 0.01f)
        {
            float angle1 = Mathf.Atan2(pupilOffset.y, pupilOffset.x) * Mathf.Rad2Deg;
            angle1 = Mathf.Clamp(angle1, -maxRotation, maxRotation);
            pupil1.localRotation = Quaternion.Lerp(pupil1.localRotation, Quaternion.Euler(0, 0, angle1), speed * Time.deltaTime);
        }

        // --- Pupila 2 ---
        Vector2 targetPupil2Pos = Vector2.Lerp(pupil2.anchoredPosition, pupilOffset, speed * Time.deltaTime);

        targetPupil2Pos.x = Mathf.Round(targetPupil2Pos.x * pixelsPerUnit) / pixelsPerUnit;
        targetPupil2Pos.y = Mathf.Round(targetPupil2Pos.y * pixelsPerUnit) / pixelsPerUnit;

        pupil2.anchoredPosition = targetPupil2Pos;

        if (pupilOffset.sqrMagnitude > 0.01f)
        {
            float angle2 = Mathf.Atan2(pupilOffset.y, pupilOffset.x) * Mathf.Rad2Deg;
            angle2 = Mathf.Clamp(angle2, -maxRotation, maxRotation);
            // Rotación invertida
            angle2 = -angle2;
            pupil2.localRotation = Quaternion.Lerp(pupil2.localRotation, Quaternion.Euler(0, 0, angle2), speed * Time.deltaTime);
        }

        Vector2 scleraOffset = pupilOffset * scleraFollowFactor;
        Vector2 targetScleraPos = Vector2.Lerp(sclera.anchoredPosition, scleraInitialPos + scleraOffset, speed * Time.deltaTime);

        targetScleraPos.x = Mathf.Round(targetScleraPos.x * pixelsPerUnit) / pixelsPerUnit;
        targetScleraPos.y = Mathf.Round(targetScleraPos.y * pixelsPerUnit) / pixelsPerUnit;

        sclera.anchoredPosition = targetScleraPos;
    }
}
