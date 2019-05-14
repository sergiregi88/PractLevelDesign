
using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour {
    // width of skin collider 
    private const float SkinWidth = .02f;
    // Vertical Rays
    private const int TotalVerticalRays = 4;
    // Horizontal Rays
    private const int TotalHorizontalRays = 8;
    // Slope Limit Tangent 75f Deg to Rad
    private static readonly float SlopeLimitTangent= Mathf.Tan(75f*Mathf.Deg2Rad);

    // Platform Mask of colliders
    public LayerMask PlatformMask;
    // Parameters of ControllerParameters 
    public ControllerParamaters2D DefaultParameters;
    // State Controllers 
    public ControllerState2D State { get; private set; }
    // velocity of the controller character
    public Vector2 Velocity{ get { return _velocity; }}
    // platform velocity
    public Vector3 PlaformVelocity { get; private set; }
    // FLAG SAY IF HANDLE COLLISONS
    public bool HandleCollisions;
    // if OverriveParameters is not null return DefaultParameters else return overrideparameters
    public ControllerParamaters2D Parameters { get {return  _overrideParameters ?? DefaultParameters; } }
    public GameObject StandingOn {get; private set;}
    // flag says if Player Can Jump
    public bool CanJump
    {
        get{
            if (Parameters.JumpRestricions == ControllerParamaters2D.JumpBehavior.CanJumpAnyWhere)
                return _jumpIn <= 0;
            if (Parameters.JumpRestricions == ControllerParamaters2D.JumpBehavior.CanJumpOnGround) 
                return State.IsGrounded;
            return false;
        }
    }

    //fields
    //velocity local
    private Vector2 _velocity;
    // transform local
    private Transform _transform;
    // localScale local
    private Vector3 _localScale;
    // boxCollider local
    private BoxCollider2D _boxCollider;
    // Horizontal verical distance between rays to calculate de distance of hit box collider
    private float _verticalDistanceBetweenRays,
        _horizontalDistanceBetweenRays;
    private ControllerParamaters2D _overrideParameters;
    private Vector3
        _rayCastTopLeft,
        _rayCastBottomLeft, 
        _rayCastBottomRight;
    private float _jumpIn;
    private Vector3 _activeGlobalPlatformPoint, _activeLocalPlatformPoint;
    private GameObject _lastStandingOn;
    


    void Awake()
    {
        HandleCollisions = true;
        State = new ControllerState2D();
        _transform = transform;
        _localScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider2D>();
        // get the collider width to calculate _horizontalDistanceBetweenRays
        // to do that we get size x * scale x (to get the size x when obj is scaled) - 2 *skinwidth of the collider skin   
        var colliderWidth = _boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2 * SkinWidth);
        // to get _horizontal Distance the colliderwith dividing by TotalVerticalRays -1 because Because we have 4 rays and we need 3 slices
        _horizontalDistanceBetweenRays = colliderWidth / (TotalVerticalRays - 1);

        // get the collider width to calculate _verticalDistanceBetweenRays
        // to do that we get size.y * scale.y (to get the size y when obj is scaled) - 2 *skinwidth of the collider skin   
        var colliderHeight = _boxCollider.size.y * Mathf.Abs(transform.localScale.y) - (2 * SkinWidth);
        // to get _verical Distance the collider with dividing by TotalHorizontalRays -1 because Because we have 8 rays and we need 3 slices
        _verticalDistanceBetweenRays = colliderHeight / (TotalHorizontalRays - 1);
    }
    public void AddForce(Vector2 force)
    {
        _velocity += force;
    }
    public void SetForce(Vector2 force)
    {
        _velocity += force;
    }
    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }
    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }
    public void Jump()
    {
        // todo moving platforms
        AddForce(new Vector2(0, Parameters.JumpMagnitude));
        var _jumpIn = Parameters.JumpFrequency;

    }
	// Use this for initialization

	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        _jumpIn -= Time.deltaTime;
        _velocity.y += Parameters.Gravity * Time.deltaTime;
        Move(Velocity * Time.deltaTime);    

	}
    private void Move(Vector2 deltaMovement)
    {
        // get if is colliding below
        var wasGraunded = State.IsCollidingBelow;
        // Set States to false 
        State.Reset();
        // If Handle Collisions
        if(HandleCollisions)
        {
            // HandleMovingPlatforms
            HandlePlatforms();
            // Every time player moves we need to calculate ray origins 
            CalculateRayOrigins();
            // if is affected by gravity i is on the ground
            if (deltaMovement.y < 0 && wasGraunded)
                HandleVerticalSlope(ref deltaMovement);
            //if is moving to the left or to the right
            if (Mathf.Abs(deltaMovement.x) > .001f)
                MoveHorizontaly(ref deltaMovement);

            // Move verticali appliys always because the force og gravity is aplied always
            MoveVerticaly(ref deltaMovement);

            CorrectHorizontalPlacement(ref deltaMovement, true);
            CorrectHorizontalPlacement(ref deltaMovement, false);
        }
        _transform.Translate(deltaMovement, Space.World);

         
        // think it
        if (Time.deltaTime > 0)
            
            _velocity = deltaMovement / Time.deltaTime;
        
        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        if(State.IsMovingUpSlope)
            _velocity.y =0;

        /// Platforms 
        if (StandingOn != null)
        {
            _activeGlobalPlatformPoint = transform.position;
            _activeLocalPlatformPoint = StandingOn.transform.InverseTransformPoint(transform.position);
            //Debug.DrawLine(transform.position, _activeGlobalPlatformPoint);
            //Debug.DrawLine(transform.position, _activeLocalPlatformPoint + StandingOn.transform.position);

            // if 
            if(_lastStandingOn!=StandingOn)
            {
                if (_lastStandingOn != null)
                    _lastStandingOn.SendMessage("ControllerExit2D", this, SendMessageOptions.DontRequireReceiver);

                StandingOn.SendMessage("ControllerEnter2D", this, SendMessageOptions.DontRequireReceiver);
                    _lastStandingOn = StandingOn;
                

            }
            else if (StandingOn!=null)
            {
                StandingOn.SendMessage("ControllerStay2D", this, SendMessageOptions.DontRequireReceiver);
            }

        }
        else if(_lastStandingOn!=null)
        {
            _lastStandingOn.SendMessage("ControllerExit2D",this,SendMessageOptions.DontRequireReceiver);
        }
    }
    private void HandlePlatforms()
    {

       if ( StandingOn != null)
       {
           var newGlobalPlatformPoint = StandingOn.transform.TransformPoint(_activeLocalPlatformPoint);
           var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;

           if (moveDistance != Vector3.zero)
               transform.Translate(moveDistance, Space.World);

           PlaformVelocity = (newGlobalPlatformPoint - _activeGlobalPlatformPoint) / Time.deltaTime;
        }
        else

            PlaformVelocity = Vector3.zero;

        StandingOn = null;
    }
    private void CorrectHorizontalPlacement(ref Vector2 deltaMovement, bool isRight)
    {
        var halfWidth = (_boxCollider.size.x * _localScale.x) / 2;
        var rayOrigin = isRight ? _rayCastBottomRight : _rayCastBottomLeft;
        if (isRight)
            rayOrigin.x -= (halfWidth - SkinWidth);
        else
            rayOrigin.x += (halfWidth - SkinWidth);
        var rayDirection = isRight ? Vector2.right : -Vector2.right;
        var offset = 0f;
        for (var i = 1; i < TotalHorizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, deltaMovement.y + rayOrigin.y + (i * _verticalDistanceBetweenRays));
            // Debug.DrawRay(rayVector, rayDirection * halfWidth,isRight? Color.cyan:Color.magenta);
            var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, halfWidth, PlatformMask);
            if (!rayCastHit)
                continue;
            offset = isRight ? ((rayCastHit.point.x - _transform.position.x) - halfWidth) : (halfWidth - (_transform.position.x - rayCastHit.point.x));
        }
        deltaMovement.x += offset;





    }
    private void CalculateRayOrigins()
    {
        //   size and center of collider the with factor sclaled of player 
        // get size of collider Vector2 with BoxCollider.size.x * scale.x to get the x and the same for y  
        // that gets the total size of the collider we need dividd by 2 to get the size in scaled 
        var size = new Vector2(_boxCollider.size.x * Mathf.Abs(_localScale.x), _boxCollider.size.y * Mathf.Abs(_localScale.y))/2;
        //get center of box colliders
        // get center of collider boxcollidr.center.x * localscale.x to gey the x th same for y 
        var center =new Vector2(_boxCollider.offset.x * _localScale.x, _boxCollider.offset.y * _localScale.y);
       // Debug.Log("center x"+center.x+ " center y"+ center.y+" trns positon x"+ _transform.position.x+" trans positn y="+ _transform.position.y);
        // we need to know 3 points of collider TopLeft BottomRight Bottom Left that  modifies when player moves 
        // to find points 
        // rayCastTopLeft  center.x - size.x is left of the collider and center.y + size.y is top o collider  
        _rayCastTopLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y + size.y - SkinWidth);
        _rayCastBottomRight = _transform.position + new Vector3(center.x + size.x - SkinWidth, center.y - size.y + SkinWidth);
        _rayCastBottomLeft = _transform.position + new Vector3(center.x - size.x + SkinWidth, center.y - size.y + SkinWidth);
        //Debug.Log(_rayCastTopLeft.x + "   " + _rayCastTopLeft.y);

    }
    private void MoveHorizontaly(ref Vector2 deltaMovement)
    {
        // get is going right or left
        var isGoingRight = deltaMovement.x > 0;
        // get distance of ray 
        // movement.x + skin = how far the player moves in x axe
        var rayDistance= Mathf.Abs(deltaMovement.x) + SkinWidth;
        // get direccion of ray
        // 
        var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
        //get origin of ray
        // edge of colider if we going ritgh or left of the collider 
        var rayOrigin = isGoingRight ? _rayCastBottomRight : _rayCastBottomLeft;
        // loop the Rays to draw and create
        for (var i=0; i<TotalHorizontalRays;i++)
        {
            // create ray vector for each ray
            // x was the same for each ray
            // y was modifing + i* _verticalDistance between rays
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);
            // calcule ray hit with platforms
            var rayCastHit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, PlatformMask);
            // is not ray hit 
            if (!rayCastHit)
                // continue with loop
                continue;
            // we hit something

            if (i == 0 && HandleHorizontalSlope(ref deltaMovement, Vector2.Angle(rayCastHit.normal, Vector2.up), isGoingRight))
                break;
            // stops the player by rayCastHit.potin.x - rayvector.x 

            deltaMovement.x = rayCastHit.point.x - rayVector.x;

            rayDistance = Mathf.Abs(deltaMovement.x);

            if(isGoingRight)
            {
                deltaMovement.x -= SkinWidth;
                State.IsCollidingRight = true;
            }
            else
            {
                deltaMovement.x += SkinWidth;
                State.IsCollidingLeft = true;
            }
            if (rayDistance < SkinWidth + 0.0001f)
                break;



            
        }

    }
    // its appling alwasys  because gravity
    private void MoveVerticaly(ref Vector2 deltaMovement)
    {
        // get bool isGoingUp if deltaMovemt.y is > 0
        var isGoingUp = deltaMovement.y > 0;
        // gat ray Distance with distance of moviment + skinwith 
        var rayDistance = Mathf.Abs(deltaMovement.y) + SkinWidth;
        // get ray Directon if is going up or down
        var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
        // get rayOrigin if is going up or down
        var rayOrigin = isGoingUp ? _rayCastTopLeft : _rayCastBottomLeft;
        //adds deltamovement.x to rayOrigin.x
        rayOrigin.x += deltaMovement.x;
        // delines staninOnDistance
        var standingOnDistance = float.MaxValue;
        // loop vertical Rays
        for(var i=0; i< TotalVerticalRays; i++)
        {
            // create ray vector for each ray
            // y was the same for each ray
            // x was modifing + i* _horizontalDistance between rays
            var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDirection * rayDistance, Color.red);
            // calcle hit between platforms
            var rayCastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, PlatformMask);
            // doesn't hit
            if (!rayCastHit)
                // continue
                continue;
            // if not going up
            if(!isGoingUp)
            {
                // is going down
                // get vertical distance to hit= position.y- rayCastHit.y
                var verticalDistanceToHit = _transform.position.y - rayCastHit.point.y;
                if (verticalDistanceToHit < standingOnDistance)
                    StandingOn = rayCastHit.collider.gameObject;
            }

            deltaMovement.y = rayCastHit.point.y - rayVector.y;
            rayDistance = Mathf.Abs(deltaMovement.y);
            //is going up
            if(isGoingUp)
            {
                //substract skinwidth deltamovement y
                deltaMovement.y -= SkinWidth;
                State.IsCollidingAbove = true;
            }
            else
            {
                // add skinwitdh to deltamovemt .y
                deltaMovement.y += SkinWidth;
                State.IsCollidingBelow = true;
            }
            if (!isGoingUp && deltaMovement.y > .0001f)
                State.IsMovingUpSlope = true;
            if (rayDistance < SkinWidth + .0001f)
                break;
        }


    }
    private void HandleVerticalSlope(ref Vector2 deltaMovement)
    {
        // get center of collider
        var center = (_rayCastBottomLeft.x + _rayCastBottomRight.x) / 2;
        // get direccion
        var direction = -Vector2.up;
        //get SlopeDistance
        var slopeDistance = SlopeLimitTangent * (_rayCastBottomRight.x - center);
        // get SlopeRay vector
        var slopeRayVector = new Vector2(center, _rayCastBottomLeft.y);
        Debug.DrawRay(slopeRayVector, direction * slopeDistance, Color.yellow);
        // calcule raycashit
        var rayCastHit = Physics2D.Raycast(slopeRayVector, direction, slopeDistance, PlatformMask);
        //if not raycasthit
        if (!rayCastHit)
            // conintue
            return;
        // detects if is moving down slope
        var isMovingDownSlope = Mathf.Sign(rayCastHit.normal.x) == Mathf.Sign(deltaMovement.x);
        if (!isMovingDownSlope)
            return;
        var angle = Vector2.Angle(rayCastHit.normal, Vector2.up);
        // angle is 0 no have slope ground
        if (Mathf.Abs(angle) < .0001f)
            return;
        // we have an slope ground and moving down
        State.IsMovingDownSlope = true;
        State.SlopeAngle = angle;

        deltaMovement.y = rayCastHit.point.y - slopeRayVector.y;




    }

    private bool HandleHorizontalSlope(ref Vector2 deltaMovement,float angle, bool isGoingRight)
    {
        if (Mathf.RoundToInt(angle) == 90)
            return false;
        if(angle > Parameters.SlopeLimit)
            {
                deltaMovement.x = 0;
                return true;
            }
        if (deltaMovement.y > .07f)
            return true;
        deltaMovement.x += isGoingRight ? -SkinWidth : SkinWidth;
        deltaMovement.y = Mathf.Abs(Mathf.Tan(angle * Mathf.Deg2Rad) * deltaMovement.x);
        State.IsMovingUpSlope = true;
        State.IsCollidingBelow = true;
        return true;

   }

    public void OnTriggerEnter2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhsyicsVolume2D>();
        if (parameters == null)
            return;
        _overrideParameters = parameters.Parameters;
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        var parameters = other.gameObject.GetComponent<ControllerPhsyicsVolume2D>();
        if (parameters == null)
            return;
        _overrideParameters = null;
    }
}
