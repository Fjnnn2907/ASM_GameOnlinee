using Photon.Pun;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviourPun
{
    public static ScoreManager Instance;

    public int leftScore = 0;
    public int rightScore = 0;

    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    [PunRPC]
    public void AddScore(bool isLeft)
    {
        if (isLeft)
            leftScore++;
        else
            rightScore++;

        UpdateScoreUI();
    }

    public void BallScored(bool isLeft)
    {
        if (isLeft) leftScore++;
        else rightScore++;

        UpdateScoreUI();
        CheckGameEnd();
    }

    void CheckGameEnd()
    {
        int ballCount = GameObject.FindGameObjectsWithTag("Ball").Length;

        if (leftScore >= 2000 || rightScore >= 2000 || ballCount == 0)
        {
            if (leftScore > rightScore)
                Debug.Log("🏆 Left Wins!");
            else if (rightScore > leftScore)
                Debug.Log("🏆 Right Wins!");
            else
                Debug.Log("🏳️ Draw!");

            PhotonNetwork.LoadLevel("ResultScene"); // Hoặc restart
        }
    }

    void UpdateScoreUI()
    {
        leftScoreText.text = leftScore.ToString();
        rightScoreText.text = rightScore.ToString();
    }
}
