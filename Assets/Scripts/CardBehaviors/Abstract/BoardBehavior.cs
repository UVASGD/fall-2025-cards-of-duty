using UnityEngine;

namespace CardBehaviors
{
    /**
     * Holds methods used to subscribe to events when a card stays on the board.
     * For example, a card that allows you to draw an extra card at the beginning of your turn
     */
    public abstract class BoardBehavior : Behavior
    {
        // Register events here
        public abstract void RegisterEvents();
        public abstract void UnregisterEvents();

        public virtual void OnSacrifice()
        {
            
        }
    }
}