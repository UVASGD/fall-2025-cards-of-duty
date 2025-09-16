using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class YoinkPlayBehavior : PlayBehavior
    {
        [SerializeField] private int pointsToSteal = 10;
        
        public override IEnumerator Play()
        {
            card.GetPlayer().SetActionable(false);
            yield return playCoroutine();
        }
        
        public override bool CanPlay()
        {
            return true;
        }
        
        IEnumerator playCoroutine()
        {
            Player player = card.GetPlayer();
            card.playText.SetActive(true);
            // todo check if score is actually above 10
            // todo allow choosing opponent if there are multiple
            player.GetOpponent().RemoveScore(10);
            player.AddScore(pointsToSteal);

            yield return new WaitForSeconds(2);
            
            card.Discard();
            player.SetActionable(true);
        }
    }
}