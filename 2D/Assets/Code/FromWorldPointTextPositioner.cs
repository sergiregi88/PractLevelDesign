using UnityEngine;

public  class FromWorldPointTextPositioner : IFloatingTextPositioner
{
    private readonly Camera _camera;
    private readonly Vector3 _worldPosition;
    private  float _timeToLive;
    private  float _speed;
    private float _yOffset;

    public FromWorldPointTextPositioner(Camera camera, Vector3 wordPosition,float timeToLive=.5f, float speed=40f )
    {
        _camera = camera;
        _worldPosition = wordPosition;
        _timeToLive = timeToLive;
        _speed = speed;
        

    }
    public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
    {

        // idicates that is time to destroy floating text
        // if Time to live -= deltatime is minus or equals 0 retuens false
        // every frame we calclates TimeTLive-= Timedeltatime if this is min to 0 
        if ((_timeToLive -= Time.deltaTime) <= 0)
            return false;
        var screenPosition = _camera.WorldToScreenPoint(_worldPosition);
        position.x = screenPosition.x - (size.x / 2);
        position.y = Screen.height - screenPosition.y - _yOffset;
        _yOffset += Time.deltaTime * _speed;
        
        return true;
        
        

    }
}

