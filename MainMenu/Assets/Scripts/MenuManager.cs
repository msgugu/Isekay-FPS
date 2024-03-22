using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // 싱글톤
    public static MenuManager instance;

    [SerializeField] Menu[] menus;

    private void Awake()
    {
        instance = this;
    }
    
    public void OpenMenu(string menuName)
    {

        for(int i = 0; i < menus.Length; i++)
        {
            // 인자로 받은 메뉴 이름과 현재 메뉴이름 과 같다면 키고
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            // 아니면 다 끄기
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }
    // 인스펙터창에 들어가있는 메뉴중 
    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            // 열려있는 메뉴창 끄고
            if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        // 인자로 받은 메뉴만 활성화
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
