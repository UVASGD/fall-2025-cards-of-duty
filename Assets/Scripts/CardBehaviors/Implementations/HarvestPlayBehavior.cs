using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class HarvestPlayBehavior : PlayBehavior
    {
        public override IEnumerator Play()
        {
            yield return base.Play();
            Player player = card.GetPlayer();
            player.SetActionable(false);
            int cardsToDraw = card.GetPlayer().GetTokens().TokenCount();
            card.SetText($"Draw {cardsToDraw}");
            card.playText.SetActive(true);
            yield return DrawAndEnd(cardsToDraw);
        }
        
        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            if (card.GetPlayer().GetTokens().TokenCount() == 0)
            {
                Game.Log("You don't have any tokens to harvest!");
                return false;
            }
            return true;
        }
        
        IEnumerator DrawAndEnd(int count)
        {
            yield return new WaitForSeconds(0.5f);
            Player player = card.GetPlayer();
            for (int i = 0; i < count; i++)
            {
                player.GetDeck().DrawTopCard();
                yield return new WaitForSeconds(0.5f);
            }
            card.Discard();
            card.playText.SetActive(false);
            player.SetActionable(true);
        }
    }
}