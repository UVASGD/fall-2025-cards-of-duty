using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class DoubleTerritoryBonusPlayBehavior : PlayBehavior
    {
        public override IEnumerator Play()
        {
            yield return base.Play();
            card.GetPlayer().SetActionable(false);
            card.playText.SetActive(true);
            card.GetPlayer().SetTerritoryBonusDoubleThisTurn(true);
            yield return new WaitForSeconds(1);
            card.Discard();
        }
    }
}