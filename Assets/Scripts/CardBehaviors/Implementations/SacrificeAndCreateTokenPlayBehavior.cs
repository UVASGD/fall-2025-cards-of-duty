using System.Collections;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class SacrificeAndCreateTokenPlayBehavior : PlayBehavior
    {
        [SerializeField] private List<string> possibleTokenIDs;
        [SerializeField] private string tokenToCreateID;
        [SerializeField] private int count = 2;
        
        public override IEnumerator Play()
        {
            base.Play();
            
            Tokens.TokenSacrificeEvent += OnTokenSacrifice;
            Game.Log("Sacrifice a (placeholder) token...");
            yield return 0;
        }

        void OnTokenSacrifice(string tokenID)
        {
            Tokens.TokenSacrificeEvent -= OnTokenSacrifice;
            StartCoroutine(AddNewTokens());
        }

        IEnumerator AddNewTokens()
        {
            Tokens.TokenSacrificeEvent -= OnTokenSacrifice;
            for (int i = 0; i < count; i++)
            {
                card.GetPlayer().GetTokens().MaterializeToken(tokenToCreateID);
                yield return new WaitForSeconds(0.5f);
            }
            card.Discard();
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