using UnityEngine;
using System.Collections.Generic;
using System;

public class NoRotationManager : MonoBehaviour {

	private HashSet<Int2> noRotationZones; 

	void Awake(){
		noRotationZones = new HashSet<Int2>();

		NoRotationZone[] zones = GameObject.FindObjectsOfType<NoRotationZone>();

		foreach (NoRotationZone z in zones){
		
			Int2 zonePosition = new Int2(z.transform.position.x, z.transform.position.y);
			noRotationZones.Add(zonePosition);
		}
	}

	public bool hasNoRotationZone(Int2 position){
	
		if(noRotationZones.Contains(position)){
			return true;
		}

		return false;
	}
}