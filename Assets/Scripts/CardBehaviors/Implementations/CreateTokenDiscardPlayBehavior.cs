using System.Collections;
using Board;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class CreateTokenDiscardPlayBehavior : PlayBehavior
    {
        [SerializeField] private string tokenCardID;
        [SerializeField] private int count;

        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            Player player = card.GetPlayer();
            if (player.GetPlayerHand().CountCards() < 1)
            {
                Game.Log("Not enough cards in hand to play this card!");
                return false;
            }
            return true;
        }

        public override IEnumerator Play()
        {
            yield return base.Play();
            Game.Log("Click 1 card in your hand to discard.");
            DragCards.CardClickEvent += OnCardClick;
        }
        
        void OnCardClick(DragCards.CardClickEventData data)
        {
            Card targetCard = data.card;
            if (card == targetCard) return;
            Player player = targetCard.GetPlayer();
            if (player != card.GetPlayer()) return;
            targetCard.Discard();
            DragCards.CardClickEvent -= OnCardClick;
            StartCoroutine(CreateTokens());
        }

        IEnumerator CreateTokens()
        {
            Player player = card.GetPlayer();
            Tokens tokens = player.GetTokens();
            
            for (int i = 0; i < count; i++)
            {
                tokens.MaterializeToken(tokenCardID);
                yield return new WaitForSeconds(0.3f);
            }
            card.Discard();
        }
    }
}