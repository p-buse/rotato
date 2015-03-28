using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System;

public class GameData: ScriptableObject {

    public static int UNLOCKED_LEVELS_DEFAULT = 45;

    // Making a singleton
    private static GameData _instance;
    public static GameData instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = ScriptableObject.CreateInstance<GameData>();
            }
            return _instance;
        }
    }

    private static string path = "DUMMY";

    private static string VeggieStr(int levelIndex)
    {
        return "Level" + levelIndex + "VeggieCount";
    }

    private static string BestVeggieStr(int levelIndex)
    {
        return "Level" + levelIndex + "BestVeggies";
    }

    private static string FewestRotationStr(int levelIndex)
    {
        return "Level" + levelIndex + "FewestRotations";
    }

    private static string UnlockedLevelStr = "UnlockedLevel";

    void Awake()
    {
        if (!PlayerPrefs.HasKey(UnlockedLevelStr))
        {
            PlayerPrefs.SetInt(UnlockedLevelStr, UNLOCKED_LEVELS_DEFAULT);
        }
    }

    public static  void WritePrefs(GameDataSkeleton gameDataSkeleton, string dummy)
    {
        /*
         * 1. set the unlocked level
         * FOR EACH LEVEL
         * 1. get the level index
         * 2. get the other string values
         * 3. write each string value to the database
         */

        PlayerPrefs.SetInt(UnlockedLevelStr, gameDataSkeleton.unlockedLevel);
        foreach (GameDataSkeleton.LevelDataSkeleton level in gameDataSkeleton.levelData)
        {
            PlayerPrefs.SetInt(VeggieStr(level.levelIndex), level.veggieCount);
            PlayerPrefs.SetInt(BestVeggieStr(level.levelIndex), level.yourBestVeggies);
            PlayerPrefs.SetInt(FewestRotationStr(level.levelIndex), level.fewestRotations);
        }
    }

    public static GameDataSkeleton ReadXML(string dummy)
    {
        GameDataSkeleton gameData = new GameDataSkeleton();
        if (!PlayerPrefs.HasKey(UnlockedLevelStr))
        {
            return gameData;
        }
        else
        {
            int unlockedLevel = PlayerPrefs.GetInt(UnlockedLevelStr);
            gameData.unlockedLevel = unlockedLevel;
            for (int i = 1; i <= unlockedLevel; i++)
            {
                GameDataSkeleton.LevelDataSkeleton levelSkelly = new GameDataSkeleton.LevelDataSkeleton();
                levelSkelly.levelIndex = i;
                levelSkelly.veggieCount = PlayerPrefs.GetInt(VeggieStr(i), -1);
                levelSkelly.yourBestVeggies = PlayerPrefs.GetInt(BestVeggieStr(i), -1);
                levelSkelly.fewestRotations = PlayerPrefs.GetInt(FewestRotationStr(i), -1);
                if (-1 == levelSkelly.veggieCount ||
                    -1 == levelSkelly.yourBestVeggies ||
                    -1 == levelSkelly.fewestRotations)
                    continue;
                gameData.levelData.Add(levelSkelly);
            }
            return gameData;
        }
    }

    public bool TryGetLevel(int levelIndex, out GameDataSkeleton.LevelDataSkeleton outputLevel)
    {
        GameDataSkeleton skelly = ReadXML(path);
        foreach (GameDataSkeleton.LevelDataSkeleton levelSkelly in skelly.levelData)
        {
            if (levelSkelly.levelIndex == levelIndex)
            {
                outputLevel = levelSkelly;
                return true;
            }
        }
        outputLevel = new GameDataSkeleton.LevelDataSkeleton();
        return false;
    }

	public bool setLevel(GameDataSkeleton skelly, int levelIndex, GameDataSkeleton.LevelDataSkeleton changedSkelly) {
		GameDataSkeleton.LevelDataSkeleton levelSkelly = new GameDataSkeleton.LevelDataSkeleton();
		foreach (GameDataSkeleton.LevelDataSkeleton l in skelly.levelData) {
			if (l.levelIndex == levelIndex) {
				levelSkelly = l;
				break;
			}
		}
		skelly.levelData.Remove(levelSkelly);
		skelly.levelData.Add(changedSkelly);
		return true;
	}

    public void ChangeBestVeggies(int levelIndex, int veggieAmount)
    {
        GameDataSkeleton skelly = ReadXML(path);
        GameDataSkeleton.LevelDataSkeleton levelSkelly;
        if (TryGetLevel(levelIndex, out levelSkelly))
        {
            levelSkelly.yourBestVeggies = Math.Max(veggieAmount, levelSkelly.yourBestVeggies);
			setLevel(skelly, levelIndex, levelSkelly);
            WritePrefs(skelly, path);
        }
        else
        {
            Debug.LogWarning(String.Format("Level index {0} not in GameData file!", levelIndex));
        }
    }

    public void ChangeBestRotations(int levelIndex, int rotationsUsed)
    {
        GameDataSkeleton skelly = ReadXML(path);
        GameDataSkeleton.LevelDataSkeleton levelSkelly;
        if (TryGetLevel(levelIndex, out levelSkelly))
        {
            levelSkelly.fewestRotations = Math.Min(rotationsUsed, levelSkelly.fewestRotations);
			setLevel(skelly, levelIndex, levelSkelly);
            WritePrefs(skelly, path);
        }
        else
        {
            Debug.LogWarning(String.Format("Level index {0} not in GameData file!", levelIndex));
        }
    }

    public void ChangeUnlockedLevel(int unlockedLevelIndex)
    {
        GameDataSkeleton skelly = ReadXML(path);
        skelly.unlockedLevel = Math.Max(skelly.unlockedLevel, unlockedLevelIndex + 1);
        WritePrefs(skelly, path);
    }

    public void AddLevel(int levelIndex, int veggieCount)
    {
        GameDataSkeleton skelly = ReadXML(path);
        GameDataSkeleton.LevelDataSkeleton levelSkelly;
        if (!TryGetLevel(levelIndex, out levelSkelly))
        {
            GameDataSkeleton.LevelDataSkeleton blankLevel = new GameDataSkeleton.LevelDataSkeleton();
            blankLevel.veggieCount = veggieCount;
            blankLevel.levelIndex = levelIndex;
            blankLevel.yourBestVeggies = 0;
            blankLevel.fewestRotations = int.MaxValue;
            skelly.levelData.Add(blankLevel);
            WritePrefs(skelly, path);
        }
    }

    public int GetUnlockedLevel()
    {
        GameDataSkeleton skelly = ReadXML(path);
        return skelly.unlockedLevel;
    }
}
