using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Player player1 = null;
    [SerializeField] private Player player2 = null;
    [SerializeField] private Player opponent1 = null;
    [SerializeField] private Player opponent2 = null;

    [SerializeField] private CardDatabase cardDatabase = null;
    [SerializeField] private TextMeshProUGUI messageText = null;
    /**
     * Text for displaying messages to the player.
     * This is static so it can be accessed from anywhere.
     */
    private static TextMeshProUGUI staticMessageText = null;

    /**
     * Defines whose turn it is.
     */
    private int activePlayerIndex = 0;
    private List<Player> turnOrder = new List<Player>();

    void Awake()
    {
        // todo If we add other menus besides the actual game, we should not set the framerate here.
        Application.targetFrameRate = 60;
        staticMessageText = messageText;
        if (staticMessageText) staticMessageText.text = "";
    }
    
    void Start()
    {
        if (cardDatabase == null)
        {
            throw new SystemException("CardDatabase not initialized");
        }
        cardDatabase.InitDictionary();
        if (player1) player1.SetGame(this);
        if (player2) player2.SetGame(this);
        if (opponent1) opponent1.SetGame(this);
        if (opponent2) opponent2.SetGame(this);
        
        if (!player1 && !player2) 
        {
            throw new SystemException("There are no players!");
        }

        if (!opponent1 && !opponent2)
        {
            throw new SystemException("There are no opponents!");
        }
        
        // Build turn order
        if (player1) turnOrder.Add(player1);
        if (player2) turnOrder.Add(player2);
        if (opponent1) turnOrder.Add(opponent1);
        if (opponent2) turnOrder.Add(opponent2);
        
        turnOrder[0].TurnStart();
    }

    /**
     * Called by players to end their turn.
     */
    public void NextTurn(Player whoEndedTurn)
    {
        if (!whoEndedTurn) return;
        // Player who ended the turn must be the active player
        if (whoEndedTurn != turnOrder[activePlayerIndex]) return;
        
        activePlayerIndex++;
        if (activePlayerIndex >= turnOrder.Count) activePlayerIndex = 0;
        turnOrder[activePlayerIndex].TurnStart();
        Game.Log(turnOrder[activePlayerIndex].name + "'s turn");
    }

    public Player GetPlayer1()
    {
        return player1;
    }
    
    public Player GetPlayer2()
    {
        return player2;
    }
    
    public Player GetCpuPlayer1()
    {
        return opponent1;
    }
    
    public Player GetCpuPlayer2()
    {
        return opponent2;
    }

    public Player GetOpponent1(Player player)
    {
        if (player == player1 || player == player2) return opponent1;
        if (player == opponent1 || player == opponent2) return player1;
        return null;
    }
    
    public Player GetOpponent2(Player player)
    {
        if (player == player1 || player == player2) return opponent2;
        if (player == opponent1 || player == opponent2) return player2;
        return null;
    }
    
    public Player GetActivePlayer()
    {
        return turnOrder[activePlayerIndex];
    }

    public Player GetTeammate(Player player)
    {
        if (player == player1) return player2;
        if (player == player2) return player1;
        if (player == opponent1) return opponent2;
        if (player == opponent2) return opponent1;
        return null;
    }

    public static void Log(string text)
    {
        if (!staticMessageText) return;
        staticMessageText.text = text;
    }

    /**
     * Win message. A bit underwhelming right now.
     */
    public static void PlayerWin()
    {
        if (!staticMessageText) return;
        staticMessageText.text = "YOU WIN!";
        staticMessageText.fontSize = 72;
        staticMessageText.overrideColorTags = true;
        staticMessageText.color = Color.goldenRod;
    }
}