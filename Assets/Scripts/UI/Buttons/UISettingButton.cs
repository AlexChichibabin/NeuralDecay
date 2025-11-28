using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISettingButton : UISelectableButton, IScriptableObjectProperty
{
    [SerializeField] private Setting setting;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image previousImage;
    [SerializeField] private Image nextImage;


    [Header("ValueType")]
    [SerializeField] private GameObject commonValueGameObjects;
    [SerializeField] private GameObject sliderValueGameObjects;
    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private Slider slider;

    private void Start()
    {
        HideStartSelection();
        ApplyProperty(setting);

        if(setting.IsSlider == true) slider.onValueChanged.AddListener(SetValue);
    }
    [ContextMenu(nameof(ApplyValueType))]
    public void ApplyValueType()
    {
        if (commonValueGameObjects == null || sliderValueGameObjects == null) return;
        commonValueGameObjects.SetActive(false);
        sliderValueGameObjects.SetActive(false);
        if (setting.IsSlider == true) sliderValueGameObjects.SetActive(true);
        else commonValueGameObjects.SetActive(true);
    }
    private void OnDestroy()
    {
        if (setting.IsSlider == true) slider.onValueChanged.RemoveListener(SetValue);
    }
    public void SetNextValueSetting()
    {
        setting?.SetNextValue();
        setting?.Apply();
        UpdateInfo();
    }
    public void SetPreviousValueSetting()
    {
        setting?.SetPreviousValue();
        setting?.Apply();
        UpdateInfo();
    }
    private void SetValue(float sliderVirtualValue)
    {
        setting?.SetValue(sliderVirtualValue);
        setting?.Apply();
        UpdateInfo();
    }
    private void UpdateInfo()
    {
        titleText.text = setting.Title;
        valueText.text = setting.GetStringValue();
        sliderText.text = setting.GetStringValue();

        previousImage.enabled = !setting.isMinValue;
        nextImage.enabled = !setting.isMaxValue;
    }
    public void ApplyProperty(ScriptableObject property)
    {
        if (property == null) return;

        if (property is Setting == false) return;
        setting = property as Setting;
        ApplyValueType();
        if (setting.IsSlider == true) slider.value = (float)setting.GetVirtualValue();

        UpdateInfo();
    }
}