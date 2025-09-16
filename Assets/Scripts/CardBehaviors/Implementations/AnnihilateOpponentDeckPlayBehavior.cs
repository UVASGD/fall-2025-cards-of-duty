using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class AnnihilateOpponentDeckPlayBehavior : PlayBehavior
    {
        public override bool CanPlay()
        {
            if (card.GetPlayer().HasStarCardPlayedThisTurn())
            {
                Game.Log("A star card has already been played this turn!");
                return false;
            }
            return true;
        }

        public override IEnumerator Play()
        {
            Player player = card.GetPlayer();
            Player opponent = player.GetOpponent();
            card.playText.SetActive(true);
            if (opponent)
            {
                player.SetActionable(false);
                player.SetStarCardPlayedThisTurn(true);
                yield return Coroutine(opponent.GetDeck());
            }
        }

        private IEnumerator Coroutine(Deck opponentDeck)
        {
            Deck myDeck = card.GetPlayer().GetDeck();
            yield return new WaitForSeconds(1f);
            int cardsToDraw = opponentDeck.AnnihilateHalf();
            yield return new WaitForSeconds(0.5f);
            
            Game.Log("Drawing " + cardsToDraw + " cards!");
            
            for (int i = 0; i < cardsToDraw; i++)
            {
                myDeck.DrawTopCard();
                yield return new WaitForSeconds(0.3f);
            }
            
            card.Discard();
            card.GetPlayer().SetActionable(true);
            card.playText.SetActive(false);
        }
    }
}