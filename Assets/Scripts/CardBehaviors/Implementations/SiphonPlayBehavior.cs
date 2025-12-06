using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    public class SiphonPlayBehavior : PlayBehavior
    {
        public override IEnumerator Play()
        {
            yield return base.Play();
            card.GetPlayer().SetActionable(false);
            card.GetPlayer().drawExtraNextTurn = true;
            card.GetPlayer().GetTeammate().doNotDrawNextTurn = true;
            card.playText.SetActive(true);
            yield return new WaitForSeconds(1);
            card.Discard();
            card.GetPlayer().SetActionable(true);
        }
    }
}