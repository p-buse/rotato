using UnityEngine;
using System.Collections.Generic;

public class GameDataSkeleton {

    public struct LevelDataSkeleton
    {
        public int levelIndex;
        public int veggieCount;
        public int yourBestVeggies;
        public int fewestRotations;
    }

    public List<LevelDataSkeleton> levelData;
    public int unlockedLevel;
	
}
