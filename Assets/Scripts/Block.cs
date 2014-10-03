using UnityEngine;

public abstract class Block : MonoBehaviour
{
    protected Int2 gridPosition;
    /// <summary>
    /// Rotates the block, given a center position and a direction.
    /// Direction can be "cw" or "ccw"
    /// </summary>
    /// <param name="xCenter">The x position of the point to rotate around</param>
    /// <param name="yCenter">The y position of the point to rotate around</param>
    /// <param name="direction">The direction (can be "cw" or "ccw")</param>
    public abstract void RotateTo(Int2 center, string direction);
    public abstract bool invalidatesRotation();

}