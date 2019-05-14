using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ControllerParamaters2D
{ 
    // enum defines if Can Jump on Ground can Jump anyWhere(In whater) or Can't Jump
    public enum JumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnyWhere,
        CantJump
    }
    // Max Velocity defines max velocity by default
    public Vector2 MaxVelocity =new Vector2(float.MaxValue, float.MaxValue);
    // defines slope(inclinació) of the platforms 0-90 
    [Range(0, 90)]
    public float SlopeLimit = 30;
    // defines forçe of gravity in y axe
    public float Gravity= -25f;
    // defines JumpResctricions
    public JumpBehavior JumpRestricions;
    // defines whitch frequency can jump
    public float JumpFrequency= .25f;

    public float JumpMagnitude = 12;



}
