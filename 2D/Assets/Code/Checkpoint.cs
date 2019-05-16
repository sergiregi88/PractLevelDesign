using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint :MonoBehaviour
{
    private List<IPlayerRespawnListener> _listeners;
    public void Awake()
    {
        _listeners = new List<IPlayerRespawnListener>();
    }
    public void PlayerHitCheckPoint()
    {
        StartCoroutine(PlayerHitCheckPointCo(LevelManager.Instance.currectTimeBonus));
    }
    private IEnumerator PlayerHitCheckPointCo(int bonus)
    {
        FloatingText.Show("CheckPoint", "CheckpointText", new CenteredTextPosition(.5f));
        yield return new WaitForSeconds(.5f);
       // FloatingText.Show(string.Format("+{0} time bonus",bonus), "CheckpointText",new CenteredTextPosition(.5f));
    }
    public void PlayerLeftCheckPoint()
    {

    }
    public void SpawnPlayer(Player player)
    {
        player.RespawnAt(transform);
        foreach(var listener in _listeners)
        
            listener.OnPlayerRespawnInThisCheckpoint(this, player);
        
    }
    public void AssignObjectToCheckPoint(IPlayerRespawnListener listener)
    {
        Debug.Log("Hello");
        _listeners.Add(listener);
    }
}

