using CardGame;
using System.Collections.Generic;
using UnityEngine;

public class EffectCounterInjector : MonoBehaviour, IDependency<LevelStateTracker>, IDependency<LevelManager>
{
    [SerializeField] private Transform calculatorsContainer;
    private DropPlaceTracker dropPlaceTracker;
    private PowerCalculateType powerCalculateType;
    private LevelManager levelManager;
    private LevelStateTracker levelStateTracker;
    private Dictionary<PowerCalculateType, IEffectCounter> calculator;

    public void Construct(LevelManager obj) => levelManager = obj;
    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;

    private void Awake()
    {
        dropPlaceTracker = GetComponentInChildren<DropPlaceTracker>();
        InitiateCalculators();
        powerCalculateType = levelManager.PowerCalculateType;
        IEffectCounter calculator;
        switch (powerCalculateType)
        {
            case PowerCalculateType.Basic:
                calculator = this.calculator[PowerCalculateType.Basic];
                break;
            case PowerCalculateType.Advance:
                calculator = this.calculator[PowerCalculateType.Advance];
                break;
            default:
                calculator = this.calculator[PowerCalculateType.Basic];
                break;
        }            

        dropPlaceTracker.InjectCalculator(calculator);
    }
    private void InitiateCalculators()
    {
        calculator = new Dictionary<PowerCalculateType, IEffectCounter>();

        for (int i = 0; i < calculatorsContainer.childCount; i++)
        {
            Transform calculator = calculatorsContainer.GetChild(i);
            this.calculator.Add(calculator.GetComponent<EffectCounter>().PowerCalculateType,
                calculator.GetComponent<IEffectCounter>());
            calculator.GetComponent<EffectCounter>().SetDropPlaceTracker(dropPlaceTracker);
        }
    }
}
