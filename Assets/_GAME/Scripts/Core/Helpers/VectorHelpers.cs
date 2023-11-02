using UnityEngine;

public static class VectorHelpers
{
    /// <summary>
    /// Returns a flattened version of <paramref name="vector"/>, with the <i>y</i> value set to 0.
    /// </summary>
    public static Vector3 Flat(this Vector3 vector)
    {
        vector.y = 0.0f;
        return vector;
    }

    /// <summary>
    /// Returns the flipped version of <paramref name="vector"/> (all values multiplied by -1).
    /// </summary>
    public static Vector3 Flipped(this Vector3 vector)
    {
        return vector * -1;
    }
}
