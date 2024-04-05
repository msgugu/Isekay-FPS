using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour, IHP
{
    public static StatusManager Instance { get; private set; }
    public float CurrentHp { 
        get => _hp;
        set
        {
            value = Mathf.Clamp(value, 0, _maxHp);
            if (_hp == value)
                return;

            _hp = value;

            if (value <= 0)
                onHpMin?.Invoke();
            else if (value >= _maxHp)
                onHpMax?.Invoke();

            OnChangeHp?.Invoke(value);
        }
    }

    public float MaxHp => _maxHp;

    public float Thirstvalue { 
        get => _thirst;
        set => value = Mathf.Clamp(_thirst , 0, _maxThirst);
    }

    public float MaxThirst => _maxThirst;

    public float HungerValue { 
        get => _hunger; 
        set => value = Mathf.Clamp(_hunger, 0, _maxHunger); }
    public float MaxHunger { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public event Action<float> OnChangeHp;
    public event Action<float> OnRecoverHp;
    public event Action<float> OnDepleteHp;
    public event Action onHpMin;
    public event Action onHpMax;
    public event Action<float> OnRecoverThirst;
    public event Action<float> OnDepletThirst;
    public event Action<float> OnRecoverHunger;
    public event Action<float> OnDepleteHunger;

    private float _hp;
    private float _maxHp = 100;
    private float _maxThirst = 100;
    private float _thirst;
    private float _thirstDepleteRate = 0.2f;
    private float _hunger;
    private float _maxHunger = 100;
    private float _hungerDepleteRate = 0.3f;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RecoverHp(float amount)
    {
        _hp += amount;
        OnRecoverHp?.Invoke(amount);
    }

    public void DepleteHp(float amount)
    {
        _hp -= amount;
        OnDepleteHp?.Invoke(amount);
    }

    public void RecoverThirst(float amount)
    {
        _thirst += amount;
        OnRecoverThirst?.Invoke(amount);
    }

    public void DepleteThirst()
    {
        _thirst -= _thirstDepleteRate * Time.deltaTime;

        if(_thirst <= 0)
            _thirst = 0;
    }

    public void RecoverHunger()
    {
        throw new NotImplementedException();
    }
}
