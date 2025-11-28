using UnityEngine;

public class LevelKeyboardStarter : MonoBehaviour, IDependency<LevelStateTracker>
{
    private LevelStateTracker levelStateTracker;
    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            levelStateTracker.LaunchPreparationStart();
        }
    }*/
}
