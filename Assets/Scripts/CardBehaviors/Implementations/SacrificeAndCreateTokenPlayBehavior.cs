using System.Collections;
using System.Collections.Generic;
using Board;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class SacrificeAndCreateTokenPlayBehavior : PlayBehavior
    {
        [SerializeField] private List<string> possibleTokenIDs;
        [SerializeField] private int sacrificeCount = 1;
        [SerializeField] private string tokenToCreateID;
        [SerializeField] private int count = 2;
        
        int sacrificedSoFar = 0;

        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            Tokens tokens = card.GetPlayer().GetTokens();
            
            if (tokens.CountTokenType(possibleTokenIDs) >= sacrificeCount)
            {
                return true;
            }
            Game.Log("You don't have enough tokens you can sacrifice!");
            return false;
        }
        
        public override IEnumerator Play()
        {
            base.Play();
            
            Tokens.TokenSacrificeEvent += OnTokenSacrifice;
            string permittedTokens = string.Join(", ", possibleTokenIDs);
            Game.Log($"Sacrifice {sacrificeCount} {permittedTokens} token(s)...");
            yield return 0;
        }

        void OnTokenSacrifice(string tokenID)
        {
            if (!possibleTokenIDs.Contains(tokenID)) return;
            sacrificedSoFar++;

            if (sacrificedSoFar == sacrificeCount)
            {
                Tokens.TokenSacrificeEvent -= OnTokenSacrifice;
                StartCoroutine(AddNewTokens());
            }
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
    }
}