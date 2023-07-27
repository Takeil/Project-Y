using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartsManager
{
    public static float maxHeadHP = 25;
    public static float maxNeckHP = 25;

    public static float maxTorsoHP = 100;

    public static float maxUpperArmHP = 50;
    public static float maxForearmHP = 50;
    public static float maxHandHP = 25;

    public static float MaxThighP = 50;
    public static float maxLegHP = 50;
    public static float maxFootHP = 25;

    /*bool DeathCheck(BodyParts bodyParts)
    {
        //if Head, Neck, Torso is Destroyed
        if (bodyParts.TorsoHP <= 0 || bodyParts.NeckHP <= 0 || bodyParts.HeadHP <= 0)
            return true;
        else
            return false;
    }

    BodyParts ArmCheck(BodyParts bp, int isRight)
    {
        BodyParts bodyParts = bp;

        if (bodyParts.UpperArmHP[isRight] <= 0)
        {
            // Remove Forearm;
            bodyParts.ForearmHP[isRight] = 0;
        }
        if (bodyParts.ForearmHP[isRight] <= 0)
        {
            // Remove hand;
            bodyParts.HandHP[isRight] = 0;
        }

        return bodyParts;
    }

    BodyParts LegCheck(BodyParts bp, int isRight)
    {
        BodyParts bodyParts = bp;

        if (bodyParts.ThighHP[isRight] <= 0)
        {
            // Remove leg;
            bodyParts.LegHP[isRight] = 0;
        }
        if (bodyParts.LegHP[isRight] <= 0)
        {
            // Remove foot;
            bodyParts.FootHP[isRight] = 0;
        }

        return bodyParts;
    }*/
}
