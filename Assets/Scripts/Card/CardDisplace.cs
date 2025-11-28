using UnityEngine;
using UnityEngine.EventSystems;

namespace CardGame
{
    [RequireComponent(typeof(CardDrag))]
    public class CardDisplace : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private CardDrag cardDrag;
        private CardInfo info;
        private int displacerNewSiblingIndex;

        private LevelManager levelManager => cardDrag.Card.LevelManager;
        private GameObject tempCard => cardDrag.TempCard;
        private bool isMenu => levelManager.SceneName == levelManager.MainMenuSceneName;

        private void Awake() => cardDrag = GetComponent<CardDrag>();
        private void Start() => info = cardDrag.Card.Info;

        public void OnDrop(PointerEventData eventData)
        {
            CardDrag cardDrag = eventData.pointerDrag.GetComponent<CardDrag>();

            if (cardDrag == null) return;

            if (this.cardDrag.Card.Info != null)
            {
                if (cardDrag.Card.Info.IsDisplacer == true &&
                cardDrag.Card.ControlPlayer == this.cardDrag.Card.ControlPlayer &&
                this.cardDrag.Card.Info.CardTypeBase == CardTypeBase.CommonType)
                {
                    if (GetComponent<TempCard>() != null || transform.parent.GetComponent<DropPlace>().FrontType == FrontType.Common ||
                        GetComponent<Card>().Info.CardTypeBase == CardTypeBase.EliteType)
                    {
                        if (GetComponent<TempCard>() == null) this.cardDrag.Card.SetStrokeColor(this.cardDrag.Card.DefaultStrokeColor);
                        return;
                    }
                    displacerNewSiblingIndex = transform.GetSiblingIndex();
                    this.cardDrag.CurrentParent = cardDrag.CurrentParent;
                    this.cardDrag.SetInteractivity(true);
                    this.cardDrag.Card.SetAbilityActive(true);
                    this.cardDrag.Card.SetStrokeColor(levelManager.MainGreenColor);
                    this.cardDrag.Card.SetPower(this.cardDrag.Card.Info.DefaultPower);

                    cardDrag.CurrentParent = transform.parent;
                    cardDrag.transform.SetParent(cardDrag.CurrentParent);
                    cardDrag.transform.SetSiblingIndex(displacerNewSiblingIndex);
                    cardDrag.Card.SetAbilityActive(false);

                    transform.SetParent(this.cardDrag.CurrentParent);
                    transform.SetSiblingIndex(cardDrag.DisplacingCardIndex);

                    StartCoroutine(cardDrag.transform.GetComponentInParent<DropPlaceTracker>().DropPlace.PlayerMoveNumerator());
                }
            } 
            if (cardDrag.Card.Info.IsDisplacer == false)
            {
                if (transform.GetComponentInParent<DropPlaceTracker>() != null && transform.GetComponentInParent<DropPlaceTracker>().DropPlace != null)
                {
                    if(transform.GetComponentInParent<DropPlaceTracker>().DropPlace != null)
                        transform.GetComponentInParent<DropPlaceTracker>().DropPlace.OnDrop(eventData);
                }  
            }

            if (transform.GetComponentInParent<DropSelectionDeck>() != null)
                transform.GetComponentInParent<DropSelectionDeck>().OnDrop(eventData);

            if (this.cardDrag.Card is TempCard && (this.cardDrag.Card as TempCard).DropSelectionDeck != null)
                (this.cardDrag.Card as TempCard).DropSelectionDeck.OnDrop(eventData);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;
            CardDrag cardDrag = eventData.pointerDrag.GetComponent<CardDrag>();

            if (isMenu == false)
                if (cardDrag == null || GetComponent<TempCard>() != null || cardDrag.Card.Info.IsDisplacer == false ||
                    cardDrag == transform.GetComponent<CardDrag>() || cardDrag.Card.ControlPlayer != this.cardDrag.Card.ControlPlayer ||
                    transform.GetComponentInParent<DropPlaceTracker>().DropPlace.FrontType == FrontType.Common ||
                    this.cardDrag.Card.Info.CardTypeBase != CardTypeBase.CommonType) return;
            if (isMenu == true) return;

            this.cardDrag.Card.SetStrokeColor(levelManager.MainBlueColor);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null) return;

            CardDrag cardDrag = eventData.pointerDrag.GetComponent<CardDrag>();
            if (isMenu == false)
                /*if (cardDrag == null || GetComponent<TempCard>() != null || cardDrag.Card.Info.IsDisplacer == false ||
                cardDrag == transform.GetComponent<CardDrag>() || cardDrag.Card.ControlPlayer != this.cardDrag.Card.ControlPlayer ||
                transform.GetComponentInParent<DropPlaceTracker>().DropPlace.FrontType == FrontType.Common ||
                this.cardDrag.Card.Info.CardTypeBase != CardTypeBase.CommonType) return;*/
                if (IsValidCardInteraction(cardDrag, GetComponent<TempCard>(), transform.GetComponentInParent<DropPlaceTracker>())) return; // Метод заменяет выше закомментированную проверку
            if (isMenu == true) return;

            this.cardDrag.Card.SetStrokeColor(levelManager.MainGreenColor);
        }
        private bool IsValidCardInteraction(CardDrag _cardDrag, TempCard _tempCard, DropPlaceTracker _dropPlaceTracker)
        {
            if (cardDrag == null || cardDrag == _cardDrag) return false;
            if (_tempCard != null) return false;
            if (!cardDrag.Card.Info.IsDisplacer) return false;
            if (cardDrag.Card.ControlPlayer != this.cardDrag.Card.ControlPlayer) return false;
            if (_dropPlaceTracker == null || _dropPlaceTracker.DropPlace.FrontType == FrontType.Common) return false;
            if (this.cardDrag.Card.Info.CardTypeBase != CardTypeBase.CommonType)return false;

            return true;
        }
    }
}