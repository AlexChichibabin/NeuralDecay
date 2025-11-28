using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CardGame
{
    [RequireComponent(typeof(Image))]
    public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
    {
        [SerializeField] private bool m_IsDragging = true;
        private Canvas canvas;
        private Vector3 dragOffset;
        private RectTransform m_DraggingPlane;
        private GameObject tempCard;
        private Vector3 defaultTempCardPosition;
        private Card card;
        private Shadow cardShadow;
        private SoundHook soundHook;
        private LevelManager levelManager;
        private DropSelectionDeck dropSelectionDeck;
        private int displacingCardIndex;
        private bool DragOnSurfaces = true;

        public Transform CurrentParent, DefaultTempCardParent;
        public Transform DefaultParent;
        public GameObject TempCard => tempCard;
        public Card Card => card;
        public DropSelectionDeck DropSelectionDeck => dropSelectionDeck;
        public int DisplacingCardIndex => displacingCardIndex;

        public void OnPointerEnter(PointerEventData eventData) => PlaySound(Sound.OnButtonSelect);
        private void Awake()
        {
            canvas = FindInParents<Canvas>(gameObject);
            tempCard = canvas.GetComponentInChildren<CardDrag>().gameObject; //Зависит от положения дочерних объетов. Должен быть первой картой от канваса
            defaultTempCardPosition = tempCard.transform.localPosition;
            card = GetComponent<Card>();
            cardShadow = GetComponent<Shadow>();
            soundHook = GetComponent<SoundHook>();
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (canvas == null) return;

            if (DragOnSurfaces)
                m_DraggingPlane = transform as RectTransform;
            else
                m_DraggingPlane = canvas.transform as RectTransform;

            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos))
                dragOffset = transform.position - globalMousePos;
            
            if(Card.Info.IsDisplacer == true) displacingCardIndex = transform.GetSiblingIndex(); // Only for displacer

            if (transform.parent.GetComponent<DropSelectionDeck>() != null)
                CurrentParent = transform.parent.GetComponent<DropSelectionDeck>().DraggablePlane;
            else
                CurrentParent = transform.parent;

            DefaultParent = transform.parent;
            DefaultTempCardParent = transform.parent;

            tempCard.transform.SetParent(DefaultTempCardParent);
            tempCard.transform.SetSiblingIndex(transform.GetSiblingIndex());
            transform.SetParent(CurrentParent.parent);

            int index = transform.GetSiblingIndex();
            GetComponent<CanvasGroup>().blocksRaycasts = false;

            SetDraggedPosition(eventData);
            tempCard.gameObject.SetActive(true);

            PlaySound(Sound.OnCardDragBegin);
            cardShadow.enabled = false;
        }
        public void OnDrag(PointerEventData data)
        {
            if (gameObject != null) SetDraggedPosition(data);

            if (tempCard.transform.parent != DefaultTempCardParent)
            {
                tempCard.transform.SetParent(DefaultTempCardParent);
                tempCard.transform.localScale = Vector3.one;
            }

            if (DefaultTempCardParent.GetComponent<DropSelectionDeck>() != null)
                CheckPositionXY(data); 
            else if(DefaultTempCardParent.GetComponent<DropPlace>() != null)
                CheckPositionOnlyX(data);
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            if (CurrentParent.GetComponent<DropPlace>() != null || CurrentParent.GetComponent<DropSelectionDeck>() != null)
                DefaultParent = CurrentParent;
            else
                CurrentParent = DefaultParent;

            transform.SetParent(CurrentParent);

            GetComponent<CanvasGroup>().blocksRaycasts = true;
            if (levelManager != null)
            {
                if (card.Info.IsDisplacer == false) transform.SetSiblingIndex(tempCard.transform.GetSiblingIndex());
                else if (levelManager.SceneName != levelManager.MainMenuSceneName)
                    if (card.AbilityIsActive == false || tempCard.GetComponentInParent<DropPlace>().FrontType == FrontType.Common)
                        transform.SetSiblingIndex(tempCard.transform.GetSiblingIndex());
            }
            else Debug.Log(levelManager + " is null");
            transform.localScale = Vector3.one; // Опционально

            DropSelectionDeck tCardDropParent;
            tempCard.transform.parent.TryGetComponent(out tCardDropParent);
            if (tCardDropParent != null && tCardDropParent.IsPool == true) tCardDropParent.UpdateDeck();
            tempCard.transform.SetParent(canvas.transform);
            tempCard.transform.localPosition = defaultTempCardPosition;
            tempCard.gameObject.SetActive(false);

            cardShadow.enabled = true;
        }
        private void SetDraggedPosition(PointerEventData data)
        {
            if (DragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
                m_DraggingPlane = data.pointerEnter.transform as RectTransform;

            var rt = gameObject.GetComponent<RectTransform>();
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
            {
                rt.position = globalMousePos + dragOffset;
                rt.rotation = m_DraggingPlane.rotation;
            }
        }
        private void CheckPositionOnlyX(PointerEventData data)
        {
            int newIndex = DefaultTempCardParent.childCount;

            for (int i = 0; i < DefaultTempCardParent.childCount; i++)
            {
                if (transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
                {
                    newIndex = i;
                    if (tempCard.transform.GetSiblingIndex() < newIndex)
                    {
                        newIndex--;
                    }
                    break;
                }
            }
            if (card.Info.IsDisplacer == false)
                tempCard.transform.SetSiblingIndex(newIndex);
        }
        private void CheckPositionXY(PointerEventData data)
        {
            int newIndex = DefaultTempCardParent.childCount;
            GridLayoutGroup grid = DefaultTempCardParent.GetComponent<GridLayoutGroup>();
            Vector3 gridOffset3 = new Vector3((grid.cellSize.x) / 2, (grid.cellSize.y) / 2);

            for (int i = 0; i < DefaultTempCardParent.childCount; i++)
            {
                RectTransform rect = transform as RectTransform;
                RectTransform tempParent = DefaultTempCardParent as RectTransform;
                Vector3 posNew = new Vector3(DefaultTempCardParent.GetChild(i).localPosition.x +
                    gridOffset3.x, DefaultTempCardParent.GetChild(i).localPosition.y + gridOffset3.y);

                if (rect.position.y > tempParent.GetChild(i).position.y - 12.5f &&
                    rect.position.x < (tempParent.GetChild(i).position.x - 3.6f))
                {
                    newIndex = i;

                    if (tempCard.transform.GetSiblingIndex() < newIndex)
                        newIndex--;
                    break;
                }
            }
            tempCard.transform.SetSiblingIndex(newIndex);
        }
        public void SetInteractivity(bool isActive)
        {
            m_IsDragging = isActive;
            //gameObject.SetActive(isActive);
            enabled = isActive;
        }
        public void ThrowProperties(Canvas can, GameObject temp, Vector3 tempPos, LevelManager levelManager) // 
        {
            canvas = can;
            tempCard = temp;
            defaultTempCardPosition = tempPos;
            if (card == null) TryGetComponent(out card);
            card.Construct(levelManager);
            //GetComponent<CardDisplace>().SetLevelManager(levelManager);
            this.levelManager = levelManager;
        }
        static public T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null) return null;
            var comp = go.GetComponent<T>();

            if (comp != null)
                return comp;

            Transform t = go.transform.parent;
            while (t != null && comp == null)
            {
                comp = t.gameObject.GetComponent<T>();
                t = t.parent;
            }
            return comp;
        }
        private void PlaySound(Sound sound)
        {
            if (tempCard == gameObject) return;
            soundHook.m_Sound = sound;
            soundHook.Play();
        }
    }
}