using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class ReturnFromDiscardPlayBehavior : PlayBehavior
    {
        [SerializeField] private int cardsToReturn = 3;
        
        public override IEnumerator Play()
        {
            yield return base.Play();
            card.GetPlayer().SetActionable(false);
            DiscardPile discard = card.GetPlayer().GetDiscardPile();
            for (int i = 0; i < cardsToReturn; i++)
            {
                if (discard.Count() == 0) break;
                string cardID = discard.PopRandomCardId();
                Card returnedCard = CardDatabase.InstantiateCard(cardID, discard.transform);
                returnedCard.SetPlayer(card.GetPlayer());
                card.GetPlayer().GetPlayerHand().AddCard(returnedCard);
                yield return new WaitForSeconds(0.5f);
            }
            
            card.Discard();
            card.GetPlayer().SetActionable(true);
        }
    }
}