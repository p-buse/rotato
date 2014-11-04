public class LevelBlock
{
    string name;
    int orientation;
    bool spiky;
    int spikeOrientation;

    public LevelBlock(string name, int orientation, bool spiky, int spikeOrientation)
    {
        this.name = name;
        this.orientation = orientation;
        this.spiky = spiky;
        this.spikeOrientation = spikeOrientation;
    }

    public LevelBlock(string name)
    {
        this.name = name;
        this.orientation = 0;
        this.spiky = false;
        this.spikeOrientation = 0;
    }

    public LevelBlock(string name, int orientation)
    {
        this.name = name;
        this.orientation = orientation;
        this.spiky = false;
        this.spikeOrientation = 0;
    }
}