using UnityEngine;

public class Randomizer
{
    static float _lastPickFloat = float.MinValue;
    static int _lastPickInt = int.MinValue;

    /// <summary>
    /// Returns a random number between <paramref name="min"/> (inclusive) and <paramref name="max"/> (exclusive).
    /// </summary>
    /// <param name="avoidRepeated">Wheather to avoid returning the same value from the last draw.</param>
    public static int GetRandom(int min, int max, bool avoidRepeated = false)
    {
        if (avoidRepeated)
        {
            return GetRandomExcluding(min, max, _lastPickInt);
        }
        else
        {
            return PickRandom(min, max);
        }
    }

    /// <summary>
    /// Returns a random number between <paramref name="min"/> (inclusive) and <paramref name="max"/> (inclusive).
    /// </summary>
    /// <param name="avoidRepeated">Wheather to avoid returning the same value from the last draw.</param>
    public static float GetRandom(float min, float max, bool avoidRepeated = false)
    {
        if (avoidRepeated)
        {
            return GetRandomExcluding(min, max, _lastPickFloat);
        }
        else
        {
            return PickRandom(min, max);
        }
    }

    /// <summary>
    /// Returns a random number between <paramref name="min"/> (inclusive) and <paramref name="max"/> (exclusive), that is never equal to
    /// <paramref name="exclude"/>.
    /// </summary>
    public static int GetRandomExcluding(int min, int max, int exclude)
    {
        int result;

        do
        {
            result = Random.Range(min, max);
        }
        while (result == exclude);

        _lastPickInt = result;
        return result;
    }

    /// <summary>
    /// Returns a random number between <paramref name="min"/> (inclusive) and <paramref name="max"/> (inclusive), that is never equal to
    /// <paramref name="exclude"/>.
    /// </summary>
    public static float GetRandomExcluding(float min, float max, float exclude)
    {
        float result;

        do
        {
            result = Random.Range(min, max);
        }
        while (result == exclude);

        _lastPickFloat = result;
        return result;
    }

    /// <summary>
    /// Draws a random number between 0 and <paramref name="chances"/>, returning <b>true</b> if the result is equal to the latter.
    /// </summary>
    public static bool GetProbability(int chances) => Random.Range(0, chances + 1) == chances;

    /// <summary>
    /// Picks a random number between <paramref name="min"/> (inclusive) and <paramref name="max"/> (exclusive), while also storing it as
    /// the last pick.
    /// </summary>
    static int PickRandom(int min, int max)
    {
        int result = Random.Range(min, max);
        _lastPickInt = result;
        return result;
    }

    /// <summary>
    /// Picks a random number between <paramref name="min"/> (inclusive) and <paramref name="max"/> (inclusive), while also storing it as
    /// the last pick.
    /// </summary>
    static float PickRandom(float min, float max)
    {
        float result = Random.Range(min, max);
        _lastPickFloat = result;
        return result;
    }
}
