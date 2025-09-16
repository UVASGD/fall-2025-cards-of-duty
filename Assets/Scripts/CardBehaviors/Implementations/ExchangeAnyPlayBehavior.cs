using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class ExchangeAnyPlayBehavior : PlayBehavior
    {
        [SerializeField] private int selfCardCount = 2;
        [SerializeField] private int teammateCardCount = 2;
        HashSet<Card> selfSelectedCards = new HashSet<Card>();
        HashSet<Card> teammateSelectedCards = new HashSet<Card>();
        
        public override bool CanPlay()
        {
            Player player = card.GetPlayer();
            Player teammate = player.GetTeammate();
            if (!teammate)
            {
                Game.Log("You have no teammate!");
                return false;
            }
            
            if (player.GetPlayerHand().CountCards() < selfCardCount)
            {
                Game.Log("You don't have enough cards in hand to exchange with!");
                return false;
            }
            
            if (teammate.GetPlayerHand().CountCards() < teammateCardCount)
            {
                Game.Log("Your teammate doesn't have enough cards to exchange with!");
                return false;
            }

            return true;
        }
        
        public override IEnumerator Play()
        {
            Player player = card.GetPlayer();
            
            player.SetActionable(false);
            card.playText.SetActive(true);
            Game.Log($"Choose {selfCardCount} cards from your hand");
            
            // Pick x cards in your hand, then pick x cards in partner's hand.
            // Make sure you can't pick the same card twice
            // Then swap the selected cards
            DragCards.CardClickEvent += OnCardClickSelf;
            yield return 0;
        }

        // todo reduce code duplication
        private void OnCardClickSelf(DragCards.CardClickEventData data)
        {
            Player player = card.GetPlayer();
            Card targetCard = data.card;
            
            if (targetCard.GetPlayer() != player)
            {
                Game.Log("This card belongs to " + targetCard.GetPlayer());
                return;
            }

            if (targetCard.HasBeenPlayed()) return;
            if (targetCard.IsHighlighted())
            {
                selfSelectedCards.Remove(targetCard);
                targetCard.SetHighlighted(false);
            }
            else
            {
                selfSelectedCards.Add(targetCard);
                targetCard.SetHighlighted(true);
            }

            if (selfSelectedCards.Count == selfCardCount)
            {
                DragCards.CardClickEvent -= OnCardClickSelf;
                Game.Log($"Now choose {selfCardCount} cards from your teammate's hand");
                DragCards.CardClickEvent += OnCardClickTeammate;
            }
        }
        
        private void OnCardClickTeammate(DragCards.CardClickEventData data)
        {
            Player player = card.GetPlayer();
            Player teammate = player.GetTeammate();
            Card targetCard = data.card;
            
            if (targetCard.GetPlayer() != teammate)
            {
                Game.Log("This card belongs to " + targetCard.GetPlayer());
                return;
            }

            if (targetCard.HasBeenPlayed()) return;
            if (targetCard.IsHighlighted())
            {
                teammateSelectedCards.Remove(targetCard);
                targetCard.SetHighlighted(false);
            }
            else
            {
                teammateSelectedCards.Add(targetCard);
                targetCard.SetHighlighted(true);
            }
            
            if (teammateSelectedCards.Count == teammateCardCount)
            {
                DragCards.CardClickEvent -= OnCardClickTeammate;
                StartCoroutine(ExchangeCards());
            }
        }
        
        private IEnumerator ExchangeCards()
        {
            Player player = card.GetPlayer();
            Player teammate = player.GetTeammate();
            
            foreach (Card selfCard in selfSelectedCards)
            {
                selfCard.SetHighlighted(false);
                selfCard.TransferToOtherPlayer(teammate);
            }

            yield return new WaitForSeconds(0.5f);
            foreach (Card teammateCard in teammateSelectedCards)
            {
                teammateCard.SetHighlighted(false);
                teammateCard.TransferToOtherPlayer(player);
            }
            
            selfSelectedCards.Clear();
            teammateSelectedCards.Clear();

            yield return new WaitForSeconds(0.5f);
            player.GetPlayerHand().UpdateCardLocations();
            teammate.GetPlayerHand().UpdateCardLocations();
            
            player.SetActionable(true);
            card.playText.SetActive(false);
            card.Discard();
        }
    }
}