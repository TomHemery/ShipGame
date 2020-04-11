using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Resource : MonoBehaviour
{
    public const float DEFAULT_MAX_VALUE = 100;
    public float Value { get; protected set; } = DEFAULT_MAX_VALUE;
    public float MaxValue { get; protected set; } = DEFAULT_MAX_VALUE;
    public event EventHandler<ResourceChangedEventArgs> ResourceValueChangedEvent;

    protected virtual void Awake() {
        
    }

    protected virtual void Update()
    {
        if(!GameManager.SimPaused) UpdateResource();
    }

    public virtual void UpdateResource() {}

    public virtual void SetResource(float r)
    {
        Value = Mathf.Clamp(r, 0, MaxValue);
        ResourceValueChangedEvent?.Invoke(this, new ResourceChangedEventArgs(Value, MaxValue));
    }

    public virtual void AddResource(float r) {
        SetResource(Value + Mathf.Abs(r));
    }

    public virtual void SubtractResource(float r) {
        SetResource(Value - Mathf.Abs(r));
    }

    public virtual void ModifyResourceBy(float r) {
        SetResource(Value + r);
    }

    public virtual void SetMaxResource(float r) {
        MaxValue = r;
        SetResource(Value);
    }

    public virtual void FillResource() {
        SetResource(MaxValue);
    }

    public virtual void EmptyResource() {
        SetResource(0);
    }

    public bool IsFull() {
        return Value >= MaxValue;
    }

    public bool IsEmpty() {
        return Value <= 0;
    }
}

public class ResourceChangedEventArgs : EventArgs
{
    public ResourceChangedEventArgs(float _newVal, float _maxVal) { NewValue = _newVal; MaxValue = _maxVal; }
    public float NewValue { get; }
    public float MaxValue { get; }
}