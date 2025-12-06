using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class ReorderCardsPlayBehavior : PlayBehavior
    {
        [SerializeField] private int numberOfCards = 3;
        
        // x cards come out of the deck offset from each other
        // listen to card click event, on click, they go back into the deck
        // once all cards are clicked end

        private int returnedCards = 0;
        List<Card> displayedCards = new List<Card>();

        public override IEnumerator Play()
        {
            yield return base.Play();
            Game.Log("Reorder Cards: Click cards to put them back on your deck");
            Hand.Alignment alignment = card.GetPlayer().GetPlayerHand().alignment;
            float gap = 1.5f;
            card.GetPlayer().SetActionable(false);
            Deck deck = card.GetPlayer().GetDeck();
            
            for (int i = 1; i < numberOfCards + 1; i++)
            {
                var newCard = deck.GrabTopCard();
                newCard.Show();
                displayedCards.Add(newCard);
                Vector3 offset = Vector3.zero;
                if (alignment == Hand.Alignment.Left)
                {
                    offset = new Vector3(gap * i, 0, 0);
                }
                else
                {
                    offset = new Vector3(gap * -i, 0, 0);
                }
                newCard.TransformLerp(deck.transform.position + offset);
                
                yield return new WaitForSeconds(0.5f);
            }
            
            DragCards.CardClickEvent += OnCardClick;
        }

        void OnCardClick(DragCards.CardClickEventData data)
        {
            if (!displayedCards.Contains(data.card)) return;
            data.cancelled = true;
            data.card.SetDestroyWhenLerpComplete(true);
            data.card.TransformLerp(card.GetPlayer().GetDeck().transform.position);
            returnedCards++;
            card.GetPlayer().GetDeck().PlaceCardOnTop(data.card.GetId());
            
            if (returnedCards >= numberOfCards)
            {
                Complete();
            }
        }

        void Complete()
        {
            card.Discard();
            card.GetPlayer().SetActionable(true);
            DragCards.CardClickEvent -= OnCardClick;
        }
    }
}