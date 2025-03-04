using System.Collections.Generic;
using Colyseus.Schema;

public class TicTacToeState : Schema
{
    [Type(0, "array", typeof(ArraySchema<string>))]
    public ArraySchema<string> board = new ArraySchema<string>();

    [Type(1, "string")]
    public string currentPlayer = "X";

    [Type(2, "map", typeof(MapSchema<Player>))]
    public MapSchema<Player> players = new MapSchema<Player>();

    [Type(3, "map", typeof(MapSchema<bool>))]
    public MapSchema<bool> ready = new MapSchema<bool>();

    [Type(4, "boolean")]
    public bool gameStarted = false;

    [Type(5, "string")]
    public string winner = null;

    [Type(6, "array", typeof(ArraySchema<string>))]
    public ArraySchema<string> chatMessages = new ArraySchema<string>();
}

public class Player : Schema
{
    [Type(0, "string")]
    public string id;

    [Type(1, "string")]
    public string symbol;
}
