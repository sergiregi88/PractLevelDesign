using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class FollowPath : MonoBehaviour {
    //define type for sai when Move Towards and lerp
    public enum FollowType
    {
        MoveTowards,
        Lerp
    }
    // Folloy Type of the Path
    public FollowType Type = FollowType.MoveTowards;
    // Path to follow 
    public PathDefinition Path;
    // Speed of the platform who follow
    public float Speed = 1f;
    // defines max distance  to goal
    public float MaxDistanceToGoal = .1f;
    // defines IEnumerator for iteration in Path Definition
    private IEnumerator<Transform> _currentPoint;
    public void Start()
    {
        // if path is null return and error msg
        if (Path == null)
        {
            Debug.LogError("Path cannot be null", gameObject);
            return;
        }
        
        //get initial point of path from IEnumerator
        _currentPoint = Path.GetPathsEnumerator();
        // move next pooint to initate IEnumerator loop 
        _currentPoint.MoveNext();
        // check if _currentPoint.currect has point
        if (_currentPoint.Current == null)
            return;
        // move the sprite to current position
        transform.position = _currentPoint.Current.position;
    }
    public void Update()
    {
        //if not exists _currentPoint return
        if (_currentPoint == null || _currentPoint.Current == null)
            return;
        // if Type is moving Towards 
        if (Type == FollowType.MoveTowards)
            // set position of Move Towards whit current position , target position of _currentPoint.current. position, Time deltatime*Speed max distance we gonna move)
            transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * Speed);
        // if Type is LERP
        else if (Type == FollowType.Lerp)
            // set position of Lerp whit transform position , _currentPoint.current. position, Time deltatime*Speed)
            transform.position = Vector3.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * Speed);
        // When hitting point we need to move to the next point 
        // do to that get the squared magnitude of vector3 result from substract object current position - current point position  
        var distanceSquared = (transform.position - _currentPoint.Current.position).sqrMagnitude;
        // si distanceSquared < MaxDistance X MaxDistanceToGoal Move Next
        if (distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal)
            _currentPoint.MoveNext();



    }
}
