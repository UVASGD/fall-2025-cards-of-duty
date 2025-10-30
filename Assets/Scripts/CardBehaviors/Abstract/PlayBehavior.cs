using System.Collections;
using UnityEngine;

namespace CardBehaviors
{
    public abstract class PlayBehavior : Behavior
    {
        [SerializeField] protected int affinityCost = 0;
        [SerializeField] protected string affinityType = "";

        /**
         * This is a coroutine that plays the card.
         * It being a coroutine is important, because it allows for animations to be played.
         * This also sets the player to not actionable while the card is being played.
         * Future code must set the player back to actionable when done.
         */
        public virtual IEnumerator Play()
        {
            card.GetPlayer().AddAffinity(affinityType, 1);
            card.GetPlayer().SetActionable(false);
            yield return 0;
        }

        public virtual bool CanPlay()
        {
            int affinity = card.GetPlayer().GetAffinity(affinityType);
            if (affinity < affinityCost)
            {
                Game.Log("You need " + affinityCost + " " + affinityType + " affinity to play this card.");
                return false;
            }
            return true;
        }
    }
}