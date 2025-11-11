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
            
            Tokens.TokenSacrificeEvent += OnTokenSacrifice;
            Game.Log("Sacrifice a (placeholder) token...");
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
            
            foreach (string tokenID in possibleTokenIDs)
            {
                if (tokens.CountTokenType(tokenID))
                {
                    return true;
                }
            }
            Game.Log("You don't have any tokens you can sacrifice!");
            return false;
        }
    }
}