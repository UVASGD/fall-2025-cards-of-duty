using System.Collections;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class SacrificeForDrawPlayBehavior : PlayBehavior
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
            card.GetPlayer().GetDeck().DrawTopCard();
            card.Discard();
            Tokens.TokenSacrificeEvent -= OnTokenSacrifice;
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