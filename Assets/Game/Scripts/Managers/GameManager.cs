using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [Header("Design")]
    public Platform platform;
    [SerializeField] private float gameSpeedTime = 1.7f;
    [SerializeField] private float fastGameSpeedTime = 2f;
    [SerializeField] private float distanceBetweenBalss = 2.5f;
    [SerializeField] private float maxBallCount = 8;
    [SerializeField] private float ballCountIncrease = 2;
    [SerializeField] private UpgradeItemData ballData;
    [SerializeField] private UpgradeItemData platformData;
    [SerializeField] private UpgradeItemData incomeData;

    public List<BallMovement> ActiveBalls = new List<BallMovement>();

    private float _spawnTimer;

    //DEFAULT GAMEPLAY INTERVAL ==== 2.5F
    //BALL 2 REPLACED

    private bool canSpeedUp;
    private bool canMerge;
    private bool takeInput;

    private static readonly string _directory = "/SaveData/";
    private static readonly string _fileName = "BallData.txt";

    Coroutine speedUpRoutine;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    private void Update()
    {
        print(Time.timeScale);
    }
    void Start()
    {
        platformData?.Initialize();
        platformData.OnLevelUp += OnPlatformLevelUp;
        InputManager.Instance.onTouchStart += ListenInput;
        Initialize();
    }
    private void OnDisable()
    {
        platformData.OnLevelUp -= OnPlatformLevelUp;
        InputManager.Instance.onTouchStart += ListenInput;
    }

    public void SetTime(float timeScaleValue)
    {
        Time.timeScale = timeScaleValue;
    }
    private void Initialize()
    {
        //early
        int ballCount = PlayerPrefs.GetInt(GameConst.BALL_COUNT_KEY, 0);
        int platformCount = PlayerPrefs.GetInt(GameConst.PLATFORM_COUNT_KEY, 0);

        maxBallCount += platformData.currentLevel * ballCountIncrease;

        //spawn ball
        platform.StartSpawnBalls(8);
        //late
        ActivateInput(true);

        if (ActiveBalls.Count > 0)
        {
            foreach (BallMovement ball in ActiveBalls)
            {
                ball.ActivateTrail(false);
            }
        }

        canMerge = true;
        canSpeedUp = true;
    }

    private void ListenInput()
    {
        if (!takeInput)
        {
            return;
        }
        if (!canSpeedUp && speedUpRoutine != null)
        {
            StopCoroutine(speedUpRoutine);
            canSpeedUp = true;
        }
        if (canSpeedUp)
        {
            speedUpRoutine = StartCoroutine(SpeedChangeProcess());
            //vibrate
        }

    }
    private IEnumerator SpeedChangeProcess()
    {
        //fast
        SpeedUp();
        yield return new WaitForSeconds(gameSpeedTime);
        SlowDown();
    }
    private void SlowDown()
    {
        if (ActiveBalls.Count > 0)
        {
            foreach (BallMovement ball in ActiveBalls)
            {
                ball.ActivateTrail(false);
            }
        }
        SetTime(1f);
        canSpeedUp = true;
    }
    private void SpeedUp()
    {
        canSpeedUp = false;
        SetTime(1.6f);
        if (ActiveBalls.Count > 0)
        {
            foreach (BallMovement ball in ActiveBalls)
            {
                ball.ActivateTrail(true);
            }
        }
    }
    public void ActivateInput(bool value = true)
    {
        takeInput = value;
    }
    public void OnPlatformLevelUp()
    {
        maxBallCount += ballCountIncrease;
    }
    private void OnLevelStart()
    {

    }

}
