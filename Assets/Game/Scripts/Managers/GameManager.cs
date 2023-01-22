using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [Header("Design")]
    public Platform platform;
    public float ballSpawnDistance = 4f;
    [SerializeField] private float gameSpeedTime = 1.7f;
    [SerializeField] private float fastGameSpeedTime = 2f;
    [SerializeField] private float distanceBetweenBalss = 2.5f;
    [SerializeField] private int maxBallCount = 8;
    [SerializeField] private int maxUnlockablePinCount = 3;
    [SerializeField] private int pinCountIncrese = 3;
    [SerializeField] private int ballCountIncrease = 2;
    [SerializeField] private UpgradeItemData ballData;
    [SerializeField] private UpgradeItemData mergeData;
    [SerializeField] private UpgradeItemData platformData;
    [SerializeField] private UpgradeItemData pinData;
    [SerializeField] private UpgradeItemData nextLevelData;

    public List<BallMovement> ActiveBalls = new List<BallMovement>();
    public CinemachineVirtualCamera virtualCamera;
    public UpgradeItemData PlatformData => platformData;

    //DEFAULT GAMEPLAY INTERVAL ==== 2.5F
    //BALL 2 REPLACED

    private bool canAddBall;
    private bool canSpeedUp;
    private bool canMerge;
    private bool takeInput;

    public event Action updateUI; 

    private static readonly string _directory = "/SaveData/";
    private static readonly string _fileName = "BallData.txt";

    Coroutine speedUpRoutine;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        ballData.Initialize();
        platformData.Initialize();
        mergeData.Initialize();
        pinData.Initialize();
        nextLevelData.Initialize();

        //early

        maxBallCount += platformData.currentLevel <= 1 ? 0 : platformData.currentLevel * ballCountIncrease;
        maxUnlockablePinCount += pinData.currentLevel <= 1 ? 0 : pinData.currentLevel * pinCountIncrese;
        //spawn ball
    }

    void Start()
    {
        BallMovement.onBallsStartToMove += OnBallStartToMove;
        platformData.OnLevelUp += OnPlatformLevelUp;
        ballData.OnLevelUp += OnBallLevelUp;
        pinData.OnLevelUp += OnPinLevelUp;
        mergeData.OnLevelUp += StartMerge;
        nextLevelData.OnLevelUp += OnNextLevel;
        InputManager.Instance.onTouchStart += ListenInput;
        virtualCamera.m_Lens.FieldOfView = 50 + maxBallCount;
        Initialize();
    }
    private void OnDisable()
    {
        platformData.OnLevelUp -= OnPlatformLevelUp;
        ballData.OnLevelUp -= OnBallLevelUp;
        pinData.OnLevelUp -= OnPinLevelUp;
        mergeData.OnLevelUp -= StartMerge;
        nextLevelData.OnLevelUp -= OnNextLevel;
        InputManager.Instance.onTouchStart -= ListenInput;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartMerge();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnBallLevelUp();
        }
    }
    public void SetTime(float timeScaleValue)
    {
        Time.timeScale = timeScaleValue;
    }
    private void Initialize()
    {
        //late
        platform.ActivateNewPlatformLevel(platformData.currentLevel);
        platform.UnlockPins(pinData.currentLevel - 1);
        MoneyFloor.Instance.ActivateTargetToShoot(nextLevelData.currentLevel - 1, 0);

        canMerge = true;
        canSpeedUp = true;
        canAddBall = true;

        updateUI?.Invoke();

        platform.StartSpawnBalls(3);
        ActivateInput(true);

        if (ActiveBalls.Count > 0)
        {
            foreach (BallMovement ball in ActiveBalls)
            {
                ball.ActivateTrail(false);
            }
        }
    }
    private void OnBallStartToMove()
    {
        foreach (BallMovement item in GameManager.Instance.ActiveBalls)
        {
            item.SetSpline(platform.GetCurrentSplineComputer(), false);
        }

        BallMovement.onBallsStartToMove -= OnBallStartToMove;
    }
    public bool CheckCanMergeBalls()
    {
        if (ActiveBalls.Count > 0)
        {
            //wait for merge end
            for (int i = 0; i < 10; i++)
            {
                List<BallMovement> currentBalls = new List<BallMovement>();

                for (int k = 0; k < ActiveBalls.Count; k++)
                {
                    if ((int)ActiveBalls[k].GetComponent<PoolElement>().value == i && ActiveBalls[k].GetComponent<MergeHandler>().canMerge)
                    {
                        currentBalls.Add(ActiveBalls[k]);
                    }
                }

                if (currentBalls.Count >= 3)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public void StartMerge()
    {
        if (ActiveBalls.Count > 0)
        {
            //wait for merge end
            Debug.Log("Merge Started");
            for (int i = 0; i < 10; i++)
            {
                List<BallMovement> currentBalls = new List<BallMovement>();

                for (int k = 0; k < ActiveBalls.Count; k++)
                {
                    if ((int)ActiveBalls[k].GetComponent<PoolElement>().value == i && ActiveBalls[k].GetComponent<MergeHandler>().canMerge)
                    {
                        Debug.Log("Ball Added");
                        currentBalls.Add(ActiveBalls[k]);
                    }
                }

                if (currentBalls.Count >= 3)
                {
                    List<BallMovement> mergeBalls = new List<BallMovement>();
                    mergeBalls.Add(currentBalls[0]);
                    mergeBalls.Add(currentBalls[1]);
                    mergeBalls.Add(currentBalls[2]);
                    for (int z = 0; z < mergeBalls.Count; z++)
                    {
                        ActiveBalls.Remove(mergeBalls[z]);
                    }
                    Debug.Log("Activate Merge");
                    BallAnimationSystem.Instance.ActivateMerge(mergeBalls.ToArray());
                    SetCanAddBall(false);
                    SetCanMerge(false);
                    updateUI?.Invoke();
                    break;
                }
            }
        }
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
    public void SetCanAddBall(bool value)
    {
        canAddBall = value;
    }
    public void SetCanMerge(bool value)
    {
        canMerge = value;
    }
    public bool GetCanAddBall()
    {
        if (ActiveBalls.Count < maxBallCount && canAddBall)
        {
            return true;
        }

        return false;
    }
    public bool GetCanAddPins()
    {
        if (pinData.currentLevel <= maxUnlockablePinCount)
        {
            return true;
        }

        return false;
    }
    public bool GetCanMergeBalls()
    {
        if (CheckCanMergeBalls() && canMerge) 
        {
            return true;
        }

        return false;
    }
    public void ActivateInput(bool value = true)
    {
        takeInput = value;
    }
    public void OnBallLevelUp()
    {
        Debug.Log("OnBallLevelUp");
        platform.SpawnNewBall(PoolItems.Ball1);
        updateUI?.Invoke();
    }

    public void OnPinLevelUp()
    {
        platform.UnlockPins(pinData.currentLevel - 1);
        updateUI?.Invoke();
    }

    public void OnPlatformLevelUp()
    {
        maxUnlockablePinCount += pinCountIncrese;
        maxBallCount += ballCountIncrease;
        virtualCamera.m_Lens.FieldOfView = 50 + maxBallCount;
        platform.ActivateNewPlatformLevel(platformData.currentLevel);
        foreach (BallMovement ball in ActiveBalls)
        {
            ball.ActivateSplineFollow(false);
            ball.SetSpline(platform.GetCurrentSplineComputer(), false);
            ball.ActivateSplineFollow(true);
        }
        OnPinLevelUp();
        updateUI?.Invoke();
    }
    private void OnNextLevel()
    {
        MoneyFloor.Instance.ActivateTargetToShoot(nextLevelData.currentLevel - 1, 0);
        updateUI?.Invoke();
    }
}
