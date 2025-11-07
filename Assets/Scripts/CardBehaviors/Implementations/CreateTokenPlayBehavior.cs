using System.Collections;
using Board;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class CreateTokenPlayBehavior : PlayBehavior
    {
        [SerializeField] private string tokenCardID;
        [SerializeField] private int count;
        [SerializeField] private bool drawCard;

        public override IEnumerator Play()
        {
            yield return base.Play();
            Player player = card.GetPlayer();
            Tokens tokens = player.GetTokens();
            
            for (int i = 0; i < count; i++)
            {
                tokens.MaterializeToken(tokenCardID);
                yield return new WaitForSeconds(0.3f);
            }

            // Seems out of place, but I didn't make it possible to "chain" play behaviors, so here we are
            if (drawCard)
            {
                player.GetDeck().DrawTopCard();
                yield return new WaitForSeconds(0.3f);
            }
            
            card.Discard();
        }
    }
}