using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// RowScript represents a row in the game scene.
/// It is responsible to create all items in a row and manage one row.
/// </summary>
public class RowScript : MonoBehaviour {
	
	// loaded in Inspector / Unity Editor.
	public GameObject itemHolder;
	
	private GameObject m_cover;
	private GameObject m_cloudParticle;
	private List<GameObject> m_items;
	private List<GameObject> m_clouds;
	private string m_themeName;
	private Vector3 m_missingPosition;
	private string m_missingElement;
	private AudioClip m_coverSfx;
	private AudioClip m_uncoverSfx;
	private AudioSource m_audioSource;

	/// <summary>
	/// Function called once for each row (5 in total), to create row items, select theme, load textures etc.
	/// </summary>
	public void CreateItems(string themeName, int z, Transform parent) {
	     
	    AddSounds();
	    SetThemeName(themeName);
		SetCover(z);
		SetParent(parent);
		CreateCloudParticle();
		List<string> themeItemList = GrabThemeItemList(themeName);
		
		// create items list.
		m_items = new List<GameObject>();
		
		int numberOfItems = CalculateNumberOfItems();
		int startPosition = CalculateStartingPosition(); // starting position of items.
		
		// put items in random positions.
		List<int> positionsInRow = new List<int>();
		// list of numbers in a row.
		for (int i = startPosition; i < numberOfItems + startPosition; i++)
			positionsInRow.Add(i);
		
		// variables for iteration.
		int arrLength;
		int select;
		int num_copies = 0; // by default there are no copies = all items different.
		string itemName;
		
		// Only levels 1,2,4,5 have items that are the same. (not 3, 6, 7, 8, 9)
		if (Game.Mode != Game.Difficulty.Hard 
			&& Global.currentLevel != 3
		    && Global.currentLevel != 6) {
			
			// Select an item to copy (used in the item creation loop below).
			arrLength = themeItemList.Count;
			select = Random.Range(0, arrLength); // select one of the items
			itemName = themeItemList[select]; // pick it up.
			themeItemList.RemoveAt(select); // remove it from array.
			
			// In Easy / Medium game difficulty.
			if (Game.SuperMode != Game.Difficulty.Hard) {
				// Level 1 and 4 have all same items (all copies).
				if (Global.currentLevel == 1 || Global.currentLevel == 4) {
					num_copies = positionsInRow.Count;
				}
				// Level 2 and 5 have some different items (variability).
				else {
					num_copies = Random.Range(2, 4); // either 2 or 3 items.
				}
			}
			else {
				if (Global.currentLevel < 4) 
					num_copies = 0;
				else if(Global.currentLevel < 7) 
					num_copies = 1;
				else 
					num_copies = 2;
			}
		}
		
		// load prefab to hold the item.
		itemHolder = (GameObject) Resources.Load(Constants.ROW_ITEM_NAME);
		
		// Finally create all the items.
		for (int j = 0; j < numberOfItems; j++) {
			
			// it needs to be a copy of the previous item.
			if (num_copies > 0) {
				num_copies--;
			}
			// otherwise pick new random item.
			else {
				arrLength = themeItemList.Count; // get number of elements
				select = Random.Range(0, arrLength); // select one of the items
				
				itemName = themeItemList[select]; // pick it up.
				themeItemList.RemoveAt(select); // remove it from array.
			}
			
			// POSITIONING.
			arrLength = positionsInRow.Count; // get number of elements.
			select = Random.Range(0, arrLength); // select one of the x positions.
			int x = positionsInRow[select]; // set x;
			positionsInRow.RemoveAt(select); // remove it from array.
			
			// the last element is the item missing - it is used by SelectItemsManager.
			if (j == numberOfItems - 1) {
				SetMissingItem(itemName, x, z);
			}
			// all other items need to be created.
			else {
				CreateRowItem(itemName, x, z);
			}
		}
	}
	
	private void SetMissingItem(string itemName, float x, float z) {
		
		m_missingElement = itemName;
		m_missingPosition = new Vector3(-4.25f + x * 2.125f, Constants.SelectItemUpYPos, 2.25f * -z);
	}
	
