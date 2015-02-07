using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Static class to keep the constants of the project.
/// </summary>
public class Constants : MonoBehaviour {

	/// <summary>
	/// Time it takes to cover one row.
	/// </summary>  
	public const float COVER_MOVE_TIME = 1.5f;
	/// <summary>
	/// Raise selected item to this y position.
	/// </summary> 
	public static float SELECT_ITEM_HIGHER_Y = 4.5f;
	/// <summary>
	/// The chance in percentage that the items are different.
	/// </summary>
	public const float ITEMS_VARIABILTY = 0.7f;
	/// <summary>
	/// Indicates how much time to wait for unclocking the mouse (enable).
	/// </summary> 
	public const float WAIT_BEFORE_UNCLOCK_MOUSE = 2f;
	/// <summary>
	/// List with values to move the covers (x direction). [easy, medium, hard] level difficulty.
	/// </summary>
	public const List<float> COVER_MOVE_BY = new List<float>() { 10.765f, 8.6f, 6.47f };
	/// <summary>
	/// List with the names of the prefabs of the frames for the show reel.
	/// </summary>
	public const List<string> SHOW_REEL_FRAME_NAMES = new List<string>() { "frame01", "frame02", "frame03", "frame04", "frame05" };
	/// <summary>
	/// The name of the game object which contains all other gameobjects with main gameplay scripts.
	/// </summary>
	public const string PARENT_OBJECT_NAME = "Objects";
	
	public const string COVER_NAME = "cover";
	public const string SELECT_ITEMS_COVER = "right_cover";
	public const string ROW_NAME = "Row";
	public const string ROW_ITEM_NAME = "RowItem";
	public const string LOGO_NAME = "Logo";
	public const string ROW_MANAGER_NAME = "Rows Manager";
	public const string SELECT_ITEMS_MANAGER_NAME = "SelectItems Manager";
	public const string SHOW_REEL_NAME = "Show Reel";
	public const string KINECT_MOUSE_INPUT_NAME = "Kinect Mouse Input";
	public const string PICTURE_LOADER_NAME = "Picture Loader";
	public const string CLOUD_PARTICLE_NAME = "Cloud";
	public const string CAMERA_NAME = "Main Camera";
	public const string FRAME_PLANE_NAME = "framePlane";
	public const float SECONDS_BEFORE_INIT_SCRIPTS = 0.5f; // safety initialization wait.
	public const float CLOUD_TIME = 5f;
	public const float CLOUD_TIME_RANGE = 1.5f; // seconds to vary the cloud time to show the clouds.
	public const float WAIT_BEFORE_CLOUDS = 6f;
	public const int NUMBER_OF_ROWS = 5;
	public const int NUMBER_OF_SELECTABLE_ITEMS = 4;
	public const float CLOUDS_LOWER_Y = -1.8f;
	public const float CLOUDS_HIGHER_Y = 1.4f;
	public static float INBETWEEN_COVERS_TIME = -0.5f;
	public static float SELECT_ITEM_MOVE_TIME = 1.5f;
	public static float SHOWREEL_ITEM_XOFFSET = 18f;
	public static float SHOWREEL_MOVE_TIME = 1f;
	public const float ITEM_GROUND_LEVEL = 2f; 
	public const float SELECTITEMS_COVER_MOVEBY = 2.11f;
}
