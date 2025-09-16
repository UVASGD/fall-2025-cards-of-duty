using System.Collections;
using UnityEngine;

namespace CardBehaviors
{
    public abstract class PlayBehavior : Behavior
    {
        /**
         * This is a coroutine that plays the card.
         * It being a coroutine is important, because it allows for animations to be played.
         */
        public abstract IEnumerator Play();
        
        public abstract bool CanPlay();
    }
}