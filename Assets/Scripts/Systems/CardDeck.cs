using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [CreateAssetMenu(fileName = "CardDeck", menuName = "ScriptableObjects/CardDeck", order = 2)]
    public class CardDeck : ScriptableObject
    {
        [System.Serializable]
        public class CardEntry
        {
            public string cardName;
            public int count;
        }
        
        public List<CardEntry> cards;
        public List<CardEntry> extraDeck;
    }
}