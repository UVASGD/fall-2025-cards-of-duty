using System.Collections;

namespace CardBehaviors.Implementations
{
    public class DefaultPlayBehavior : PlayBehavior
    {
        public override IEnumerator Play()
        {
            card.Discard();
            card.GetPlayer().SetActionable(true);
            yield return 0;
        }
        
        public override bool CanPlay()
        {
            return true;
        }
    }
}