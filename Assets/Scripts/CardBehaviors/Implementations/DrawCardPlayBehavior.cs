using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    /**
     * Makes the player draw a specified number of cards when played.
     */
    public class DrawCardPlayBehavior : PlayBehavior
    {
        [SerializeField] private int cardsToDraw = 1;

        public override IEnumerator Play()
        {
            Player player = card.GetPlayer();
            player.SetActionable(false);
            card.SetText($"Draw {cardsToDraw}");
            card.playText.SetActive(true);
            yield return DrawAndEnd();
        }
        
        public override bool CanPlay()
        {
            return true;
        }
        
        IEnumerator DrawAndEnd()
        {
            yield return new WaitForSeconds(0.5f);
            Player player = card.GetPlayer();
            for (int i = 0; i < cardsToDraw; i++)
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