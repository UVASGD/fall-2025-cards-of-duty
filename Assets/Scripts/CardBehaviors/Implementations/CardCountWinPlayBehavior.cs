using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    /**
     * Counts all cards in the player's hand, deck, and discard pile.
     * If that total exceeds requireCards, the player wins the game.
     */
    public class CardCountWinPlayBehavior : PlayBehavior
    {
        [SerializeField] private int requiredCards = 100;
        
        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            if (card.GetPlayer().HasStarCardPlayedThisTurn())
            {
                Game.Log("A star card has already been played this turn!");
                return false;
            }
            return true;
        }
        
        public override IEnumerator Play()
        {
            yield return base.Play();
            card.GetPlayer().SetActionable(false);
            card.playText.SetActive(true);
            yield return Coroutine();
            
        }

        private IEnumerator Coroutine()
        {
            Hand hand = card.GetPlayer().GetPlayerHand();
            Deck deck = card.GetPlayer().GetDeck();
            DiscardPile discardPile = card.GetPlayer().GetDiscardPile();
            int totalCards = hand.CountCards() + deck.Count() + discardPile.Count();
            
            Game.Log($"{hand.CountCards()} from hand + {deck.Count()} from deck + {discardPile.Count()} from discard pile = {totalCards} total cards");
            yield return new WaitForSeconds(3);
            if (totalCards >= requiredCards)
            {
                Game.PlayerWin(card.GetPlayer());
            }
            else
            {
                Game.Log($"Not enough cards to win (needed {requiredCards})!");
                card.Discard();
                card.GetPlayer().SetActionable(true);
                card.playText.SetActive(false);
            }
        }
    }
}