using System;
using System.Collections.Generic;
using System.Linq;

namespace TakeOne {
	public class Program {

		///<summary>A dictionary that contains all of the supported characters and their given score.</summary>
		private static Dictionary<char,int> _dictLetterScore=new Dictionary<char,int> { { 'a', 1 }, { 'c', 3 }, { 'b', 3 }, { 'e', 1 },
			{ 'd', 2 }, { 'g', 2 }, { 'f', 4 }, { 'i', 1 }, { 'h', 4 }, { 'k', 5 }, { 'j', 8 }, { 'm', 3 }, { 'l', 1 },
			{ 'o', 1 }, { 'n', 1 }, { 'q', 10 }, { 'p', 3 }, { 's', 1 }, { 'r', 1 }, { 'u', 1 }, { 't', 1 }, { 'w', 4 },
			{ 'v', 4 }, { 'y', 4 }, { 'x', 8 }, { 'z', 10 }, {'?',0 } };

		public static void Main(string[] args) {
			Console.WriteLine("Welcome to Take One!");
			//Get the number of players.
			int numberOfPlayers=GetNumberOfPlayers();
			List<Player> listPlayers=GetListPlayers(numberOfPlayers);
			Console.Clear();
			//Game loop.
			while(true) {
				foreach(Player player in listPlayers) {
					GetScoreForRound(player);
					GetSubtractedScoreForRound(player);
					ShowScoreForRound(player);
				}
				ShowSummaryForRound(listPlayers);
				ClearPlayersRoundWords(listPlayers);
			}
		}

		///<summary>Gets the leftover tiles and adds it to the player's ScoreSubtracted.</summary>
		private static void GetSubtractedScoreForRound(Player player) {
			Console.Clear();
			string subtractedLetters="";
			do {
				Console.Write("Enter leftover letters: ");
				subtractedLetters=Console.ReadLine();
			} while(!string.IsNullOrWhiteSpace(subtractedLetters) && WordContainsErrors(subtractedLetters));
			player.ScoreSubtracted=GetScoreForWord(subtractedLetters);
		}

		///<summary>Clears all of the players words for the given round.</summary>
		private static void ClearPlayersRoundWords(List<Player> listPlayers) {
			foreach(Player player in listPlayers) {
				player.ListWordsCur=new List<Tuple<string,int>>();
				player.ScoreSubtracted=0;
			}
		}

		///<summary>Shows the summary for all players for the given round.</summary>
		private static void ShowSummaryForRound(List<Player> listPlayers) {
			Console.Clear();
			Console.WriteLine("Score for Current Round: ");
			foreach(Player player in listPlayers) {
				int scoreRound=player.ListWordsCur.Sum(x => x.Item2)-player.ScoreSubtracted;
				player.ScoreTotal+=scoreRound;
				Console.WriteLine($"{player.Name} - {scoreRound}");
			}
			Console.WriteLine("\nTotal Score:");
			foreach(Player player in listPlayers) {
				Console.WriteLine($"{player.Name} - {player.ScoreTotal}");
			}
			ConsoleKeyInfo _ = Console.ReadKey();
		}

		///<summary>Shows the score for the given player for the round. Gives them the option to look at their words.</summary>
		private static void ShowScoreForRound(Player player) {
			while(true) {
				Console.Clear();
				int totalScore=player.ListWordsCur.Sum(x => x.Item2)-player.ScoreSubtracted;
				Console.WriteLine($"Round score for {player.Name}: {totalScore}");
				Console.WriteLine($"Highest scoring word: {player.ListWordsCur.First(x => x.Item2==player.ListWordsCur.Max(y => y.Item2)).Item1} "+
					$"- {player.ListWordsCur.Max(x => x.Item2)}\n");
				Console.WriteLine($"1. Print Words - Alphabetically");
				Console.WriteLine($"2. Print Words - Score");
				Console.WriteLine($"3. Print Words - Length");
				Console.WriteLine($"4. Continue");
				Console.Write("\nEnter Option: ");
				ConsoleKeyInfo input=Console.ReadKey();
				if(input.KeyChar!='1' && input.KeyChar!='2' && input.KeyChar!='3' && input.KeyChar!='4') {
					//Repeat as they did not enter the correct thing.
					continue;
				}
				int inputInt=int.Parse(input.KeyChar.ToString());
				void PrintWords(List<Tuple<string,int>> listWords) {
					Console.WriteLine();
					Console.WriteLine();
					foreach(Tuple<string,int> word in listWords) {
						Console.WriteLine($"{word.Item1} - {word.Item2}");
					}
					ConsoleKeyInfo _=Console.ReadKey();
				}
				if(inputInt==1) {
					List<Tuple<string,int>> listAlphabetical=player.ListWordsCur.OrderBy(x => x.Item1).ToList();
					PrintWords(listAlphabetical);
				}
				else if(inputInt==2) {
					List<Tuple<string,int>> listScore=player.ListWordsCur.OrderByDescending(x => x.Item2).ToList();
					PrintWords(listScore);
				}
				else if(inputInt==3) {
					List<Tuple<string,int>> listScore=player.ListWordsCur.OrderByDescending(x => x.Item1.Length).ToList();
					PrintWords(listScore);
				}
				else if(inputInt==4) {
					break;
				}
			}
		}

