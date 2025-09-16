using UnityEngine;

/**
 * A nice utility script that destroys a GameObject after its animation completes.
 */
public class AnimationSelfDestruct : MonoBehaviour {
    
    void Start()
    {
        var time = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, time);
    }
    
    
}