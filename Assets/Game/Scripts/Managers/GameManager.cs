using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System;
using Cinemachine;
using System.IO;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [Header("Design")]
    public Platform platform;
    public float ballSpawnDistance = 4f;
    [SerializeField] private float gameSpeedTime = 1.7f;
    [SerializeField] private float FastTime = 1.6f;
    [SerializeField] private int maxBallCount = 8;
    [SerializeField] private int maxUnlockablePinCount = 3;
    [SerializeField] private int pinCountIncrese = 3;
    [SerializeField] private int ballCountIncrease = 2;
    [SerializeField] private UpgradeItemData ballData;
    [SerializeField] private UpgradeItemData mergeData;
    [SerializeField] private UpgradeItemData platformData;
    [SerializeField] private UpgradeItemData pinData;
    [SerializeField] private UpgradeItemData nextLevelData;

    public List<BallMovement> ActiveBalls;
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

    private BallData data = new BallData();

    private List<PoolItems> saveDataForBalls = new List<PoolItems>();

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

        maxBallCount += platformData.currentLevel <= 1 ? 1 : platformData.currentLevel * ballCountIncrease;

        if (platformData.currentLevel == 2)
        {
            maxUnlockablePinCount = 6;
        }
        else
        {
            maxUnlockablePinCount = pinData.currentLevel <= 1 ? 5 : (platformData.currentLevel * pinCountIncrese);
        }
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

        if (LoadData() == null)
        {
            platform.StartSpawnBalls(null, 3);
        }
        else
        {
            saveDataForBalls = LoadData();
            data.BallIds = saveDataForBalls;
            platform.StartSpawnBalls(saveDataForBalls, saveDataForBalls.Count);
        }

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
                        RemoveBallFromData(mergeBalls[z].GetComponent<PoolElement>().value);
                        ActiveBalls.Remove(mergeBalls[z]);
                    }
                    Debug.Log("Activate Merge");
                    BallAnimationSystem.Instance.ActivateMerge(mergeBalls.ToArray());
                    GameManager.Instance.AddBallToData(mergeBalls[0].GetComponent<MergeHandler>().evolveToThis);
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
        SetTime(FastTime);
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
        int pinCount = platform.activePins.Count + 2;

        if (pinCount < maxUnlockablePinCount)
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
        if (platformData.currentLevel == 2)
        {
            maxUnlockablePinCount = 6;
        }
        else
        {
            maxUnlockablePinCount += pinCountIncrese;
        }

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
    public void AddBallToData(PoolItems ballID)
    {
        data.AddItem(ballID);

        DataSaving();
    }
    public void RemoveBallFromData(PoolItems ballID)
    {
        data.RemoveItem(ballID);

        DataSaving();
    }
    public void DataSaving()
    {
        string dir = Application.persistentDataPath + _directory;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string json = JsonUtility.ToJson(data);
        Debug.Log($"If you want to delete save data go to {dir} and delete BallData");
        File.WriteAllText(dir + _fileName, json);
    }
    public List<PoolItems> LoadData()
    {
        string fullPath = Application.persistentDataPath + _directory + _fileName;

        if (File.Exists(fullPath))
        {
            string json = File.ReadAllText(fullPath);
            BallData balls = JsonUtility.FromJson<BallData>(json);
            return balls.BallIds;
        }
        else
        {
            return null;
        }
    }
}
