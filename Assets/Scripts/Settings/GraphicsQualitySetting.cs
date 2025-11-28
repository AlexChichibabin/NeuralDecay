using UnityEngine;

[CreateAssetMenu]
public class GraphicsQualitySetting : Setting
{
    private int currentLevelIndex;
    public override bool isMinValue { get => currentLevelIndex == 0; }
    public override bool isMaxValue { get => currentLevelIndex == QualitySettings.names.Length - 1; }


    public override object GetValue() => QualitySettings.names[currentLevelIndex];
    public override string GetStringValue() => QualitySettings.names[currentLevelIndex];
    public override void SetNextValue()
    {
        if (isMaxValue == false) currentLevelIndex++;
    }
    public override void SetPreviousValue()
    {
        if (isMinValue == false) currentLevelIndex--;
    }
    public override void Apply()
    {
        QualitySettings.SetQualityLevel(currentLevelIndex);
        Save();
    }
    public override void Load() =>
        currentLevelIndex = PlayerPrefs.GetInt(title, QualitySettings.names.Length - 1);

    private void Save() => PlayerPrefs.SetInt(title, currentLevelIndex);

}