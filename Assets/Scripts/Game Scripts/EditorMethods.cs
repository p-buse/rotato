using UnityEngine;
using System.Collections;

public class EditorMethods : MonoBehaviour
{

	GameManager gameManager;
	BlockManager blockManager;
	public enum BlockType{regular, cracked, falling, cannon, fix, mirror, laser}
	

	/// <summary>
	/// Creates a new  prefab of the type passed, at the given position.
	/// if it's a block, it adds itself to the BlockManager
	/// </summary>
	/// <param name="blockType">Block type.</param>
	/// <param name="pos">Position.</param>
	private void addPrefab(GameObject prefab, Int2 pos)
	{
		GameObject newObject = (GameObject)Instantiate (prefab);

		newObject.transform.position = new Vector3 (pos.x, pos.y, 0);

		AbstractBlock blockScript = newObject.GetComponent<AbstractBlock> ();
		if(blockScript!=null)
		{	
			blockManager.grid.Add(pos, blockScript);
		}
					
	}

	/// <summary>
	/// safely destroys the object in question
	/// </summary>
	/// <param name="prefab">Prefab.</param>
	private void removeObject(GameObject ob)
	{
		AbstractBlock blockScript = ob.GetComponent<AbstractBlock> ();
		CrawlerSegment crawler = ob.GetComponent<CrawlerSegment> ();
		if(blockScript!=null)
		{
			blockManager.grid.Remove(blockScript.GetCurrentPosition());
			Destroy(ob);
		}
		else if(crawler!=null)
		{
			crawler.dieSafely();
		}
		else{
			Destroy (ob);
		}

	}


}