		///<summary>Gets all the words and the score for the given player.</summary>
		private static void GetScoreForRound(Player player) {
			Console.Clear();
			Console.WriteLine($"Score for {player.Name}:");
			string endStr="END";
			string undoStr="UNDO";
			Console.WriteLine($"Enter {endStr} to end the scoring.\nEnter {undoStr} to undo the previous word.\n");
			while(true) {
				Console.Write("Enter word: ");
				string inputWord=Console.ReadLine();
				//If they want to end, break them out of the loop.
				if(inputWord==endStr) {
					if(player.ListWordsCur.Count==0) {
						Console.WriteLine("You cannot end without a single word.");
						continue;
					}
					break;
				}
				//They want to remove the last word. We need to make sure that they have at least one word to remove.
				if(inputWord==undoStr) {
					if(player.ListWordsCur.Count==0) {
						Console.WriteLine("There are no words to undo");
						continue;
					}
					//Otherwise, we should remove the last entry
					string removedWord=player.ListWordsCur.Last().Item1;
					player.ListWordsCur.RemoveAt(player.ListWordsCur.Count-1);
					Console.WriteLine($"Removed word: {removedWord}");
					continue;
				}
				//Now we can treat the word as lower case
				inputWord=inputWord.ToLower().Trim();
				if(WordContainsErrors(inputWord)) {
					Console.WriteLine("There was an error in the previous word. Please try again.");
					continue;
				}
				//Otherwise, they entered a real word with no errors.
				int score=GetScoreForWord(inputWord);
				Console.WriteLine(score);
				player.ListWordsCur.Add(Tuple.Create(inputWord,score));
			}
		}

		///<summary>Returns the score for the given word. The word should already be validated.</summary>
		private static int GetScoreForWord(string inputWord) {
			if(string.IsNullOrWhiteSpace(inputWord)) {
				return 0;
			}
			return inputWord.Sum(x => _dictLetterScore[x]);
		}

		///<summary>Indicates if the given word contains an error.</summary>
		private static bool WordContainsErrors(string inputWord) {
			if(string.IsNullOrWhiteSpace(inputWord)) {
				return true;
			}
			foreach(char car in inputWord) {
				if(!_dictLetterScore.ContainsKey(car)) {
					return true;
				}
			}
			return false;
		}

		///<summary>Returns a list of players after prompting the user for their names. numberOfPlayers should already be validated.</summary>
		private static List<Player> GetListPlayers(int numberOfPlayers) {
			List<Player> listPlayers=new List<Player>();
			for(int i=0;i<numberOfPlayers;i++) {
				Console.Write($"Enter player {i + 1}'s name: ");
				string input=Console.ReadLine();
				//If they didn't enter anything, don't let them continue;
				if(string.IsNullOrWhiteSpace(input)) {
					i--;
					continue;
				}
				//Otherwise, we got a name;
				listPlayers.Add(new Player { Name=input });
			}
			return listPlayers;
		}

		///<summary>Prompts the user and gets a valid number of players.</summary>
		private static int GetNumberOfPlayers() {
			int numberOfPlayers=0;
			while(numberOfPlayers<=0) {
				Console.Write("How many players are there? ");
				string input=Console.ReadLine();
				if(!int.TryParse(input,out int num)) {
					continue;
				}
				numberOfPlayers=num;
			}
			return numberOfPlayers;
		}
	}
	
}
