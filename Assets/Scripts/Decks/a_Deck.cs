using UnityEngine;

namespace CardGame
{
    [RequireComponent(typeof(SoundHook))]
    public abstract class a_Deck : MonoBehaviour, ISpawnObjectsThrow, IDependency<LevelStateTracker>
    {
        [SerializeField] protected Card[] cards;
        [SerializeField] protected Transform container;
        [SerializeField] protected Transform handPlace;
        protected PlayerNum controlPlayer;
        protected SoundHook soundHook;
        protected LevelStateTracker levelStateTracker;

        public void Construct(LevelStateTracker obj) => levelStateTracker = obj;
        protected void Awake()
        {
            soundHook = GetComponent<SoundHook>();
            controlPlayer = handPlace.GetComponent<DropPlace>().ControlPlayer;
        }
        public void TakeGameObjects(GameObject[] gameObjects) //Take cards by SpawnObjectsbyPropertiesList
        {
            if (gameObjects == null) return;

            cards = new Card[gameObjects.Length];

            for (int i = 0; i < gameObjects.Length; i++)
            {
                cards[i] = gameObjects[i].GetComponent<Card>();
            }
        }
        [ContextMenu(nameof(ShuffleCards))]
        public void ShuffleCards(int shuffleCount)
        {
            shuffleCount = Mathf.Clamp(shuffleCount, 1, 10);
            for (int c = 0; c < shuffleCount; c++)
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    int randomIndex = Random.Range(0, cards.Length - 1);

                    Card[] tempCards = new Card[cards.Length];
                    tempCards[i] = cards[i];
                    tempCards[randomIndex] = cards[randomIndex];

                    cards[i] = tempCards[randomIndex];
                    cards[randomIndex] = tempCards[i];
                }
                for (int i = 0; i < cards.Length; i++)
                {
                    cards[i].transform.SetSiblingIndex(i);
                }
            }
        }
    }
}