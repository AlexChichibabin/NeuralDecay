using UnityEngine;

public class UIResetButton : UISelectableButton, IDependency<MapCompletion> // UNUSED
{
    [SerializeField] private GameObject resetConfirmPanel;
    [SerializeField] private GameObject levelsPanel;

    private MapCompletion mapCompletion;
    public void Construct(MapCompletion obj) => mapCompletion = obj;

    public void No()
    {
        resetConfirmPanel.SetActive(false);
        SetUnfocus();
    }
    public void Yes()
    {
        mapCompletion.ResetProgress();
        resetConfirmPanel.SetActive(false);
        SetUnfocus();
    }
}