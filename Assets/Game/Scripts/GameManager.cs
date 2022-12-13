using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [Header("Design")]
    [SerializeField] private float _gameSpeedTime = 1.0f;
    [SerializeField] private float _fastGameSpeedTime = 1.6f;
    [SerializeField] private float _spawnInterval = 2.5f;
    [SerializeField] private float _distanceBetweenBalss = 2.5f;
    [SerializeField] private float maxBallCount = 8;
    [SerializeField] private float ballCountIncrease = 2;


    private List<BallMovement> ActiveBalls = new List<BallMovement>();

    private float _spawnTimer;

    //DEFAULT GAMEPLAY INTERVAL ==== 2.5F
    //BALL 2 REPLACED

    private bool _canSpeedUp;
    private bool _canMerge;

    private static readonly string _directory = "/SaveData/";
    private static readonly string _fileName = "BallData.txt";

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        int ballCount = PlayerPrefs.GetInt(GameConst.BALL_COUNT_KEY, 0);
        int platformCount = PlayerPrefs.GetInt(GameConst.PLATFORM_COUNT_KEY, 0);

        //spawn ball
        BallPath.Instance.StartSpawnBalls(8);
    }
    public void OnPlatformLevelUp()
    {

    }
    private void OnLevelStart()
    {

    }

}
