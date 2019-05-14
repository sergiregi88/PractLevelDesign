using UnityEngine;

class FollowObject : MonoBehaviour
{
    public Vector2 Offset;
    public Transform Following;
    public void Update()
    {
        transform.position = Following.transform.position + (Vector3)Offset;
    }
}

