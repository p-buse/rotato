using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System;

public class GameData: ScriptableObject {

    private static GameData _instance;
    public static GameData instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameData();
            }
            return _instance;
        }
    }

    public static string fileName = "GameData.xml";
    public string path;

    void Awake()
    {
        this.path = Application.dataPath + "/" + fileName;
        if (!File.Exists(path))
        {
            WriteXML(new GameDataSkeleton(), path);
        }
    }

    public static void WriteXML(GameDataSkeleton gameDataSkeleton, string path)
    {
        XmlSerializer writer = new XmlSerializer(typeof(GameDataSkeleton));
        System.IO.StreamWriter file = new System.IO.StreamWriter(path);
        writer.Serialize(file, gameDataSkeleton);
        file.Close();
        Debug.Log("Wrote game data to " + path);
    }

    public static GameDataSkeleton ReadXML(string path)
    {
        XmlSerializer deserializer = new XmlSerializer(typeof(GameDataSkeleton));
        try
        {
            TextReader textReader = new StreamReader(path);
            GameDataSkeleton loadedLevel;
            loadedLevel = (GameDataSkeleton)deserializer.Deserialize(textReader);
            textReader.Close();
            return loadedLevel;
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("Couldn't read file from " + path);
            return null;
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
            WriteXML(skelly, path);
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
            WriteXML(skelly, path);
        }
        else
        {
            Debug.LogWarning(String.Format("Level index {0} not in GameData file!", levelIndex));
        }
    }

    public void ChangeUnlockedLevel(int unlockedLevelIndex)
    {
        GameDataSkeleton skelly = ReadXML(path);
        skelly.unlockedLevel = Math.Max(skelly.unlockedLevel, unlockedLevelIndex);
        WriteXML(skelly, path);
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
            WriteXML(skelly, path);
        }
    }

    public int GetUnlockedLevel()
    {
        GameDataSkeleton skelly = ReadXML(path);
        return skelly.unlockedLevel;
    }
}
