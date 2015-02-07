using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SelectItemsManager creates and manages all selectable items (4).
/// </summary>
public class SelectItemsManager : MonoBehaviour {

	private GameObject m_itemHolder;
	private GameObject m_cover;
	private RowScript m_currentRowScript;
	private Vector3 m_correctItemPos;
	private List<string> m_selectItemsNames;
	private ShowReelScript m_showReel;
	private RowsManager m_rowsManager;
	private List<GameObject> m_selectItems;
	private List<string> m_currentThemes;
	private AudioSource m_audioSource;
	private AudioClip m_coverSfx;
	private AudioClip m_uncoverSfx;
	private KinectGestureListener m_kinectGestureListener;
	private KinectManager m_kinectManager;
    
	void Start () {
		
		Init();
	}

	private void Init()
	{
		m_selectItems = new List<GameObject>();
		m_selectItemsNames = new List<string>();
		
		m_itemHolder = (GameObject) Resources.Load(Constants.ROW_ITEM_NAME);
		m_rowsManager = GameObject.Find(Constants.ROW_MANAGER_NAME).GetComponent<RowsManager>();
		m_cover = GameObject.Find(Constants.SELECT_ITEMS_COVER);
		m_showReel = GameObject.Find(Constants.SHOW_REEL_NAME).GetComponent<ShowReelScript>();
		
		m_kinectGestureListener = Camera.main.GetComponent<KinectGestureListener>();
		m_kinectManager = Camera.main.GetComponent<KinectManager>();

		AddSounds();
	}
	
	private void AddSounds() {
		
		m_audioSource = gameObject.AddComponent<AudioSource>();
		m_coverSfx = GameObject.Find(Constants.PARENT_OBJECT_NAME).GetComponent<Game>().CoverSound;
		m_uncoverSfx = GameObject.Find(Constants.PARENT_OBJECT_NAME).GetComponent<Game>().UncoverSound;
	}
	
	/// <summary>
	/// Move the cover to hide the items.
	/// </summary>
	public void CoverItems() {
		
		m_audioSource.PlayOneShot(m_coverSfx);
	
		Global.lockMouse = true;
		iTween.MoveBy(m_cover, new Vector3(-Constants.SELECTITEMS_COVER_MOVEBY, 0, 0), Constants.CoverMoveTime);
	}
	
	/// <summary>
	/// Move the cover to show the items.
	/// </summary>
	public void UnCoverItems() {
		
		m_audioSource.PlayOneShot(m_uncoverSfx);
	
		iTween.MoveBy(m_cover, new Vector3(Constants.SELECTITEMS_COVER_MOVEBY, 0, 0), Constants.CoverMoveTime);
		Global.lockMouse = false;
	}
	
	/// <summary>
	/// Sets up the manager and creates all the selectable items.
	/// Makes sure one of the items is the correct one to complete the row.
	/// </summary>
	public void SetUp() {
		
		DestroyOldItems();
		
		// List to make sure that the items on the bar are from different themes.
		m_currentThemes = new List<string>();
		
		// int to pick one of the four items that will match with current row.
		int randomItemPosition = Random.Range(0, Constants.NUMBER_OF_SELECTABLE_ITEMS);

		m_currentRowScript = m_rowsManager.GetCurrentRowScript();
		
		// What is the current theme name?
		string currentTheme = m_currentRowScript.GetThemeName();
		
		// Add it on used themes.
		List<string> itemThemes = new List<string>();

		for (int i = 0; i < Constants.NUMBER_OF_SELECTABLE_ITEMS; i++) {
			
			string itemName;
			string selectTheme = currentTheme;
			
			GameObject item = (GameObject) Instantiate(m_itemHolder);
			item.AddComponent<SelectItemScript>(); // selectable item.
			
			// this is the item that matches.
			if (randomItemPosition == i) {
				itemName = m_currentRowScript.GetMissingElement();

				item.GetComponent<SelectItemScript>().SetUpItem(itemName, selectTheme, i, m_currentRowScript.gameObject.transform, m_showReel);
                
				m_selectItemsNames.Add(itemName);
				correctItemPos = item.transform.position;
			}
			// this is a random item.
			else {
				GameObject pictureLoaderObject = GameObject.Find(Constants.PICTURE_LOADER_NAME);
				PictureLoader pictureLoader = pictureLoaderObject.GetComponent<PictureLoader>();
				
				// Make sure it's not an item of the same theme! Item should be unique.
				if(Game.SuperMode != Game.Difficulty.Hard)
				{
					do {
						selectTheme = pictureLoader.SelectRandomTheme();
					} 
					while (selectTheme == currentTheme || itemThemes.IndexOf(selectTheme) != -1);
					
					itemName = pictureLoader.SelectRandomURL(selectTheme);
				} 
				else
				{
					selectTheme = currentTheme;
					
					do {
						itemName = pictureLoader.SelectRandomURL(selectTheme);
					} 
					while (m_selectItemsNames.IndexOf(itemName) != -1 
							|| itemName == m_currentRowScript.GetMissingElement());
				}
				
				item.GetComponent<SelectItemScript>().SetUpItem(itemName, selectTheme, i, gameObject.transform, m_showReel);
				itemThemes.Add(selectTheme);
				m_selectItems.Add(item);
				m_selectItemsNames.Add(itemName);
			}
		}
	}
	
	/// <summary>
	/// Returns true if the manager has the selectable item.
	/// </summary>
	public bool Contains(GameObject item) {
		
		return m_selectItems.IndexOf(item) != -1 || item.name == m_currentRowScript.GetMissingElement();
	}

	private void DestroyOldItems() {
		while (m_selectItems.Count > 0) {
			
			Destroy(m_selectItems[0]);
			m_selectItems.RemoveAt(0);
		}
	}
}
