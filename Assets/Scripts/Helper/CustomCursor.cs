using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Sprite cursorSprite;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Cursor.SetCursor(cursorSprite.texture, Vector2.zero, CursorMode.Auto);
    }
}
