using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D neutralCursor;
    public Texture2D hoverCursor;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.SetCursor(neutralCursor, hotspot, cursorMode);
    }

    void Update()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Possess"))
        {
            Cursor.SetCursor(hoverCursor, hotspot, cursorMode);
        }
        else
        {
            Cursor.SetCursor(neutralCursor, hotspot, cursorMode);
        }
    }
}
