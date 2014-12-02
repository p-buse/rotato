using UnityEngine;
using System.Collections.Generic;
using System;

public class NoRotationManager : MonoBehaviour {

	public Dictionary<Int2, NoRotationZone> noRotationZones; 

	void Awake(){
        noRotationZones = new Dictionary<Int2, NoRotationZone>();

		NoRotationZone[] zones = GameObject.FindObjectsOfType<NoRotationZone>();

		foreach (NoRotationZone z in zones){
		
			Int2 zonePosition = new Int2(z.transform.position.x, z.transform.position.y);
			noRotationZones.Add(zonePosition, z);
		}
	}

    public bool AddNoRoZone(Int2 pos, GameObject noRoPrefab)
    {
        if (!noRotationZones.ContainsKey(pos))
        {
            
            GameObject newNoRoZone = Instantiate(noRoPrefab, pos.ToVector2(), Quaternion.identity) as GameObject;
            NoRotationZone nrzComponent = newNoRoZone.GetComponent<NoRotationZone>();
            if (nrzComponent == null)
            {
                Debug.LogError("NoRoZone gameobject at " + pos + " couldn't find norozone component!");
            }
            noRotationZones.Add(pos, nrzComponent);
            return true;
        }
        else // There's already a noRoZone at position pos
        {
            return false;
        }
        
    }

    public bool RemoveNoRoZone(Int2 position)
    {
        NoRotationZone nrz;
        if (noRotationZones.TryGetValue(position, out nrz))
        {
            Destroy(nrz.gameObject);
            noRotationZones.Remove(position);
            return true;
        }
        // There's not a noRoZone at the position we want to remove from
        return false;
    }

	public bool hasNoRotationZone(Int2 position){
        return noRotationZones.ContainsKey(position);
	}

    public void ClearNoRotationZones()
    {
        foreach (Int2 noRoPos in this.noRotationZones.Keys)
        {
            if (noRotationZones[noRoPos] != null)
            {
                Destroy(noRotationZones[noRoPos].gameObject);
            }
        }
        this.noRotationZones = new Dictionary<Int2, NoRotationZone>();
    }
}