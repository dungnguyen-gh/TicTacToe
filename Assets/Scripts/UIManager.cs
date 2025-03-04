using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public Button[] buttons;
    private ColyseusManager colyseusManager;

    void Awake()
    {
        // Ensure Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        colyseusManager = FindObjectOfType<ColyseusManager>();

        if (colyseusManager == null)
        {
            Debug.LogError("ColyseusManager not found in the scene.");
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => colyseusManager.SendMove(index));
        }
    }

    public void UpdateBoard(string[] board)
    {
        if (board == null || buttons == null || buttons.Length == 0)
        {
            Debug.LogError("Board or buttons array is null.");
            return;
        }

        for (int i = 0; i < board.Length && i < buttons.Length; i++)
        {
            Text btnText = buttons[i].GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.text = board[i];
            }
            else
            {
                Debug.LogError($"Button {i} is missing a Text component.");
            }
        }
    }
}
