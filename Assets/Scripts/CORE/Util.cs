using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

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

            if (!parameters.ContainsKey("NoGender"))
            {
                if (parameters.ContainsKey("Actor") && (Character)parameters["Actor"] != null)
                {
                    genderType = ((Character)parameters["Actor"]).Gender;
                }
            }

            if (genderType == GenderType.Female)
            {
                content = Regex.Replace(content, @"\bhim\b", "her");
                content = Regex.Replace(content, @"\bhis\b", "her");
                content = Regex.Replace(content, @"\bhe\b", "she");
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

    private static int[] DTGetShiftIndexes(string key)
    {
        int keyLength = key.Length;
        int[] indexes = new int[keyLength];
        List<KeyValuePair<int, char>> sortedKey = new List<KeyValuePair<int, char>>();
        int i;

        for (i = 0; i < keyLength; ++i)
            sortedKey.Add(new KeyValuePair<int, char>(i, key[i]));

        sortedKey.Sort(
            delegate (KeyValuePair<int, char> pair1, KeyValuePair<int, char> pair2) {
                return pair1.Value.CompareTo(pair2.Value);
            }
        );

        for (i = 0; i < keyLength; ++i)
            indexes[sortedKey[i].Key] = i;

        return indexes;
    }

    public static string DTEncipher(string input, string key, char padChar)
    {
        input = (input.Length % key.Length == 0) ? input : input.PadRight(input.Length - (input.Length % key.Length) + key.Length, padChar);
        StringBuilder output = new StringBuilder();
        int totalChars = input.Length;
        int totalColumns = key.Length;
        int totalRows = (int)Math.Ceiling((double)totalChars / totalColumns);
        char[,] rowChars = new char[totalRows, totalColumns];
        char[,] colChars = new char[totalColumns, totalRows];
        char[,] sortedColChars = new char[totalColumns, totalRows];
        int currentRow, currentColumn, i, j;
        int[] shiftIndexes = DTGetShiftIndexes(key);

        for (i = 0; i < totalChars; ++i)
        {
            currentRow = i / totalColumns;
            currentColumn = i % totalColumns;
            rowChars[currentRow, currentColumn] = input[i];
        }

        for (i = 0; i < totalRows; ++i)
            for (j = 0; j < totalColumns; ++j)
                colChars[j, i] = rowChars[i, j];

        for (i = 0; i < totalColumns; ++i)
            for (j = 0; j < totalRows; ++j)
                sortedColChars[shiftIndexes[i], j] = colChars[i, j];


        for (i = 0; i < totalChars; ++i)
        {
            currentRow = i / totalRows;
            currentColumn = i % totalRows;
            output.Append(sortedColChars[currentRow, currentColumn]);
        }

        return output.ToString();
    }

    public static string DTDecipher(string input, string key)
    {
        StringBuilder output = new StringBuilder();
        int totalChars = input.Length;
        int totalColumns = (int)Math.Ceiling((double)totalChars / key.Length);
        int totalRows = key.Length;
        char[,] rowChars = new char[totalRows, totalColumns];
        char[,] colChars = new char[totalColumns, totalRows];
        char[,] unsortedColChars = new char[totalColumns, totalRows];
        int currentRow, currentColumn, i, j;
        int[] shiftIndexes = DTGetShiftIndexes(key);

        for (i = 0; i < totalChars; ++i)
        {
            currentRow = i / totalColumns;
            currentColumn = i % totalColumns;
            rowChars[currentRow, currentColumn] = input[i];
        }

        for (i = 0; i < totalRows; ++i)
            for (j = 0; j < totalColumns; ++j)
                colChars[j, i] = rowChars[i, j];

        for (i = 0; i < totalColumns; ++i)
            for (j = 0; j < totalRows; ++j)
                unsortedColChars[i, j] = colChars[i, shiftIndexes[j]];

        for (i = 0; i < totalChars; ++i)
        {
            currentRow = i / totalRows;
            currentColumn = i % totalRows;
            output.Append(unsortedColChars[currentRow, currentColumn]);
        }

        return output.ToString();
    }


}
