using System;

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

}
