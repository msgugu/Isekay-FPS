using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpHandler : MonoBehaviour, IHP
{
    public float CurrentHp
    {
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

    public event Action<float> OnChangeHp;
    public event Action<float> OnRecoverHp;
    public event Action<float> OnDepleteHp;
    public event Action onHpMin;
    public event Action onHpMax;

    private float _hp;
    private float _maxHp = 100;

    public void DepleteHp(float amount)
    {
        _hp -= amount;
        OnDepleteHp?.Invoke(amount);
    }

    public void RecoverHp(float amount)
    {
        _hp += amount;
        OnRecoverHp?.Invoke(amount);
    }

   
}
