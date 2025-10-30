using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Decision maker for CPU players, which then uses external methods on the Player object to perform actions.
 */
public class CPUDecisionMaker : MonoBehaviour
{
    public Player player;
    
    public void Decide()
    {
        StartCoroutine(DecideCoroutine());
    }

    /**
     * If less than 2 cards, end turn.
     * Otherwise, the chance to play a card increases based on number of cards in hand.
     */
    IEnumerator DecideCoroutine()
    {
        yield return new WaitForSeconds(2);
        int cards = player.GetPlayerHand().CountCards();
        if (cards < 2)
        {
            Player.GetPlayer(this).TurnOver();
        }
        else
        {
            // Number of cards * 5 + 50% chance to play a card
            int random = Random.Range(0, 100);
            if (random < cards * 5 + 50)
            {
                int randomCard = Random.Range(0, player.GetPlayerHand().CountCards());
                player.GetPlayerHand().PlayCard(randomCard);
                // Wait for the card to complete its play behavior
                Debug.Log("[CPU] Waiting...");
                Player.PlayerActionableEvent += OnActionable;
            }
            else
            {
                Player.GetPlayer(this).TurnOver();
            }
        }
    }
    
    void OnActionable(Player p)
    {
        if (p != player) return;
        Player.PlayerActionableEvent -= OnActionable;
        Debug.Log("[CPU] Choosing next action...");
        StartCoroutine(DecideCoroutine());
    }

    // For when the CPU needs to select a card. The CPU cannot drag cards.
    void ClickCard(int index)
    {
        DragCards.HandleLeftClickCPU(player.GetPlayerHand().GetCard(index));
    }

    public void ForceDiscardCard(int count)
    {
        StartCoroutine(DiscardCards(count));
    }

    IEnumerator DiscardCards(int count)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < count; i++)
        {
            if (player.GetPlayerHand().CountCards() == 0) yield break;
            
            int randomCard = Random.Range(0, player.GetPlayerHand().CountCards());
            player.GetPlayerHand().GetCard(randomCard).Discard();
            yield return new WaitForSeconds(0.5f);
        }
    }
}