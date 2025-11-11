using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class DronePidgeonBoardBehavior : BoardBehavior
    {
        [SerializeField] private int territoryPerTurn = 5;
        public override void RegisterEvents()
        {
            Player.TurnStartEvent += OnTurnStart;
        }

        public override void UnregisterEvents()
        {
            Player.TurnStartEvent -= OnTurnStart;
        }

        public override void OnSacrifice()
        {
            DragCards.CardClickEvent += OnCardClick;
            Game.Log("Click any card to annihilate it!");
        }

        void OnTurnStart(Player player)
        {
            if (card.GetPlayer() != player) return;
            player.AddScore(territoryPerTurn);
        }

        void OnCardClick(DragCards.CardClickEventData data)
        {
            if (data.card == card) return;
            DragCards.CardClickEvent -= OnCardClick;
            Game.Log("Annihilated " + data.card.GetId() + "!");
            data.cancelled = true;
            Destroy(data.card.gameObject);
            Destroy(card.gameObject);
        }
    }
}