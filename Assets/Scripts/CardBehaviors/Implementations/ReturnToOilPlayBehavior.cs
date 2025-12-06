using System.Collections;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class ReturnToOilPlayBehavior : PlayBehavior
    {
        [SerializeField] private List<string> possibleTokenIDs;
        
        public override IEnumerator Play()
        {
            base.Play();
            
            Tokens.TokenSacrificeEvent += OnTokenSacrifice;
            string permittedTokens = string.Join(", ", possibleTokenIDs);
            Game.Log($"Sacrifice a {permittedTokens} token...");
            yield return 0;
        }

        void OnTokenSacrifice(string tokenID)
        {
            Tokens.TokenSacrificeEvent -= OnTokenSacrifice;
            Game.Log("Click a card from your teammate's hand.");
            DragCards.CardClickEvent += OnCardClick;
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
            card.SetHasBeenPlayed(false);
            
            Game.Log($"Grabbed {targetCard.GetId()}");
            DragCards.CardClickEvent -= OnCardClick;
            
            card.playText.SetActive(false);
            card.Discard();
            player.SetActionable(true);
        }

        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            Tokens tokens = card.GetPlayer().GetTokens();
            
            if (tokens.CountTokenType(possibleTokenIDs) > 0)
            {
                return true;
            }
            Game.Log("You don't have any tokens you can sacrifice!");
            return false;
        }
    }
}