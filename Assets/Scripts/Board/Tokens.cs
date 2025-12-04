using System;
using UnityEngine;

namespace Board
{
    public class Tokens : MonoBehaviour
    {
        [SerializeField] private Player player;

        public delegate void TokenSacrificeAction(string cardID);
        public static event TokenSacrificeAction TokenSacrificeEvent;

        public void Start()
        {
            DragCards.CardClickEvent += OnCardClick;
        }

        public void MaterializeToken(string cardID)
        {
            Card token = CardDatabase.InstantiateCard(cardID, transform);
            token.SetPlayer(player);
            token.isToken = true;
            token.Show();
            if (token.boardBehavior)
            {
                token.boardBehavior.RegisterEvents();
            }
        }

        // public bool DestroyToken(string cardID)
        // {
        //     foreach (Transform child in transform)
        //     {
        //         Card card = child.GetComponent<Card>();
        //         if (card != null && card.GetId() == cardID)
        //         {
        //             if (card.boardBehavior) card.boardBehavior.UnregisterEvents();
        //             Destroy(card.gameObject);
        //             return true;
        //         }
        //     }
        //
        //     return false;
        // }

        public bool CountTokenType(string cardID)
        {
            foreach (Transform child in transform)
            {
                Card card = child.GetComponent<Card>();
                if (card != null && card.GetId() == cardID)
                {
                    return true;
                }
            }
            return false;
        }
        
        public int TokenCount()
        {
            return transform.childCount;
        }

        private void OnCardClick(DragCards.CardClickEventData data)
        {
            if (data.cancelled) return;
            Card targetCard = data.card;
            if (!targetCard.GetComponentInParent<Tokens>()) return;
            if (targetCard.GetPlayer() != player) return;
            if (!player.IsTurn()) return;

            data.cancelled = true;
            if (TokenSacrificeEvent != null) TokenSacrificeEvent(targetCard.GetId());

            if (targetCard.boardBehavior)
            {
                targetCard.boardBehavior.OnSacrifice();
                targetCard.boardBehavior.UnregisterEvents();
            }
        }
    }
}