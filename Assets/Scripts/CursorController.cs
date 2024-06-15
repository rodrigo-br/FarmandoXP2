using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D cursor;

    private void Awake()
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        //Cursor.lockState = CursorLockMode.Confined;
    }
}
