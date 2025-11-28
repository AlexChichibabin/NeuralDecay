using UnityEngine;

[CreateAssetMenu]
public class ResolutionSetting : Setting
{
    [SerializeField]
    private Vector2Int[] availableResolutions = new Vector2Int[]
    {
            new Vector2Int(800, 600),
            new Vector2Int(1200, 720),
            new Vector2Int(1600, 900),
            new Vector2Int(1920, 1080),
            new Vector2Int(2560, 1440)
    };

    private int currentResolutionIndex = 0;
    private int maxIndex;
    public override bool isMinValue => currentResolutionIndex == 0;
    public override bool isMaxValue => currentResolutionIndex == maxIndex;
    public override void SetNextValue()
    {
        if (isMaxValue == false)
        {
            currentResolutionIndex++;
        }
    }
    public override void SetPreviousValue()
    {
        if (isMinValue == false)
        {
            currentResolutionIndex--;
        }
    }
    public override object GetValue()
    {
        return availableResolutions[currentResolutionIndex];
    }
    public override string GetStringValue()
    {
        return availableResolutions[currentResolutionIndex].x + "x" +
            availableResolutions[currentResolutionIndex].y;
    }
    public override void Apply()
    {
        Screen.SetResolution(availableResolutions[currentResolutionIndex].x,
                             availableResolutions[currentResolutionIndex].y, true);
        Save();
    }
    public override void Load()
    {
        GetAvailableResolutions();
        currentResolutionIndex = maxIndex;
        Saver<int>.TryLoad("ResolutionSave", ref currentResolutionIndex);
    }
    private void Save()
    {
        Saver<int>.Save("ResolutionSave", currentResolutionIndex);
    }
    private void GetAvailableResolutions()
    {
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            if (availableResolutions[i].x > Screen.resolutions[Screen.resolutions.Length - 1].width)
            {
                maxIndex = i - 1;
                break;
            }
            else maxIndex = i;
        }
    }
}