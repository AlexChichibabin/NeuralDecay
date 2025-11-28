using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class AudioMixerFloatSetting : Setting
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string settingTitle;

    [SerializeField] private bool isSlider;
    [SerializeField] private float minRealValue;
    [SerializeField] private float maxRealValue;
    [SerializeField] private float virtualStep;
    [SerializeField] private float minVirtualValue;
    [SerializeField] private float maxVirtualValue;
    private float currentValue = 0;

    public override bool IsSlider => isSlider;
    public override bool isMinValue { get => currentValue == minRealValue; }
    public override bool isMaxValue { get => currentValue == maxRealValue; }


    public override void SetNextValue() => AddValue(Mathf.Abs((maxRealValue - minRealValue) / virtualStep));
    public override void SetPreviousValue() => AddValue(-Mathf.Abs((maxRealValue - minRealValue) / virtualStep));
    public override object GetValue() => currentValue;
    public override void Load() => currentValue = PlayerPrefs.GetFloat(title, 0);
    public override float GetVirtualValue() =>
    Mathf.Lerp(minVirtualValue, maxVirtualValue, (currentValue - minRealValue) / (maxRealValue - minRealValue));
    public override void SetValue(float virtualValue)
    {
        currentValue = Mathf.Lerp(minRealValue, maxRealValue, (virtualValue - minVirtualValue) / (maxVirtualValue - minVirtualValue));
        currentValue = Mathf.Clamp(currentValue, minRealValue, maxRealValue);
    }
    public override string GetStringValue()
    {
        int value = (int)Mathf.Lerp(minVirtualValue, maxVirtualValue,
            (currentValue - minRealValue) / (maxRealValue - minRealValue));
        return value.ToString();
    }
    public override void Apply()
    {
        audioMixer.SetFloat(settingTitle, currentValue);
        Save();
    }

    private void Save() => PlayerPrefs.SetFloat(title, currentValue);
    private void AddValue(float value)
    {
        currentValue += value;
        currentValue = Mathf.Clamp(currentValue, minRealValue, maxRealValue);
    }
}