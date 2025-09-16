using System.Collections;
using UnityEngine;

namespace CardBehaviors.Implementations
{
    /**
     * A board behavior that clones any non-star card and discards the duplicate.
     */
    public class ClonePlayedCardsBoardBehavior : BoardBehavior
    {
        public override void RegisterEvents()
        {
            Card.CardPlayEvent += CloneAndDiscard;
        }
        
        public override void UnregisterEvents()
        {
            Card.CardPlayEvent -= CloneAndDiscard;
        }

        private void CloneAndDiscard(Card target)
        {
            StartCoroutine(Coroutine(target));
        }

        private IEnumerator Coroutine(Card target)
        {
            if (target == card) yield break;
            if (target.GetPlayer() != card.GetPlayer()) yield break;
            if (target.IsStarCard()) yield break;
            card.Wiggle();
            // todo Change so I don't have to set the player here
            Card clone = CardDatabase.InstantiateCard(target.GetId(), transform);
            clone.SetPlayer(card.GetPlayer());
            clone.Show();
            Vector3 position = card.transform.position + new Vector3(0.5f, 0.5f, 0);
            clone.TransformLerp(position);
            yield return new WaitForSeconds(1);
            clone.Discard();
        }
    }
}