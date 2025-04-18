using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class QuizGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public GameObject endGamePanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI[] playerScores;

    public AudioSource audioSource;
    public AudioClip clipCorrect;
    public AudioClip clipWrong;
    public AudioClip clipGameOver;

    private int currentQuestionIndex = 0;
    private int correctAnswer = 0;
    private int totalQuestions = 10;

    private List<int> answeredPlayers = new List<int>();

    private string[] questions = new string[]
    {
        "Ai là người viết nên 'Bản giao hưởng số 9'?",
        "Hệ phương trình nào sau đây không có nghiệm?",
        "Thành phần chủ yếu của khí quyển Trái Đất là gì?",
        "Số nguyên tố lớn nhất nhỏ hơn 100 là số nào?",
        "Ai là người đặt nền móng cho thuyết tương đối?",
        "Quốc gia nào có diện tích lớn nhất thế giới?",
        "Kim loại nào nhẹ nhất trong bảng tuần hoàn?",
        "Trong tin học, 'byte' bằng bao nhiêu bit?",
        "Quá trình quang hợp diễn ra ở bộ phận nào của cây?",
        "Trong hình học không gian, tứ diện đều có bao nhiêu mặt tam giác đều?"
    };

    private string[,] answers = new string[,]
    {
        {"Mozart", "Beethoven", "Bach", "Chopin"},
        {"x + y = 2; x - y = 0", "2x + 3y = 5; 4x + 6y = 10", "x + y = 1; x + y = 3", "x - y = 1; y = x - 1"},
        {"Oxy", "Cacbonic", "Nitơ", "Ozon"},
        {"91", "97", "89", "93"},
        {"Isaac Newton", "Nikola Tesla", "Galileo Galilei", "Albert Einstein"},
        {"Mỹ", "Trung Quốc", "Canada", "Nga"},
        {"Nhôm", "Kali", "Liti", "Natri"},
        {"4", "8", "16", "32"},
        {"Rễ", "Thân", "Lá", "Hoa"},
        {"4", "3", "6", "8"}
    };

    private int[] correctAnswers = new int[] { 1, 2, 2, 1, 3, 3, 2, 1, 2, 0 };

    private int localScore = 0;
    private int otherScore = 0;

    private const byte ANSWER_EVENT = 1;
    private const byte ANSWER_RESULT_EVENT = 2;
    private const byte NEXT_QUESTION_EVENT = 3;
    private const byte GAME_OVER_EVENT = 4;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        endGamePanel.SetActive(false);
        ShowQuestion();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => Answer(index));
        }
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= questions.Length || currentQuestionIndex >= totalQuestions)
        {
            PhotonNetwork.RaiseEvent(GAME_OVER_EVENT, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            return;
        }

        answeredPlayers.Clear();
        questionText.text = questions[currentQuestionIndex];
        correctAnswer = correctAnswers[currentQuestionIndex];

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = answers[currentQuestionIndex, i];
            answerButtons[i].interactable = true;
        }
    }

    public void Answer(int index)
    {
        if (answeredPlayers.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
            return;

        // ❌ Chặn chọn lại trước khi gửi event
        foreach (var btn in answerButtons)
            btn.interactable = false;

        // Gửi sự kiện trả lời cho master
        object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, index };
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        PhotonNetwork.RaiseEvent(ANSWER_EVENT, content, options, SendOptions.SendReliable);
    }


    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == ANSWER_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNumber = (int)data[0];
            int answerIndex = (int)data[1];

            bool isCorrect = answerIndex == correctAnswer;
            int pointChange = isCorrect ? 1 : -1;

            // Gửi kết quả chấm điểm cho tất cả
            object[] resultData = new object[] { actorNumber, pointChange };
            RaiseEventOptions resultOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(ANSWER_RESULT_EVENT, resultData, resultOptions, SendOptions.SendReliable);

            if (isCorrect)
            {
                // Nếu đúng thì chuyển câu
                Invoke(nameof(SendNextQuestionEvent), 2f);
            }
            else
            {
                // Nếu sai thì thêm vào danh sách người đã trả lời
                answeredPlayers.Add(actorNumber);
                if (answeredPlayers.Count >= 2)
                {
                    Invoke(nameof(SendNextQuestionEvent), 2f);
                }
            }
        }
        else if (photonEvent.Code == ANSWER_RESULT_EVENT)
        {
            object[] result = (object[])photonEvent.CustomData;
            int actorNumber = (int)result[0];
            int pointChange = (int)result[1];

            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            {
                localScore += pointChange;
                audioSource.PlayOneShot(pointChange > 0 ? clipCorrect : clipWrong);
            }
            else
            {
                otherScore += pointChange;
            }

            UpdateScores();
        }
        else if (photonEvent.Code == NEXT_QUESTION_EVENT)
        {
            currentQuestionIndex++;
            ShowQuestion();
        }
        else if (photonEvent.Code == GAME_OVER_EVENT)
        {
            ShowGameOver();
        }
    }

    void SendNextQuestionEvent()
    {
        PhotonNetwork.RaiseEvent(NEXT_QUESTION_EVENT, null, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    void UpdateScores()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playerScores[0].text = $"Player 1: {localScore}";
            playerScores[1].text = $"Player 2: {otherScore}";
        }
        else
        {
            playerScores[0].text = $"Player 1: {otherScore}";
            playerScores[1].text = $"Player 2: {localScore}";
        }
    }

    void ShowGameOver()
    {
        audioSource.PlayOneShot(clipGameOver);
        endGamePanel.SetActive(true);

        string winner = "";

        if (localScore > otherScore)
        {
            winner = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? "Player 1" : "Player 2";
        }
        else if (localScore < otherScore)
        {
            winner = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? "Player 2" : "Player 1";
        }

        if (localScore == otherScore)
        {
            resultText.text = "Kết quả hòa nhau!";
        }
        else
        {
            resultText.text = $"{winner} là người chiến thắng!";
        }
    }


    public override void OnEnable() => PhotonNetwork.AddCallbackTarget(this);
    public override void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);
}
