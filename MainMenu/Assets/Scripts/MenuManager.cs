using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메인 메뉴 관리
/// </summary>
public class MenuManager : MonoBehaviour
{
    
    public static MenuManager instance;

    [SerializeField] Menu[] menus;

    private void Awake()
    {
        instance = this;
    }
    /// <summary>
    /// 선택한 메뉴 켜고 나머지 다끄기
    /// </summary>
    /// <param name="menuName"></param>
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
    /// <summary>
    /// 인스펙터창에 들어가있는 메뉴중 
    /// </summary>
    /// <param name="menu"></param>
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
