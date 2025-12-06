using UnityEngine;

namespace Systems
{
    public class GameInformation : MonoBehaviour
    {
        [SerializeField] string player1Deck = "Water";
        [SerializeField] string player2Deck = "Fire";
        [SerializeField] string cpuDeck = "Earth";

        private static GameInformation instance;
        
        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
        }

        public static bool IsLoaded()
        {
            return instance != null;
        }
        
        public static string GetPlayer1Deck()
        {
            return instance.player1Deck;
        }
        
        public static string GetPlayer2Deck()
        {
            return instance.player2Deck;
        }
        
        public static string GetCpuDeck()
        {
            return instance.cpuDeck;
        }
        
        public static void SetPlayer1Deck(string deckName)
        {
            instance.player1Deck = deckName;
        }
        
        public static void SetPlayer2Deck(string deckName)
        {
            instance.player2Deck = deckName;
        }
        
        public static void SetCpuDeck(string deckName)
        {
            instance.cpuDeck = deckName;
        }
    }
}