using UnityEngine;


public abstract class Setting : ScriptableObject
{
    [SerializeField] protected string title;
    public string Title => title;
    public virtual bool IsSlider {  get; protected set; }
    public virtual bool isMinValue { get; }
    public virtual bool isMaxValue { get; }

    public virtual void SetNextValue() { }
    public virtual void SetPreviousValue() { }
    public virtual void SetValue(float virtualValue) { }
    public virtual object GetValue() { return default(object); }
    public virtual string GetStringValue() { return string.Empty; }
    public virtual float GetVirtualValue() { return 0f; }
    public virtual void Apply() { }
    public virtual void Load() { }
}
