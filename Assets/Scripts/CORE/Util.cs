﻿using System;
using UnityEngine;
using System.Collections.Generic;

public class Util
{
    public static string GenerateUniqueID()
    {
        return Guid.NewGuid().ToString("N");
    }

    public static bool EquationResult(float A, Equation currentEquation ,float B)
    {
        switch (currentEquation)
        {
            case Equation.Above:
                {
                    return A > B;
                }
            case Equation.AboveOrEquals:
                {
                    return A >= B;
                }
            case Equation.Below:
                {
                    return A < B;
                }
            case Equation.BelowOrEquals:
                {
                    return A <= B;
                }
            default:
                {
                    return A == B;
                }
        }
    }

    public static string FormatTags(string content, Dictionary<string, object> parameters,GenderType genderType = GenderType.Male)
    {
        if (parameters != null)
        {            
            foreach (string key in parameters.Keys)
            {
                if (!content.Contains("{" + key + "}"))
                {
                    continue;
                }

                content = content.Replace("{" + key + "}", (string)parameters[key]);
            }

            if (parameters.ContainsKey("Actor"))
            {
                genderType = ((Character)parameters["Actor"]).Gender;
            }

            if (genderType == GenderType.Female)
            {
                content.Replace(" him ", " her ");
                content.Replace(" his ", " hers ");
                content.Replace(" he ", " she ");
                content.Replace(" Him ", " Her ");
                content.Replace(" His ", " Hers ");
                content.Replace(" He ", " She ");
            }
        }

        return content;
    }

    public static Vector3 SplineLerpY(Vector3 source, Vector3 target, float Height, float t)
    {
        Vector3 ST = new Vector3(source.x, source.y + Height, source.z);
        Vector3 TT = new Vector3(target.x, target.y + Height, target.z);

        Vector3 STTTM = Vector3.Lerp(ST, TT, t);

        Vector3 STM = Vector3.Lerp(source, ST, t);
        Vector3 TTM = Vector3.Lerp(TT, target, t);

        Vector3 SplineST = Vector3.Lerp(STM, STTTM, t);
        Vector3 SplineTM = Vector3.Lerp(STTTM, TTM, t);

        return Vector3.Lerp(SplineST, SplineTM, t);
    }

    public static Vector3 SplineLerpX(Vector3 source, Vector3 target, float Height, float t)
    {
        Vector3 ST = new Vector3(source.x + Height, source.y, source.z);
        Vector3 TT = new Vector3(target.x + Height, target.y, target.z);

        Vector3 STTTM = Vector3.Lerp(ST, TT, t);

        Vector3 STM = Vector3.Lerp(source, ST, t);
        Vector3 TTM = Vector3.Lerp(TT, target, t);

        Vector3 SplineST = Vector3.Lerp(STM, STTTM, t);
        Vector3 SplineTM = Vector3.Lerp(STTTM, TTM, t);

        return Vector3.Lerp(SplineST, SplineTM, t);
    }




}
