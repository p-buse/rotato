using UnityEngine;

public abstract class Block : MonoBehaviour
{
    protected int x;
    protected int y;
    /// <summary>
    /// Rotates the block, given a center position and a direction.
    /// Direction can be "cw" or "ccw"
    /// </summary>
    /// <param name="xCenter">The x position of the point to rotate around</param>
    /// <param name="yCenter">The y position of the point to rotate around</param>
    /// <param name="direction">The direction (can be "cw" or "ccw")</param>
    public abstract void RotateTo(int xCenter, int yCenter, string direction);
    public abstract bool invalidatesRotation();

}
    