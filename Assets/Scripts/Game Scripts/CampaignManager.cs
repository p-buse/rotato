using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class CampaignManager : MonoBehaviour {

    string campaignsPath;
    string[] campaignNames;
    Dictionary<string, List<string>> campaigns;

    public string[] GetCampaigns()
    {
        return this.campaignNames;
    }

    public List<string> GetLevelsInCampaign(string campaignName)
    {
        if (campaigns.ContainsKey(campaignName))
        { return campaigns[campaignName]; }
        else
        {
            Debug.LogWarning("campaign " + campaignName + " doesn't exist!");
            return null;
        }
    }


	// Use this for initialization
	void Awake ()
    {
        this.campaigns = new Dictionary<string, List<string>>();
        this.campaignsPath = Application.dataPath + "/CampaignLevels/";
        if (!Directory.Exists(campaignsPath))
        {
            Directory.CreateDirectory(campaignsPath);
        }
        this.campaignNames = Directory.GetDirectories(campaignsPath);
        foreach (string campaignName in campaignNames)
        {
            if (!campaigns.ContainsKey(campaignName))
                campaigns.Add(campaignName, new List<string>());
            string[] levelNames = Directory.GetFiles(campaignName, "*.xml");
            foreach (string levelName in levelNames)
            {
                campaigns[campaignName].Add(levelName);
            }
        }
	}
}
