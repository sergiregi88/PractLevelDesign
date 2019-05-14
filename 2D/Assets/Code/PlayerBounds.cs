using UnityEngine;
class PlayerBounds :MonoBehaviour
{
    public enum BoundsBehaviour
    {
        Nothing,
        Contraint,
        Kill
    }

    public BoxCollider2D Bounds;
    public BoundsBehaviour Above;
    public BoundsBehaviour Below;
    public BoundsBehaviour Left;
    public BoundsBehaviour Right;

    private Player _player;
    private BoxCollider2D _boxCollider;


    public void Start()
    {
        _player = GetComponent<Player>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Update()
    {
        if (_player.IsDead)
        
            return;
        //calculate size of box collider
        Debug.Log("d" + transform.localScale.x+_boxCollider);
        var colliderSize = new Vector2(
            _boxCollider.size.x * Mathf.Abs(transform.localScale.x), 
            _boxCollider.size.y * Mathf.Abs(transform.localScale.y));

        if (Above != BoundsBehaviour.Nothing && transform.position.y + colliderSize.y > Bounds.bounds.max.y)
            ApplyBoundsBehaviourBounds(Above, new Vector2(transform.position.x, Bounds.bounds.max.y - colliderSize.y));
        if(Below != BoundsBehaviour.Nothing && transform.position.y - colliderSize.y< Bounds.bounds.min.y)
            ApplyBoundsBehaviourBounds(Below, new Vector2(transform.position.x, Bounds.bounds.min.y + colliderSize.y));
        if (Right != BoundsBehaviour.Nothing && transform.position.x + colliderSize.x > Bounds.bounds.max.x)
            ApplyBoundsBehaviourBounds(Right, new Vector2(Bounds.bounds.max.x - colliderSize.x, transform.position.y));
        if (Left != BoundsBehaviour.Nothing && transform.position.x - colliderSize.x < Bounds.bounds.min.x)
            ApplyBoundsBehaviourBounds(Left, new Vector2(Bounds.bounds.min.x + colliderSize.x, transform.position.y ));
    }
    private void ApplyBoundsBehaviourBounds(BoundsBehaviour behaviuor, Vector2 constrainedPosition)
    {
        if(behaviuor == BoundsBehaviour.Kill)
        {
            LevelManager.Instance.KillPlayer();
            return;
        }
        transform.position = constrainedPosition;
    }

}

