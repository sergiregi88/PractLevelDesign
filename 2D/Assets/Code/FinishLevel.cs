using UnityEngine;
class FinishLevel : MonoBehaviour
{
    public string LevelName;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() == null)
            return;
        LevelManager.Instance.GoToNextLevel(LevelName);
    }
}

