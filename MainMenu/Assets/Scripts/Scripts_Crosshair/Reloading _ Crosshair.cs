using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Reload : MonoBehaviour
{
    [Header("Radial Timers")]
    [SerializeField] private float indicatorTimer = 1.0f;     // 재장전에 걸리는 시간  
    [SerializeField] private float maxIndicatorTimer = 1.0f;  // 최대 재장전 시간 (이 값을 기반으로 UI가 업데이트)

    [Header("UI Indicator")]
    [SerializeField] private Image radialIndicatorUI = null;  // R키가 눌렸을 때 나타날 Image UI

    [Header("UI Text")]
    [SerializeField] private Text TextUI = null;              // R키가 눌렸을 때 나타날 Text UI


    [Header("Key Codes")]
    [SerializeField] private KeyCode selectKey = KeyCode.R;

    [Header("Unity Envent")]
    [SerializeField] private UnityEvent myEvent = null;

    private bool shouldUpdate = false;

    private void Update()
    {
        if (Input.GetKey(selectKey))
        {
            shouldUpdate = false;
            indicatorTimer -= Time.deltaTime;
            radialIndicatorUI.enabled = true;
            radialIndicatorUI.fillAmount = indicatorTimer;
            TextUI.enabled = true; 

            if (indicatorTimer <= 0)
            {
                indicatorTimer = maxIndicatorTimer;
                radialIndicatorUI.fillAmount = maxIndicatorTimer;
                radialIndicatorUI.enabled = false;
                TextUI.enabled = false; 
                myEvent.Invoke();

            }
        }

        else
        {
            if (shouldUpdate)
            {
                indicatorTimer += Time.deltaTime;
                radialIndicatorUI.fillAmount = indicatorTimer;

                if (indicatorTimer >= maxIndicatorTimer)
                {
                    indicatorTimer = maxIndicatorTimer;
                    radialIndicatorUI.fillAmount = maxIndicatorTimer;
                    radialIndicatorUI.enabled = false;
                    TextUI.enabled = false;
                    shouldUpdate = false;
                }
            }
        }

        if (Input.GetKeyUp(selectKey))
        {
            shouldUpdate = true;
            TextUI.enabled = false;
        } 
    }
}