	private void CreateRowItem(string itemName, float x, float z) {
		
		GameObject item = (GameObject) Instantiate(itemHolder);
		
		// set name
		item.name = itemName;
		
		// setup texture!
		SetUpTexture(item.renderer.material, "Pictures/" + m_themeName + "/" + itemName);
		
		// set parent.
		item.transform.parent = gameObject.transform;
		
		m_items.Add(item);
		
		AddCloud();
	}
	
	private void AddSounds() {
		// Add AudioSource.
		m_audioSource = gameObject.AddComponent<AudioSource>();
		m_coverSfx = GameObject.Find(Constants.PARENT_OBJECT_NAME).GetComponent<Game>().CoverSound;
		m_uncoverSfx = GameObject.Find(Constants.PARENT_OBJECT_NAME).GetComponent<Game>().UncoverSound;
	}
	
	private void SetThemeName(string themeName) {
		
		m_themeName = themeName;
	}
	
	private void SetCover(int z) {
		
		// retrieve the cover of this row from the scene.
		m_cover = GameObject.Find(Constants.COVER_NAME + (z + 1).ToString());
	}
	
	private void SetParent(Transform parent) {
		
		// set parent.
		this.transform.parent = parent;
	}
	
	private List<string> GrabThemeItemList(string themeName) {
		
		// get a reference of pictureLoader
		PictureLoader pictureLoader = GameObject.Find(Constants.PICTURE_LOADER_NAME).GetComponent<PictureLoader>();
		
		// select random theme -> returns a list of names for the pictures in that theme.
		return pictureLoader.GetTheme(themeName);
	}
	
	private void CreateCloudParticle() {
		
		m_cloudParticle = (GameObject) Resources.Load(Constants.CLOUD_PARTICLE_NAME);
		m_clouds = new List<GameObject>();
	}
	
	/// <summary>
	/// Calculate the number of items in the row.
	/// </summary>
	private void CalculateNumberOfItems() {
		
		int numberOfItems = 0;
		
		if (Game.Mode == Game.Difficulty.Easy) {
			// all 5 slots occupied.
			numberOfItems = 5;
		}
		else if (Game.Mode == Game.Difficulty.Medium) {
			// 4 slots occupied.
			numberOfItems = 4;
		}
		else {
			// 3 slots occupied.
			numberOfItems = 3;
		}
		
		return numberOfItems;
	}
	
	/// <summary>
	/// Calculate the starting position of the items in the row.
	/// </summary>
	private void CalculateStartingPosition() {
		
		int startingPosition = 0;
		
		if (Game.Mode == Game.Difficulty.Easy) {
			// all 5 slots occupied so start at the first one (0).
			startingPosition = 0;
		}
		else if (Game.Mode == Game.Difficulty.Medium) {
			// 4 slots occupied, the first one is not used (so start at 1).
			startingPosition = 1;
		}
		else {
			// 3 slots occupied, 2 first ones are not used (so start at 2).
			startingPosition = 2;
		}
		
		return startingPosition;
	}
	
	/// <summary>
	/// Load and scale the texture to fit the cube (not stretch!).
	/// </summary>
	public void SetUpTexture(Material itemMaterial, string textureName) {
		
		int width = 0;
		int height = 0;
		Texture2D itemTexture = (Texture2D) Resources.Load(textureName);
		
		// get texture width and height.
		GetImageSize(itemTexture, out width, out height);
		
		// scaleX, offsetX
		if (width > height) {
			float scaleX = 1 / ((float) width / (float) height);
			
			itemMaterial.mainTextureScale = new Vector2(scaleX, 1f);
		}
		// scaleY, offsetY
		else {
			float scaleY = 1 / ((float) height / (float) width);
			
			itemMaterial.mainTextureScale = new Vector2(1f, scaleY);
		}
		
		itemMaterial.mainTexture = itemTexture;
	}
	
