using CardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIType { Random, AI_1 }
public struct AIData
{
    Dictionary<FrontType, DropPlaceTracker> selfDropPlaceTrackers;
    Dictionary<FrontType, DropPlaceTracker> enemyDropPlaceTrackers;

    Dictionary<FrontType, List<Card>> selfCards;
    Dictionary<FrontType, List<Card>> enemyCards;

    Dictionary<FrontType, int> selfFrontPower;
    Dictionary<FrontType, int> enemyFrontPower;

    int selfPower;
    int enemyPower;
    AIData(bool createData)
    {
        selfDropPlaceTrackers = new Dictionary<FrontType, DropPlaceTracker>();
        enemyDropPlaceTrackers = new Dictionary<FrontType, DropPlaceTracker>();
        selfCards = new Dictionary<FrontType, List<Card>>();
        enemyCards = new Dictionary<FrontType, List<Card>>();
        selfFrontPower = new Dictionary<FrontType, int>();
        enemyFrontPower = new Dictionary<FrontType, int>();
        selfPower = 0;
        enemyPower = 0;
    }
    public void SetData(
        Dictionary<FrontType, DropPlaceTracker> selfDropPlaceTrackers,
        Dictionary<FrontType, DropPlaceTracker> enemyDropPlaceTrackers,
        Dictionary<FrontType, List<Card>> selfCards,
        Dictionary<FrontType, List<Card>> enemyCards,
        Dictionary<FrontType, int> selfFrontPower,
        Dictionary<FrontType, int> enemyFrontPower,
        int selfPower,
        int enemyPower)
    {
        this. selfDropPlaceTrackers = selfDropPlaceTrackers;
        this.enemyDropPlaceTrackers = enemyDropPlaceTrackers;
        this.selfCards = selfCards;
        this.enemyCards = enemyCards;
        this.selfFrontPower = selfFrontPower;
        this.enemyFrontPower = enemyFrontPower;
        this.selfPower = selfPower;
        this.enemyPower = enemyPower;
    }
}
public class AICalculator : MonoBehaviour, IDependency<LevelStateTracker>, IDependency<TableCardTracker>, IDependency<MoveManager>, IDependency<LevelManager>
{
    [SerializeField] protected AIType type;

    protected LevelStateTracker levelStateTracker;
    protected TableCardTracker tableCardTracker;
    protected MoveManager moveManager;
    protected LevelManager levelManager;
    protected PlayerNum controlPlayer;

    public AIType AI_Type => type;
    public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
    public void Construct(TableCardTracker obj) => tableCardTracker = obj;
    public void Construct(MoveManager obj) => moveManager = obj;
    public void Construct(LevelManager obj) => levelManager = obj;

    #region CardData
    protected Dictionary<FrontType, DropPlaceTracker> selfDropPlaceTrackers;
    protected Dictionary<FrontType, DropPlaceTracker> enemyDropPlaceTrackers;

    protected Dictionary<FrontType, List<Card>> selfCards;
    protected Dictionary<FrontType, List<Card>> enemyCards;

    protected Dictionary<FrontType, int> selfFrontPower;
    protected Dictionary<FrontType, int> enemyFrontPower;

    protected int selfPower;
    protected int enemyPower;

    public void SetControlPlayer(PlayerNum controlPlayer) => this.controlPlayer = controlPlayer;
    protected void Pass(bool isPassed)
    {
        levelStateTracker.SetPassPlayerState(controlPlayer, isPassed);
        Debug.Log("Passed");
    }

    #endregion

    private void Start()
    {
        InitDictionaries(); // Инициализирует все словари
    }
    protected void GetCards()
    {
        ClearCardDictionaries(); // Обнуляет все словари
        UploadCards(selfDropPlaceTrackers, selfCards, selfFrontPower); // Загружает актуальные карты себя
        UploadCards(enemyDropPlaceTrackers, enemyCards, enemyFrontPower); // Загружает актуальные карты противника
        UploadPowers(); // Загружает актуальные силы сторон
    }
    protected IEnumerator AIMoveNumerator(CardDrag selectedHandCard)
    {
        if (levelStateTracker.State != LevelState.RoundAction) StopCoroutine(AIMoveNumerator(selectedHandCard));
        yield return new WaitForSeconds(1);

        if (selectedHandCard != null)
        {
            selectedHandCard.transform.SetParent(selectedHandCard.CurrentParent, false);
            selectedHandCard.GetComponentInParent<DropPlaceTracker>().CheckDropPlaceState();
            Debug.Log(selectedHandCard + "AI_Move");
        }
        moveManager.SwitchTurn();

        yield return new WaitForEndOfFrame();
        moveManager.Move(controlPlayer);
    }
    private void UploadCards(Dictionary<FrontType, DropPlaceTracker> dropPlaceTrackers, Dictionary<FrontType, List<Card>> cards, Dictionary<FrontType, int> frontPowers)
    {
        if (cards == null) return;
        for (int i = 0; i < Enum.GetNames(typeof(FrontType)).Length; i++)
        {
            if (dropPlaceTrackers[(FrontType)i].transform.childCount > 0)
                for (int j = 0; j < dropPlaceTrackers[(FrontType)i].transform.childCount; j++)
                    cards[(FrontType)i].Add(dropPlaceTrackers[(FrontType)i].transform.GetChild(j).GetComponent<Card>());
            frontPowers[(FrontType)i] = dropPlaceTrackers[(FrontType)i].PowerCount;
        }
    }
    private void UploadPowers()
    {
        for (int i = 0; i < Enum.GetNames(typeof(PlayerNum)).Length; i++)
        {
            if ((PlayerNum)i == controlPlayer)
                selfPower = tableCardTracker.PlayerCardsTable[(PlayerNum)i].TotalScores;
            else
                enemyPower = tableCardTracker.PlayerCardsTable[(PlayerNum)i].TotalScores;
        }
    }
    private void ClearCardDictionaries()
    {
        for (int i = 0; i < Enum.GetNames(typeof(FrontType)).Length; i++)
        {
            if (selfCards != null && enemyCards != null)
            {
                selfCards[(FrontType)i].Clear();
                enemyCards[(FrontType)i].Clear();
            }
        }
    }
    private void InitDictionaries()
    {
        selfDropPlaceTrackers = new Dictionary<FrontType, DropPlaceTracker>();
        enemyDropPlaceTrackers = new Dictionary<FrontType, DropPlaceTracker>();
        selfCards = new Dictionary<FrontType, List<Card>>();
        enemyCards = new Dictionary<FrontType, List<Card>>();
        selfFrontPower = new Dictionary<FrontType, int>();
        enemyFrontPower = new Dictionary<FrontType, int>();

        for (int playerIndex = 0; playerIndex < Enum.GetNames(typeof(PlayerNum)).Length; playerIndex++)
        {
            for (int i = 0; i < Enum.GetNames(typeof(FrontType)).Length; i++)
            {
                if ((PlayerNum)playerIndex == controlPlayer)
                {
                    selfDropPlaceTrackers.Add((FrontType)i, tableCardTracker.PlayerCardsTable[(PlayerNum)playerIndex].cardTrackers[(FrontType)i]);
                    selfCards.Add((FrontType)i, new List<Card>());
                    selfFrontPower.Add((FrontType)i, 0);
                }
                else
                {
                    enemyDropPlaceTrackers.Add((FrontType)i, tableCardTracker.PlayerCardsTable[(PlayerNum)playerIndex].cardTrackers[(FrontType)i]);
                    enemyCards.Add((FrontType)i, new List<Card>());
                    enemyFrontPower.Add((FrontType)i, 0);
                }
            }
        }
    }
}
