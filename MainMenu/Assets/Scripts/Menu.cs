using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 끄고 켜기 
/// </summary>
public class Menu : MonoBehaviour
{
    public string menuName;
    public bool open;
    
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;   
        gameObject.SetActive(false);
    }
}
