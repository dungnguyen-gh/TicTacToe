using UnityEngine;
using Colyseus;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class ColyseusManager : MonoBehaviour
{
    private ColyseusClient client;
    private ColyseusRoom<TicTacToeState> room;

    public Text statusText;
    public Button readyButton;
    public Text winnerText;
    public InputField chatInput;
    public Text chatBox;

    async void Start()
    {
        await Task.Delay(1000);
        client = new ColyseusClient("ws://localhost:2567");
    }

    public async void CreateRoom()
    {
        await JoinRoom("tic-tac-toe", true);
    }

    public async void JoinRoom()
    {
        await JoinRoom("tic-tac-toe", false);
    }

    private async Task JoinRoom(string roomName, bool isCreating)
    {
        try
        {
            room = isCreating
                ? await client.JoinOrCreate<TicTacToeState>(roomName)
                : await client.Join<TicTacToeState>(roomName);

            if (room == null || room.State == null)
            {
                Debug.LogError("Failed to join room: Room or State is null.");
                return;
            }

            Debug.Log("Successfully joined room.");
            statusText.text = "Waiting for another player...";
            SetupRoomHandlers();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to {(isCreating ? "create" : "join")} room: {ex.Message}");
            statusText.text = $"{(isCreating ? "Room creation" : "Joining")} failed.";
        }
    }

    private void SetupRoomHandlers()
    {
        if (room == null)
        {
            Debug.LogError("Room is null, cannot setup handlers.");
            return;
        }

        room.OnStateChange += (state, isFirstState) =>
        {
            if (state == null || state.board == null)
            {
                Debug.LogError("State or board is null.");
                return;
            }

            // Convert ArraySchema<string> to string[]
            string[] boardArray = new string[state.board.Count];
            for (int i = 0; i < state.board.Count; i++)
            {
                boardArray[i] = state.board[i];
            }

            UIManager.Instance?.UpdateBoard(boardArray);
        };

        room.OnMessage<List<string>>("chatUpdate", messages =>
        {
            if (chatBox != null)
            {
                chatBox.text = string.Join("\n", messages);
            }
            else
            {
                Debug.LogError("Chat box is null.");
            }
        });

        statusText.text = "Game Joined!";
    }

    public void SendMove(int index)
    {
        room?.Send("move", new { index });
    }

    public void SendReady()
    {
        if (room != null)
        {
            room.Send("ready");
            readyButton.gameObject.SetActive(false);
        }
    }

    public void SendChat()
    {
        if (!string.IsNullOrEmpty(chatInput.text) && room != null)
        {
            room.Send("chat", chatInput.text);
            chatInput.text = "";
        }
    }
}
