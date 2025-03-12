using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractibleManager : MonoBehaviour
{
    Vector4 baseColor;
    Vector4 hoverColor;

    SpriteRenderer sRender;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        baseColor = GetComponent<SpriteRenderer>().color;
        hoverColor = new Vector4(.8f, .8f, .8f, 1);

        if (baseColor != Vector4.one)
        {
            hoverColor.x = baseColor.x + (1 - baseColor.x) / 2;
            hoverColor.y = baseColor.y + (1 - baseColor.y) / 2;
            hoverColor.z = baseColor.z + (1 - baseColor.z) / 2;
            hoverColor.w = baseColor.w + (1 - baseColor.w) / 2;
        }

        if (gameObject.GetComponent<SpriteRenderer>() != null)
            sRender = gameObject.GetComponent<SpriteRenderer>();
        else
            sRender = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    protected virtual void OnMouseOver()
    {
        sRender.color = hoverColor;
    }

    protected virtual void OnMouseExit()
    {
        sRender.color = baseColor;
    }
}
