using UnityEngine;

namespace Systems
{
    public class GameInformation : MonoBehaviour
    {
        [SerializeField] string player1Deck = "Water";
        [SerializeField] string player2Deck = "Fire";
        [SerializeField] string cpuDeck = "Earth";
        
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}