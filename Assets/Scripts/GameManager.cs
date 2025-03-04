using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private string[,] board = new string[3, 3]; // 2D array - 3x3 grid
    private string currentPlayer = "X";
    public Text resultText;
    public Text turnText;
    public Button[] buttons;
    public bool isAIEnabled = true;

    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnCellClick(index, false));
        }
        ResetBoard();
    }

    public void OnCellClick(int index, bool isAiMove)
    {
        if (!isAiMove && currentPlayer == "O" && isAIEnabled) return;

        int row = index / 3, col = index % 3;
        if (board[row, col] != "") return;

        board[row, col] = currentPlayer;
        buttons[index].GetComponentInChildren<Text>().text = currentPlayer;
        buttons[index].interactable = false;

        if (CheckWinner())
        {
            DisplayWinner();
            DisableButtons();
            return;
        }
        if (IsDraw())
        {
            resultText.text = "Draw!";
            return;
        }

        currentPlayer = (currentPlayer == "X") ? "O" : "X";
        turnText.text = currentPlayer + "'s Turn";

        if (currentPlayer == "O" && isAIEnabled)
        {
            Invoke(nameof(AIMove), 0.5f); //delay Ai
        }
    }

    private void AIMove()
    {
        if (CheckWinner() || IsDraw()) return;

        int bestScore = int.MinValue, bestMove = -1;

        //find best move
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3, col = i % 3;
            if (board[row, col] != "") continue;

            board[row, col] = "O"; // Try AI move
            int score = Minimax(0, false);
            board[row, col] = ""; // Undo move

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = i;
            }
        }

        Debug.Log($"AI chooses: {bestMove} with score: {bestScore}");
        if (bestMove != -1) OnCellClick(bestMove, true); //perform selection for ai
    }

    private int Minimax(int depth, bool isMaximizing) // minimax algorithm
    {
        string winner = GetWinner();
        if (winner == "O") return 10 - depth;
        if (winner == "X") return depth - 10;
        if (IsDraw()) return 0;

        int bestScore = isMaximizing ? int.MinValue : int.MaxValue;
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3, col = i % 3;
            if (board[row, col] != "") continue;

            board[row, col] = isMaximizing ? "O" : "X";
            int score = Minimax(depth + 1, !isMaximizing);
            board[row, col] = ""; // Undo move

            bestScore = isMaximizing ? Mathf.Max(score, bestScore) : Mathf.Min(score, bestScore);
        }

        return bestScore;
    }

    private string GetWinner()
    {
        //check vertical and horizontal
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] != "" && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                return board[i, 0];
            if (board[0, i] != "" && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                return board[0, i];
        }

        //check diagonal
        if (board[0, 0] != "" && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
            return board[0, 0];
        if (board[0, 2] != "" && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
            return board[0, 2];

        return "";
    }

    private bool CheckWinner() => GetWinner() != "";

    private bool IsDraw() => board.Cast<string>().All(cell => cell != "") && GetWinner() == "";

    public void ResetBoard()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                board[i, j] = "";

        foreach (Button btn in buttons)
        {
            btn.GetComponentInChildren<Text>().text = "";
            btn.interactable = true;
        }

        resultText.text = "";
        currentPlayer = "X";
        turnText.text = currentPlayer + "'s Turn";

        //trigger ai if it goes first
        if (currentPlayer == "O" && isAIEnabled)
        {
            Invoke(nameof(AIMove), 0.5f);
        }
    }

    private void DisableButtons()
    {
        foreach (Button btn in buttons)
        {
            btn.interactable = false;
        }
    }

    private void DisplayWinner()
    {
        string winner = GetWinner();

        if (winner == "X")
        {
            resultText.text = isAIEnabled ? "Player Wins!" : "Player 1 (X) Wins!";
        }
        else if (winner == "O")
        {
            resultText.text = isAIEnabled ? "AI Wins!" : "Player 2 (O) Wins!";
        }
    }
}
