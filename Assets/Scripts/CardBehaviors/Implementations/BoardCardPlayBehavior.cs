using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    /**
     * Play behavior that adds the card to the board when played.
     */
    public class BoardCardPlayBehavior : PlayBehavior
    {
        public override IEnumerator Play()
        {
            yield return base.Play();
            card.GetPlayer().SetActionable(false);
            card.playText.SetActive(true);
            yield return playCoroutine();
        }
        
        IEnumerator playCoroutine()
        {
            Player player = card.GetPlayer();
            PlayingArea playingArea = player.GetPlayingArea();
            card.boardBehavior.RegisterEvents();
            yield return new WaitForSeconds(2);
            
            card.playText.SetActive(false);
            playingArea.AddBoardCard(card);
            player.SetActionable(true);
        }
        
        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            return true;
        }
    }
}