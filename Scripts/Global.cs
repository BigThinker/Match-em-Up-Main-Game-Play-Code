using UnityEngine;
using System.Collections;

/// <summary>
/// Class contains global variables and methods for defining the narrative of the game.
/// </summary>
public class Global : MonoBehaviour {

	/// <summary>
	/// Flag to allow / disable mouse input.
	/// </summary>
	public static bool lockMouse = false;
	
	//// <summary>
	/// Reference to the current level (ranges from 1 to 9).
	/// </summary>
	public static int currentLevel = 1;
	
	/// <summary>
	/// All possible states of the game:
	/// * Menu
	/// * Playing
	/// * ShowImg - Show Reel with the solved image after a row is solved.
	/// * ShowReel - Show Reel with all images.
	/// * Timer - Timed events (i.e. Hard difficulty).
	/// </summary>
	public enum State
	{
		Menu,
		Playing,
		ShowImg,
		ShowReel,
		Timer
	}
	
	/// <summary>
	/// Reference to current state.
	/// </summary>
	public static State state;
    
	/// <summary>
    /// Flag to indicate whether Kinect Input is used.
	/// </summary>
    public static bool useKinect = true;
    
	/// <summary>
	/// Flag to indicate whether the selected items should move automatically
	/// with a click or manually with a click - hold - move - release.
	/// </summary>
    public static bool playerSnapMode = false;

	/// <summary>
	/// Use function to prepare next Game Mode and next Level Mode.
	/// </summary>
	public static void PrepareNextLevel() {
		
		// increment level.
		currentLevel++;
		
		// next game difficulty (supermode).
		if (currentLevel > 9) {
			// reset level to 1 for next difficulty.
			if(Game.SuperMode == Game.Difficulty.Hard)
				currentLevel = 1;
			
			// from easy to medium.
			if (Game.SuperMode == Game.Difficulty.Easy) 
				Game.SuperMode = Game.Difficulty.Medium;
			// from medium to hard.
			else if (Game.SuperMode == Game.Difficulty.Medium) 
				Game.SuperMode = Game.Difficulty.Hard;
		}
		
		// next level difficulty (mode).
		if (Game.SuperMode != Game.Difficulty.Hard) {
			if (currentLevel <= 3)
				Game.Mode = Game.Difficulty.Easy;
			else if (currentLevel <= 6)
				Game.Mode = Game.Difficulty.Medium;
			else
				Game.Mode = Game.Difficulty.Hard;
		} 
		// in the case where the difficulty is hard, the order of level difficulty is reversed.
		// it's easier to remember 1 of 3 items than 1 of 5 items.
		else {
			if (currentLevel <= 3)
				Game.Mode = Game.Difficulty.Hard;
			else if (currentLevel <= 6)
				Game.Mode = Game.Difficulty.Medium;
			else
				Game.Mode = Game.Difficulty.Easy;
		}
	}
	
	/// <summary>
	/// Function returns the variability of the items based on the current level,
	/// in percentage. 0 -> all items are the same. 1 -> all items are different.
	/// </summary>
	public static float GetVariability() {
		
		// Narrative (Easy): Levels 1 to 3 -> 5 are shown / same (1), variable (2), different (3).
		// 4 to 6 -> 4 are shown / same (4), variable (5), different (6).
		// 7 to 9 -> 3 are shown / all different (7, 8, 9).
		
		// Narrative (Medium): Same as Easy with the exception that there are clouds
		// hiding the items after a while.
		
		// Narrative (Hard): Levels 1 to 3 -> 3 are shown / all same (1, 2, 3).
		// 4 to 6 -> 4 are shown / all variable (70% different) (4, 5, 6).
		// 7 to 9 -> 5 are shown / all different (7, 8, 9).
		
		float itemsVariability = 0;
		
		// If it's easy or medium.
		// for 1, 4 it's 0%
		// for 2, 5 it's 70%
		// for 3, 6, 7, 8, 9 it's 100%
		if (Game.SuperMode != Game.Difficulty.Hard) {
			if (currentLevel == 1 || currentLevel == 4)
				itemsVariability = 0;
			else if (currentLevel == 2 || currentLevel == 5)
				itemsVariability = Constants.ITEMS_VARIABILTY;
			else
				itemsVariability = 1;
		}
		// If it's hard  
		else {
			if (currentLevel < 4) 
				itemsVariability = 0f;
			else if (currentLevel < 7) 
				itemsVariability = Constants.ITEMS_VARIABILTY;
			else 
				itemsVariability = 1;
		}
		
		return itemsVariability;
	}
}
