using UnityEngine;
using TMPro;

public class JumpingCoinsText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private float fadeDuration = 1f;

    private TextMeshPro textMesh;
    private Color startColor;
    private float elapsed;

    private float gravity = -9.8f;
    public float jumpForce = 5f;    
    private float verticalVelocity;

    public void Initialize(float damageAmount)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = "+" + Mathf.RoundToInt(damageAmount).ToString() + "<sprite=0>";
        startColor = textMesh.color;
    }

    public void Initialize(string text, float lifeTime)
    {
        textMesh = GetComponent<TextMeshPro>();
        textMesh.text = text;
        startColor = Color.yellow;
        fadeDuration = lifeTime;
    }

    void Start()
    {
        verticalVelocity = jumpForce; 
    }

    void Update()
    {
        elapsed += Time.deltaTime;


        Vector3 move = Vector3.right * floatSpeed * Time.deltaTime;

        //parabola
        verticalVelocity += gravity * Time.deltaTime;
        move += Vector3.up * verticalVelocity * Time.deltaTime;

        transform.position += move;


        //Siempre mirar a la camara
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
        }

        //Fade out
        float fade = Mathf.Clamp01(1 - (elapsed / fadeDuration));
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, fade);

        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
