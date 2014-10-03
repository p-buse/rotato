using UnityEngine;

public class Int2
{
    public int x;
    public int y;
    public Int2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Int2(float x, float y)
    {
        this.x = Mathf.RoundToInt(x);
        this.y = Mathf.RoundToInt(y);
    }

    public Int2 Add(Int2 otherInt2)
    {
        return new Int2(this.x + otherInt2.x, this.y + otherInt2.y);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as Int2);
    }

    public bool Equals(Int2 obj)
    {
        return obj.x == this.x && obj.y == this.y;
    }

    /// <summary>
    /// From StackOverflow: http://stackoverflow.com/questions/5221396/what-is-an-appropriate-gethashcode-algorithm-for-a-2d-point-struct-avoiding
    /// Yay! A hashcode that works.
    /// </summary>
    /// <returns>A unique hashcode for a 2D point.</returns>
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        return "x: " + x + "\ny: " + y;
    }
}