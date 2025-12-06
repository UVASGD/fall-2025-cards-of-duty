using System;
using System.Collections.Generic;
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
            Hand.Alignment alignment = player.GetPlayerHand().alignment;
            Vector3 offset = Vector3.zero;
            if (alignment == Hand.Alignment.Left)
            {
                offset = new Vector3(1.5f * transform.childCount, 0, 0);
            }
            else
            {
                offset = new Vector3(-1.5f * transform.childCount, 0, 0);
            }
            
            
            Card token = CardDatabase.InstantiateCard(cardID, transform);
            token.transform.localPosition += offset;
            token.SetPlayer(player);
            token.isToken = true;
            token.Show();
            if (token.boardBehavior)
            {
                token.boardBehavior.RegisterEvents();
            }
            
            UpdatePositions();
        }

        public void UpdatePositions()
        {
            Hand.Alignment alignment = player.GetPlayerHand().alignment;
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var card = transform.GetChild(i).GetComponent<Card>();
                if (!card) continue;
                
                Vector3 offset = Vector3.zero;
                if (alignment == Hand.Alignment.Left)
                {
                    offset = new Vector3(1.5f * i, 0, 0);
                }
                else
                {
                    offset = new Vector3(-1.5f * i, 0, 0);
                }

                var newPosition = transform.position + offset;
                card.TransformLerp(newPosition);
                card.GetSpriteRenderer().sortingOrder = i;
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

        public int CountTokenType(List<string> cardIDs)
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                Card card = child.GetComponent<Card>();
                if (card != null && cardIDs.Contains(card.GetId()))
                {
                    count++;
                }
            }
            return count;
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