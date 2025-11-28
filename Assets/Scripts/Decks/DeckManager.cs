using UnityEngine;
public class DeckManager : MonoBehaviour
{
    [SerializeField] private CardInfoPool pool;
    public CardInfoPool Pool => pool;
}
