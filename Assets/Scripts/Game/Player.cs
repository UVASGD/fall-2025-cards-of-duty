using System;
using System.Collections;
using System.Collections.Generic;
using Board;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Game game;
    [SerializeField] private int startingScore = 50;
    private int score;

    [SerializeField] private bool isHuman;

    [SerializeField] private GameObject scoreTextObject = null;
    private TMP_Text scoreText = null;

    [SerializeField] private PlayingArea playingArea = null;
    [SerializeField] private Hand hand = null;
    [SerializeField] private Deck deck = null;
    [SerializeField] private DiscardPile discardPile = null;
    [SerializeField] private EndTurnButton endTurnButton = null;

    [SerializeField] private Tokens tokens = null;
    // [SerializeField] private AffinityDisplay affinityDisplay = null;
    
    private bool turn;
    public Dictionary<string, int> affinities = new Dictionary<string, int>();
    private bool starCardPlayedThisTurn = false;
    private int minAffinityThisTurn = 0;
    private bool doubleTerritoryBonus = false;

    public delegate void TurnStartAction(Player player);
    public static event TurnStartAction TurnStartEvent;
    
    public delegate void PlayerActionableAction(Player player);
    // Called when a player is actionable again
    public static event PlayerActionableAction PlayerActionableEvent;
    
    // Inactionable means that the player cannot make any other moves right now (but their turn isn't over)
    // todo change this to "card being played" state
    private bool actionable;

    // If true, the player may discard cards even if it's not their turn, but nothing else.
    // Ensure that this is set to false at the beginning of the player's turn.
    private bool specialDiscard = true;
    
    [SerializeField] private CPUDecisionMaker cpu;

    [SerializeField] private CardDatabase cardDatabase;

    // The direction to push cards if we draw a card and another one is already there
    // [SerializeField] Vector3 shiftVector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = startingScore;
        // Sometimes the Deck's Start() method runs after this method for some reason, so set the reference now
        deck.SetPlayer();
        StartCoroutine(StartingCoroutine());
        scoreText = scoreTextObject.GetComponent<TMP_Text>();
        if (endTurnButton)
        {
            endTurnButton.gameObject.SetActive(false);
        }
    }

    // todo this coroutine should be called by the Game first
    IEnumerator StartingCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < 3; i++)
        {
            deck.DrawTopCard();
            yield return new WaitForSeconds(0.3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int delta)
    {
        if (score < 0) return;
        if (doubleTerritoryBonus)
        {
            delta *= 2;
            Game.Log("Territory gain doubled!");
        }
        score += delta;
        scoreText.text = $"Territory: {score}";
        
        if (score >= 100)
        {
            Game.PlayerWin(this);
            scoreText.color = Color.goldenRod;
        }
    }

    public void RemoveScore(int delta)
    {
        if (score < 0) return;
        score -= delta;
        scoreText.text = $"Territory: {score}";
    }

    public static Player GetPlayer(MonoBehaviour obj)
    {
        return obj.GetComponentInParent<Player>();
    }

    public PlayingArea GetPlayingArea()
    {
        return playingArea;
    }

    public Hand GetPlayerHand()
    {
        return hand;
    }

    public DiscardPile GetDiscardPile()
    {
        return discardPile;
    }

    public Deck GetDeck()
    {
        return deck;
    }

    public bool IsHuman()
    {
        return isHuman;
    }

    public void TurnStart()
    {
        turn = true;
        actionable = true;
        specialDiscard = false;
        ResetAffinities();
        
        deck.DrawTopCard();

        if (endTurnButton)
        {
            endTurnButton.gameObject.SetActive(true);
        }

        if (TurnStartEvent != null) TurnStartEvent(this);

        if (cpu)
        {
            cpu.Decide();
        }
    }
    
    public void SetMinAffinityThisTurn(int minAffinity)
    {
        minAffinityThisTurn = minAffinity;
    }
    
    public void SetTerritoryBonusDoubleThisTurn(bool doubleBonus)
    {
        doubleTerritoryBonus = doubleBonus;
    }

    public void TurnOver()
    {
        turn = false;
        starCardPlayedThisTurn = false;
        actionable = true;
        doubleTerritoryBonus = false;
        
        // hand.MergeCards();
        hand.UpdateCardLocations();

        if (minAffinityThisTurn > 0)
        {
            int totalAffinity = 0;
            foreach (var affinity in affinities)
            {
                totalAffinity += affinity.Value;
            }

            if (totalAffinity < minAffinityThisTurn)
            {
                StartCoroutine(LoseOnAffinity());
                return;
            }

            minAffinityThisTurn = 0;
        }
        
        game.NextTurn(this);
    }

    IEnumerator LoseOnAffinity()
    {
        Game.Log("You didn't gather enough affinity this turn...");
        yield return new WaitForSeconds(2);
        Game.PlayerWin(game.GetCpuPlayer1());
    }

    public bool IsTurn()
    {
        return turn;
    }
    
    public Player GetOpponent1()
    {
        if (game) return game.GetOpponent1(this);
        return null;
    }
    
    public Player GetOpponent2()
    {
        if (game) return game.GetOpponent2(this);
        return null;
    }
    
    public void SetGame(Game g)
    {
        game = g;
    }

    public bool IsActionable()
    {
        return actionable;
    }
    
    public void SetActionable(bool a)
    {
        actionable = a;
        if (actionable && PlayerActionableEvent != null) PlayerActionableEvent(this);
    }
    
    public void SetSpecialDiscard(bool canDiscard)
    {
        specialDiscard = canDiscard;
    }
    
    public bool CanSpecialDiscard()
    {
        return specialDiscard;
    }

    public void CPUSpecialDiscard(int count)
    {
        if (cpu)
        {
            cpu.ForceDiscardCard(count);
        }
    }

    public void SetStarCardPlayedThisTurn(bool played)
    {
        starCardPlayedThisTurn = played;
    }
    
    public bool HasStarCardPlayedThisTurn()
    {
        return starCardPlayedThisTurn;
    }

    public Player GetTeammate()
    {
        return game.GetTeammate(this);
    }
    
    public void AddAffinity(string affinity, int amount)
    {
        if (String.IsNullOrEmpty(affinity)) return;
        if (!affinities.TryAdd(affinity, amount))
        {
            affinities[affinity] += amount;
        }
    }

    public void ResetAffinities()
    {
        affinities.Clear();
    }

    public int GetAffinity(string affinityType)
    {
        return affinities.GetValueOrDefault(affinityType, 0);
    }
    
    public Tokens GetTokens()
    {
        return tokens;
    }

}
