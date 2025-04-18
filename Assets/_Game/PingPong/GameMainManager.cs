//using Photon.Pun;
//using Photon.Realtime;
//using UnityEngine;

//public class GameManager : MonoBehaviourPunCallbacks
//{
//    public static GameManager Instance;

//    public Transform leftSpawnPoint;
//    public Transform rightSpawnPoint;
//    public GameObject paddlePrefab;

//    private void Awake()
//    {
//        if (Instance == null)
//            Instance = this;
//    }

//    private void Start()
//    {
//        if (PhotonNetwork.IsConnectedAndReady)
//            SpawnPlayerPaddle();
//    }

//    private void SpawnPlayerPaddle()
//    {
//        if (!photonView.IsMine) return;

//        Transform spawnPoint = PhotonNetwork.IsMasterClient ? leftSpawnPoint : rightSpawnPoint;
//        GameObject paddle = PhotonNetwork.Instantiate(paddlePrefab.name, spawnPoint.position, Quaternion.identity);
//        paddle.name = PhotonNetwork.NickName + "_Paddle";
//        paddle.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
//    }

//    public override void OnPlayerLeftRoom(Player otherPlayer)
//    {
//        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 1)
//        {
//            Debug.Log("🏆 Người kia thoát, bạn thắng!");
//            PhotonNetwork.LoadLevel("ResultScene");
//        }
//    }
//}