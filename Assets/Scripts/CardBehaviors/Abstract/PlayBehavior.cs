using System.Collections;
using UnityEngine;

namespace CardBehaviors
{
    public abstract class PlayBehavior : Behavior
    {
        public abstract IEnumerator Play();

        public abstract bool CanPlay();
    }
}