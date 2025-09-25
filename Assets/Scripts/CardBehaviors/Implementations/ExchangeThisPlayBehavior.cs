using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    /**
     * Exchanges this card ONLY with a selected card in your teammate's hand.
     */
    public class ExchangeThisPlayBehavior : PlayBehavior
    {
        
        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            Player player = card.GetPlayer();
            Player teammate = player.GetTeammate();
            if (!teammate)
            {
                Game.Log("You have no teammate!");
                return false;
            }
            
            if (teammate.GetPlayerHand().CountCards() == 0)
            {
                Game.Log("Your teammate has no cards in hand to exchange with!");
                return false;
            }

            return true;
        }
        public override IEnumerator Play()
        {
            yield return base.Play();
            Player player = card.GetPlayer();
            
            player.SetActionable(false);
            card.playText.SetActive(true);
            Game.Log("Click a card in your teammate's hand to exchange with this card.");
            DragCards.CardClickEvent += OnCardClick;
            yield return 0;
        }
        
        void OnCardClick(DragCards.CardClickEventData data)
        {
            Player player = card.GetPlayer();
            Player teammate = player.GetTeammate();
            Card targetCard = data.card;

            if (targetCard.GetPlayer() != teammate)
            {
                Game.Log("This card belongs to " + targetCard.GetPlayer());
                return;
            }

            if (targetCard.HasBeenPlayed())
            {
                return;
            }

            data.cancelled = true;
            
            targetCard.TransferToOtherPlayer(player);
            card.TransferToOtherPlayer(teammate);
            targetCard.SetUnplayableThisTurn();
            card.SetHasBeenPlayed(false);
            
            Game.Log($"Exchanged {card.GetId()} with {targetCard.GetId()}");
            DragCards.CardClickEvent -= OnCardClick;
            
            card.playText.SetActive(false);
            player.SetActionable(true);
        }
    }
}