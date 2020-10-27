using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemCursorControl : MonoBehaviour
{

    public Texture2D interactCursor;

    public void CursorHover()
    {   
        // print("HOVER YOU");
        Cursor.SetCursor(interactCursor, Vector2.zero, CursorMode.Auto);
    }
    public void CursorExit()
    {
        // print("nothing to see here...");
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
