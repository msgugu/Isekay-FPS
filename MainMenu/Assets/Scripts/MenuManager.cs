using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // �̱���
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
            // ���ڷ� ���� �޴� �̸��� ���� �޴��̸� �� ���ٸ� Ű��
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            // �ƴϸ� �� ����
            else if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }
    // �ν�����â�� ���ִ� �޴��� 
    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            // �����ִ� �޴�â ����
            if (menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        // ���ڷ� ���� �޴��� Ȱ��ȭ
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
