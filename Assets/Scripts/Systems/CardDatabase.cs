using System.Collections.Generic;
using System.Linq;
using Cards;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "ScriptableObjects/CardDatabase", order = 1)]
public class CardDatabase : ScriptableObject
{
    [System.NonSerialized] private static Dictionary<string, GameObject> cardDictionary = new Dictionary<string, GameObject>();
    [System.NonSerialized] private static Dictionary<string, CardIdentifier> cardInfoDictionary = new Dictionary<string, CardIdentifier>();
    [System.NonSerialized] private static Dictionary<string, CardDeck> deckDictionary = new Dictionary<string, CardDeck>();
    public List<CardIdentifier> cardList;
    public List<DeckIdentifier> deckList;
    
    [System.Serializable]
    public class CardIdentifier
    {
        public string id;
        public GameObject card;
        public string name;
        [TextArea(2, 7)]
        public string description;
    }
    
    [System.Serializable]
    public class DeckIdentifier
    {
        public string id;
        public CardDeck deck;
    }

    public static Card InstantiateCard(string str, Transform parent)
    {
        if (!cardDictionary.ContainsKey(str))
        {
            Debug.Log("Attempted to instantiate non-existent card: " + str);
            return null;
        }
        Card card = Instantiate(cardDictionary[str], parent, true).GetComponent<Card>();
        CardIdentifier info = cardInfoDictionary[str];
        card.transform.localPosition = Vector3.zero;
        card.Hide();
        card.SetId(str);
        card.SetCardName(info.name);
        card.SetCardDescription(info.description);
        return card;
    }
    
    public static CardDeck GetDeckById(string id)
    {
        if (deckDictionary.ContainsKey(id))
        {
            return deckDictionary[id];
        }
        Debug.Log("Attempted to get non-existent deck: " + id);
        return null;
    }

    public static Card InstantiateRandomCard(Transform parent)
    {
        int randomInt = Random.Range(0, cardDictionary.Count);
        List<string> ids = Enumerable.ToList<string>(cardDictionary.Keys);
        return InstantiateCard(ids[randomInt], parent);
    }

    public void InitDictionary()
    {
        foreach (CardIdentifier cardWrapper in cardList)
        {
            cardDictionary[cardWrapper.id] = cardWrapper.card;
        }
        foreach (CardIdentifier cardWrapper in cardList)
        {
            cardInfoDictionary[cardWrapper.id] = cardWrapper;
        }
        foreach (DeckIdentifier deckWrapper in deckList)
        {
            deckDictionary[deckWrapper.id] = deckWrapper.deck;
        }
    }

    public static int TotalCardCount()
    {
        return cardDictionary.Count;
    }


    public static GameObject GetPrefab(string str)
    {
        return cardDictionary[str];
    }

}