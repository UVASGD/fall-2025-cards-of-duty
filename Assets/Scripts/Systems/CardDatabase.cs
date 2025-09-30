using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "ScriptableObjects/CardDatabase", order = 1)]
public class CardDatabase : ScriptableObject
{
    private static Dictionary<string, GameObject> cardDictionary = new Dictionary<string, GameObject>();
    public List<CardIdentifier> cardList;

    [System.Serializable]
    public class CardIdentifier
    {
        public string id;
        public GameObject card;
        [TextArea(1, 2)]
        public string shortDescription;
        [TextArea(2, 7)]
        public string description;
    }

    public static Card InstantiateCard(string str, Transform parent)
    {
        if (!cardDictionary.ContainsKey(str)) return null;
        Card card = Instantiate(cardDictionary[str], parent).GetComponent<Card>();
        card.transform.localPosition = Vector3.zero;
        card.Hide();
        card.SetId(str);
        return card;
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