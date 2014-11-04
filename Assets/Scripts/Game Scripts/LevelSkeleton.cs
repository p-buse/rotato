//using UnityEngine;
//using System.Collections;

//[Serializable]
//public class LevelSkeleton : MonoBehaviour
//{

//    public List<BlockSkeleton> blocks;

//    //for each item in list: item[0] is x position, item[1] is y position:
//    public List<int[]> noRoZones; 

//    public int[] player;
//    public int[][] crawlers;

//    //stores block grid
//    public void setGrid(Dictionary<Int2, AbstractBlock> grid){

//        blocks = new List<string> ();

//        foreach (var block in grid.Values) {
//            blocks.Add(block.serizalize());
//        }
	
//    }

//    //stores norozone grid
//    public void setNoRoZoneGrid(HashSet<Int2> grid){

//        foreach (var zone in grid) {	
//            int[] pos;
//            pos[0] = zone.x;
//            pos[1] = zone.y;

//            noRoZones.Add(s);
//        }

//    }
	
//}
