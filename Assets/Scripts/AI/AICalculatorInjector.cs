using CardGame;
using System.Collections.Generic;
using UnityEngine;

public class AICalculatorInjector : MonoBehaviour, IDependency<LevelManager>
{
    [SerializeField] private Transform calculatorsContainer;
    private Dictionary<AIType, IAICalculator> calculator;

    private OpponentAI opponentAI;
    private AIType AIMoveTypeType;
    private LevelManager levelManager;
    public void Construct(LevelManager obj) => levelManager = obj;

    private void Awake()
    {
        opponentAI = GetComponentInChildren<OpponentAI>();
        InitiateCalculators();
        AIMoveTypeType = levelManager.AIType;
        IAICalculator calculator;
        switch (AIMoveTypeType)
        {
            case AIType.Random:
                calculator = this.calculator[AIType.Random];
                break;
            case AIType.AI_1:
                calculator = this.calculator[AIType.AI_1];
                break;
            default:
                calculator = this.calculator[AIType.Random];
                break;
        }

        opponentAI.InjectCalculator(calculator);
    }
    private void InitiateCalculators()
    {
        calculator = new Dictionary<AIType, IAICalculator>();

        for (int i = 0; i < calculatorsContainer.childCount; i++)
        {
            Transform calculator = calculatorsContainer.GetChild(i);
            if (calculator.GetComponent<AICalculator>() == null) return;
            this.calculator.Add(calculator.GetComponent<AICalculator>().AI_Type,
                calculator.GetComponent<IAICalculator>());
            calculator.GetComponent<AICalculator>().SetControlPlayer(opponentAI.GetComponent<Player>().PlayerNum);
        }
    }
}
