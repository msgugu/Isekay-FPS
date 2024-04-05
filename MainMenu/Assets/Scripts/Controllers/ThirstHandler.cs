using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirstHandler : MonoBehaviour
{
    public float Thirstvalue {
        get => _thirstValue;
        set => value = Mathf.Clamp(_thirstValue, 0, _maxThirst);

    }

    public float MaxThirst => _maxThirst;

    private float _thirstValue;
    private float _maxThirst = 200f;
    

    public event Action<float> OnRecoverThirst;
    public event Action<float> OnDepletThirst;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void RecoverThirst(float amount)
    {
        _thirstValue += amount;
        OnRecoverThirst.Invoke(amount);
    }

    public void DepletThirst(float amount)
    {
        _thirstValue -= amount;
        OnDepletThirst.Invoke(amount);
    }
}
