using UnityEngine;

namespace CardGame
{
    public enum PlayerNum { Player_1, Player_2 }
    public enum PlayerType { Player, AI }

    public class Player : MonoBehaviour
    {
        [SerializeField] private int maxCard;
        [SerializeField] private PlayerNum player;
        [SerializeField] private PlayerType playerType;
        public PlayerType PlayerType => playerType;
        public PlayerNum PlayerNum => player;
        public int MaxCard => maxCard;
    }
}