using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    /**
     * Prompts the player to choose cards to discard, then draws new cards.
     */
    public class DiscardAndDrawPlayBehavior : PlayBehavior
    {
        [SerializeField] private int cardsToDiscard = 1;
        [SerializeField] private int cardsToDraw = 2;
        
        private int cardsDiscarded = 0;
        
        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            Player player = card.GetPlayer();
            if (player.GetPlayerHand().CountCards() < cardsToDiscard)
            {
                Game.Log("Not enough cards in hand to play this card!");
                return false;
            }

            if (player.HasStarCardPlayedThisTurn())
            {
                Game.Log("A star card has already been played this turn!");
                return false;
            }

            return true;
        }
        
        public override IEnumerator Play()
        {
            yield return base.Play();
            Player player = card.GetPlayer();
            player.SetActionable(false);
            player.SetStarCardPlayedThisTurn(true);
            card.SetText($"Discard {cardsToDiscard}\nDraw {cardsToDraw}");
            card.playText.SetActive(true);
            Game.Log($"Click {cardsToDiscard} card(s) in your hand to discard.");
            DragCards.CardClickEvent += OnCardClick;
            yield return 0;
        }

        void OnCardClick(DragCards.CardClickEventData data)
        {
            Card targetCard = data.card;
            if (card == targetCard) return;
            Player player = targetCard.GetPlayer();
            if (player != card.GetPlayer()) return;
            targetCard.Discard();
            
            cardsDiscarded++;
            if (cardsDiscarded < cardsToDiscard)
            {
                Game.Log($"{cardsToDiscard - cardsDiscarded} more card(s) to discard");
            }
            else
            {
                DragCards.CardClickEvent -= OnCardClick;
                StartCoroutine(DrawAndEnd());
            }
        }

        IEnumerator DrawAndEnd()
        {
            yield return new WaitForSeconds(0.5f);
            Player player = card.GetPlayer();
            for (int i = 0; i < cardsToDraw; i++)
            {
                player.GetDeck().DrawTopCard();
                yield return new WaitForSeconds(0.5f);
            }
            card.Discard();
            player.SetActionable(true);
            card.playText.SetActive(false);
            cardsDiscarded = 0;
        }
    }
}