using System;
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

}
