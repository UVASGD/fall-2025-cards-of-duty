using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    /**
     * Grants a set amount of points.
     */
    public class PointPlayBehavior : PlayBehavior
    {
        [SerializeField] private int pointsToAdd;
        
        public override IEnumerator Play()
        {
            yield return base.Play();
            card.GetPlayer().SetActionable(false);
            yield return playCoroutine();
        }
        
        public override bool CanPlay()
        {
            if (!base.CanPlay()) return false;
            return true;
        }
        
        IEnumerator playCoroutine()
        {
            Player player = card.GetPlayer();
            card.playText.SetActive(true);
            player.AddScore(pointsToAdd);

            yield return new WaitForSeconds(2);
            
            card.Discard();
            player.SetActionable(true);
        }
    }
}