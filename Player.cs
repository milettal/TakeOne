using System;
using System.Collections.Generic;
using System.Text;

namespace TakeOne {

	///<summary>A class that represents a single player.</summary>
	public class Player {
		///<summary>The name of the player.</summary>
		public string Name;
		///<summary>The score of the player for all games.</summary>
		public int ScoreTotal;
		///<summary>The score that is subtracted from all the words. This is made up of letters that were left over.</summary>
		public int ScoreSubtracted;
		///<summary>A list of words with their score for the current game.</summary>
		public List<Tuple<string,int>> ListWordsCur=new List<Tuple<string,int>>();
	}

}
