using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class OpponentDiscardPlayBehavior : PlayBehavior
    {
        [SerializeField] private int cardsToDiscard = 1;
        private int cardsDiscarded = 0;
        Player currentOpponent;

        public override IEnumerator Play()
        {
            yield return base.Play();
            card.playText.SetActive(true);
            card.GetPlayer().SetActionable(false);

            // todo allow choosing an opponent to target
            currentOpponent = card.GetPlayer().GetOpponent1();
            if (currentOpponent && currentOpponent.GetPlayerHand().CountCards() >= cardsToDiscard)
            {
                cardsDiscarded = 0;
                Card.CardDiscardEvent += DiscardCallback;
                NotifyOpponentDiscardCards(currentOpponent, cardsToDiscard);
            }
            else if (!currentOpponent)
            {
                Game.Log("No opponent in slot 1!");
                StartCoroutine(PostPlay());
            }
            else
            {
                Game.Log(currentOpponent.name + " does not have enough cards to discard!");
                StartCoroutine(PostPlay());
            }
        }
        
        // Tells the opponent to discard their own cards
        // Call ResetSpecialDiscardForOpponents() after this is done
        public void NotifyOpponentDiscardCards(Player opponent, int count)
        {
            if (!opponent) return;
            opponent.SetSpecialDiscard(true);
            Game.Log(opponent.name + " must discard " + count + " card(s).");
            opponent.CPUSpecialDiscard(count);
        }

        IEnumerator PostPlay()
        {
            yield return new WaitForSeconds(1);
            card.GetPlayer().SetActionable(true);
            card.Discard();
        }

        void DiscardCallback(Card discardedCard)
        {
            if (discardedCard.GetPlayer() != currentOpponent) return;
            cardsDiscarded++;
            if (cardsDiscarded >= cardsToDiscard)
            {
                Game.Log("Done!");
                Card.CardDiscardEvent -= DiscardCallback;
                StartCoroutine(PostPlay());
            }
        }

    }
}