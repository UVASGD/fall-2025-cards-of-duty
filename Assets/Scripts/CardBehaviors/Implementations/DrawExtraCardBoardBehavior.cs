namespace CardBehaviors.Implementations
{
    /**
     * Lets the player draw an extra card at the start of their turn.
     */
    public class DrawExtraCardBoardBehavior : BoardBehavior
    {
        public override void RegisterEvents()
        {
            Player.TurnStartEvent += TurnStartEvent;
        }
        
        public override void UnregisterEvents()
        {
            Player.TurnStartEvent -= TurnStartEvent;
        }
        
        public void TurnStartEvent(Player player)
        {
            PlayingArea area = card.GetPlayer().GetPlayingArea();
            if (player.GetPlayingArea() != area) return;
            player.GetDeck().DrawTopCard();
        }
    }
}