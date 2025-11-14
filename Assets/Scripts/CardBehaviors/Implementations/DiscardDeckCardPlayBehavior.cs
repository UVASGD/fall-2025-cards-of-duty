using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class DiscardDeckCardPlayBehavior : PlayBehavior
    {
        public override IEnumerator Play()
        {
            yield return base.Play();
            Player player = card.GetPlayer();
            Card draw = player.GetDeck().DrawTopCard();
            yield return new WaitForSeconds(1);
            draw.Discard();
            yield return new WaitForSeconds(0.5f);
            card.Discard();
        }
    }
}