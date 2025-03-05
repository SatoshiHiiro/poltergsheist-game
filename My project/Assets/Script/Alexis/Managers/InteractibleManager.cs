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
        baseColor = Vector4.one;
        hoverColor = new Vector4(.8f, .8f, .8f, 1);

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
