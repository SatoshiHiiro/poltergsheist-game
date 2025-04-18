using UnityEngine;

public class HoverMaterialSwitcher1 : MonoBehaviour
{
    public Material defaultMaterial;
    public Material hoverMaterial;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Find the SpriteRenderer in children
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null && defaultMaterial != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

    void OnMouseEnter()
    {
        if (CompareTag("Possess") && spriteRenderer != null)
        {
            spriteRenderer.material = hoverMaterial;
        }
    }

    void OnMouseExit()
    {
        if (CompareTag("Possess") && spriteRenderer != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }
}
