using UnityEngine;

public abstract class  Projectile : MonoBehaviour
{
    public float Speed;
    public LayerMask CollisionMask;
    
    public GameObject Owner { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector2 InitialVelocity { get; private set; }

    public void Initialize(GameObject owner,Vector2 direction, Vector2 initialVelocity)
    {
        transform.right = direction;
        Owner = owner;
        Direction = direction;
        InitialVelocity = initialVelocity;
        OnInitialized();
    }
    protected virtual void OnInitialized()
    {

    }
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Layer #   Binary      Decimal
        // Layer 0 =  0000 0001 = 1
        // Layer 1 =  0000 0010 = 2
        // Layer 2 =  0000 0100 = 4
        // Layer 3 =  0000 1000 = 8
        // Layer 4 =  0001 0000 = 16
        // Layer 5 =  0010 0000 = 32
        // Layer 6 =  0100 0000 = 64
        // Layer 7  =  1000 0000 = 128
        // other gameObject layer it is Layer # of list itself

        // collision mask is a bainari representaton of combining multyple bianari representations of eachone of this layers

        //layer #5+#2=0010 0100
        //layer = #1#2#5#6=0110 0110
        //LayerMask = 0110 0110
        // other.gameObject.layer=5
        // in that we are preguntar
        // is layer 5 in the mask?
        // x saver s una layers esta a la mask fa aixo
        // we need to turn number 5 into 32
        // esay (1<<5)=23
        // << moves the bites 1 number of times N to the left
        // if we have num 1 in banary=0000 0001 
        // 0000 0001 << 5 = 0010 0000
        // we appli << we result  0010 0000 =32 or 5
        // 0110 0110 
        // & (and)
        // 0010 0000
        //---------
        // 0010 0000 = 1

        // other.gameObject.layer=7
        // in that we are preguntar
        // is layer 7 in the mask?

        // we need to turn number 7 into 128
        // esay (1<<7)=128
        // << moves the bites 1 number of times N to the left
        // if we have num 1 in banary=0000 0001 
        // 0000 0001 << 0 = 1000 0000
        // we appli << we result  1000 0000 =128 or layer 7
        // 0110 0110 
        // & (and)
        // 1000 0000
        //---------
        // 0000 0000 = 0
        
        if ((CollisionMask.value & (1<< other.gameObject.layer)) ==0)
        {
            OnNotCollideWith(other);
            return;
        }
        var isOwner = other.gameObject == Owner;
        if (isOwner)
        {
            OnCollideOwner();
        }

        var takeDamage = (ITakeDamage)other.GetComponent(typeof(ITakeDamage));
        if(takeDamage!=null)
        {
            OnCollideTakeDamage(other, takeDamage);
            return;
        }
        OnCollideOther(other);

    }
    protected  virtual void OnNotCollideWith(Collider2D other)
    {

    }
    protected virtual void OnCollideOwner()
    {

    }
    protected virtual void OnCollideTakeDamage(Collider2D other,ITakeDamage takeDamage)
    {

    }
    protected virtual void OnCollideOther(Collider2D other)
    {

    }
}

