using System.Collections;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class SacrificeForTerritoryPlayBehavior : PlayBehavior
    {
        [SerializeField] private List<string> possibleTokenIDs;
        [SerializeField] private int scoreToAdd = 5;
        
        public override IEnumerator Play()
        {
            base.Play();
            
            string permittedTokens = string.Join(", ", possibleTokenIDs);
            Tokens.TokenSacrificeEvent += OnTokenSacrifice;
            Game.Log($"Sacrifice a {permittedTokens} token...");
            yield return 0;
        }

        void OnTokenSacrifice(string tokenID)
        {
            card.GetPlayer().AddScore(scoreToAdd);
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
            Game.Log("You don't have enough tokens you can sacrifice!");
            return false;
        }
    }
}