	/// <summary>
	/// Grab the width and height of the texture.
	/// Returns false if the operation was unsuccessful.
	/// </summary>
	public bool GetImageSize(Texture2D asset, out int width, out int height) {
		
		float t_width = 1f, t_height = 1f;
		
		if (asset != null) {
			string assetPath = AssetDatabase.GetAssetPath(asset);
			TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			
			if (importer != null) {
				object[] args = new object[2] { 0, 0 };
				MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", 
									BindingFlags.NonPublic | BindingFlags.Instance);
				mi.Invoke(importer, args);
				
				width = (int)args[0];
				height = (int)args[1];
				
				return true;
			}
		}
		
		height = width = 0;
		
		return false;
	}
	
	private void AddCloud() {
		
		// Add cloud on top.
		GameObject cloud = (GameObject) Instantiate(m_cloudParticle, 
				new Vector3(-4f + x * 2.125f, Constants.CLOUDS_LOWER_Y, 2.25f * -z - 0.25f), 
				Quaternion.identity);
							
		cloud.transform.parent = transform;
		cloud.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
		m_clouds.Add( cloud );
	}
	
	public void ShowClouds() {
		
		StartCoroutine(ShowCloudsAfterWait());
	}
	
	private IEnumerator ShowCloudsAfterWait() {
		
		yield return new WaitForSeconds(Constants.WAIT_BEFORE_CLOUDS);
		
		float time = 0;
		
		for (int i = 0; i < m_clouds.Count; i++) {
			
			time = GetRandomNumber(Constants.CLOUD_TIME - Constants.CLOUD_TIME_RANGE, 
									Constants.CLOUD_TIME + Constants.CLOUD_TIME_RANGE);
			
			m_clouds[i].GetComponent<ParticleSystem>().Play();
			Vector3 currPos = m_clouds[i].transform.position;
			
			// move clouds up to hide the items.
			iTween.MoveTo(m_clouds[i], new Vector3(currPos.x, Constants.CLOUDS_HIGHER_Y, currPos.z), time);
		}
	}
	
	public void DestroyClouds() {
		
		float time = 0;
		
		for (int i = 0; i < m_clouds.Count; i++) {
			
			time = GetRandomNumber(Constants.CLOUD_TIME - Constants.CLOUD_TIME_RANGE, 
									Constants.CLOUD_TIME + Constants.CLOUD_TIME_RANGE);
			
			Vector3 currPos = m_clouds[i].transform.position;
			iTween.MoveTo(m_clouds[i], iTween.Hash("y", Constants.CLOUDS_LOWER_Y, 
					"easeType", "easeInOutExpo", 
					"time", time));
			
			StartCoroutine(DestroyCloud(m_clouds[i]));
		}
	}
	
	private IEnumerator DestroyCloud(GameObject cloud) {
		
		yield return new WaitForSeconds(Constants.CLOUD_TIME);
		
		Destroy(cloud);
	}
	
	public float GetRandomNumber(float minimum, float maximum)
	{ 
		return Random.value * (maximum - minimum) + minimum;
	}
	
	public void Cover() {
		
		m_audioSource.PlayOneShot(m_coverSfx);
		
		iTween.MoveBy(m_cover, new Vector3(Constants.COVER_MOVE_BY[(int)Game.Mode], 0, 0),
						Constants.CoverMoveTime);
	}
	
	public void UnCover() {
		
		m_audioSource.PlayOneShot(m_uncoverSfx);
		
		iTween.MoveBy(m_cover, new Vector3(-Constants.COVER_MOVE_BY[(int)Game.Mode], 0, 0),
						Constants.CoverMoveTime);
	}
	
	public string GetMissingElement() {
	
		return m_missingElement;
	}
	
	public string GetThemeName() {
	
		return m_themeName;
	}
	
	public Vector3 GetMissingPosition() {
	
		return m_missingPosition;
	}
	
	public void SelectMissingItem() {
	
		GameObject.Destroy (GameObject.Find (m_missingElement));
		m_items.Remove (GameObject.Find (m_missingElement));
	}
}
