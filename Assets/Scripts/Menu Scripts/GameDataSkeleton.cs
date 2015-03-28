using UnityEngine;
using System.Collections.Generic;

public class GameDataSkeleton {
    public int unlockedLevel = GameData.UNLOCKED_LEVELS_DEFAULT;
    public List<LevelDataSkeleton> levelData;
    public struct LevelDataSkeleton
    {
        public int levelIndex;
        public int veggieCount;
        public int yourBestVeggies;
        public int fewestRotations;
    }

    public GameDataSkeleton()
    {
        this.levelData = new List<LevelDataSkeleton>();
    }

}
