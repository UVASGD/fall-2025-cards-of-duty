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
         */
        public abstract IEnumerator Play();

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