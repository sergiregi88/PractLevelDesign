using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public Player Player { get; private set; }
    public CameraController ControllerCamera { get; private set; }
    public TimeSpan RunningTime { get { return DateTime.UtcNow - _started; } }
    public int currectTimeBonus
    {
        get
        {
            var secondDiference = (int)(BonusCutoffSeconds - RunningTime.TotalSeconds);
            return Mathf.Max(0, secondDiference) * BonusSecondMultiplier;
        }
    }

    private List<Checkpoint> _checkPoints;
    private int _currentCheckpointIndex;
    private DateTime _started;
    private int _savedPoints;



    public Checkpoint DebugSpawn;
    public int BonusCutoffSeconds;
    public int BonusSecondMultiplier;


    public void Awake()
    {
        _savedPoints = GameManager.Instance.Points;// nomes tinc un obj 
        Instance = this;
    }
    public void Start()
    {
        _checkPoints = FindObjectsOfType<Checkpoint>().OrderBy(t => t.transform.position.x).ToList();
        _currentCheckpointIndex = _checkPoints.Count > 0 ? 0:-1;

        Player = FindObjectOfType<Player>();
        ControllerCamera = FindObjectOfType<CameraController>();

        _started = DateTime.UtcNow;

        var listeners = FindObjectsOfType<MonoBehaviour>().OfType<IPlayerRespawnListener>();
        foreach (var listener in listeners)
        {
            for (var i = _checkPoints.Count - 1; i >= 0; i--)
            {
                var distance = ((MonoBehaviour)listener).transform.position.x - _checkPoints[i].transform.position.x;
                if (distance < 0)
                    continue;
                _checkPoints[i].AssignObjectToCheckPoint(listener);
                break;

            }
        }

        // put player in 1frt checkpoint
#if UNITY_EDITOR
        if (DebugSpawn != null)
            DebugSpawn.SpawnPlayer(Player);
        else if (_currentCheckpointIndex != -1)
            _checkPoints[_currentCheckpointIndex].SpawnPlayer(Player);
#else
         if (_currentCheckpointIndex != -1)
            _checkPoints[_currentCheckpointIndex].SpawnPlayer(Player);
#endif


    }
    public void Update()
    {

        var isAtLastCheckpoint = _currentCheckpointIndex + 1 >= _checkPoints.Count;
        if (isAtLastCheckpoint)
            return;
        var distanceToNextCheckpoint = _checkPoints[_currentCheckpointIndex + 1].transform.position.x - Player.transform.position.x;
        if (distanceToNextCheckpoint >= 0)
            return;
        _checkPoints[_currentCheckpointIndex].PlayerLeftCheckPoint();
        _currentCheckpointIndex++;
        _checkPoints[_currentCheckpointIndex].PlayerHitCheckPoint();

        // Todo Timpe Bonus
        GameManager.Instance.AddPoints(currectTimeBonus);
        _savedPoints = GameManager.Instance.Points;
        _started = DateTime.UtcNow;


     



        
    }
    public void KillPlayer()
    {
        StartCoroutine(KillPlayerCo());
    }
    private IEnumerator KillPlayerCo()
    {
        Player.Kill();
        ControllerCamera.IsFollowing = false;
        yield return new WaitForSeconds(2f);
        ControllerCamera.IsFollowing = true;
        if (_currentCheckpointIndex != -1)
            _checkPoints[_currentCheckpointIndex].SpawnPlayer(Player);
        // TODO: Points
        _started = DateTime.UtcNow;
        GameManager.Instance.ResetPoints(_savedPoints);

    }
    public void GoToNextLevel(string nameLevel)
    {
        StartCoroutine(GoToNextLevelCo(nameLevel));
    }
    private IEnumerator GoToNextLevelCo(string nameLevel)
    {
        Debug.Log("sss");
        Player.FinishLevel();
        
        GameManager.Instance.AddPoints(currectTimeBonus);
        FloatingText.Show(string.Format(" Level Complete!"), "CheckpointText", new CenteredTextPosition(.15f));
       yield return new WaitForSeconds(2f);
        FloatingText.Show(string.Format("{0} points!", GameManager.Instance.Points), "CheckpointText", new CenteredTextPosition(.15f));
        yield return new WaitForSeconds(5f);
        if (string.IsNullOrEmpty(nameLevel))
            SceneManager.LoadScene("StartScreen");
        else
            SceneManager.LoadScene(nameLevel);

    }

}

