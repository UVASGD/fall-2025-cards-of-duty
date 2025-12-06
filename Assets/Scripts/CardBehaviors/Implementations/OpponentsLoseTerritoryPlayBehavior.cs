using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class OpponentsLoseTerritoryPlayBehavior : PlayBehavior
    {
        
        [SerializeField] private int pointsToRemove = 2;
        public override IEnumerator Play()
        {
            yield return base.Play();
            card.GetPlayer().SetActionable(false);
            yield return playCoroutine();
        }
        
        IEnumerator playCoroutine()
        {
            card.playText.SetActive(true);
            card.GetPlayer().GetOpponent1().RemoveScore(pointsToRemove);

            yield return new WaitForSeconds(1);
            
            card.Discard();
            card.GetPlayer().SetActionable(true);
        }
    }
}