using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    /**
     * Adds territory at the beginning of player turn
     */
    public class GainTerritoryBoardBehavior : BoardBehavior
    {
        [SerializeField] private int territoryToGain = 5;
        public override void RegisterEvents()
        {
            Player.TurnStartEvent += Callback;
        }
        
        public override void UnregisterEvents()
        {
            Player.TurnStartEvent += Callback;
        }

        private void Callback(Player player)
        {
            if (player != card.GetPlayer()) return;
            card.GetPlayer().AddScore(territoryToGain);
        }
    }
}