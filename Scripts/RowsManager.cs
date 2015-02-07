using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// RowsManager is responsible for creating the rows on the scene,
/// activating the show reel, and managing the way rows are used throughout the game.
/// </summary>
public class RowsManager : MonoBehaviour {
	
	private GameObject m_objects;
	private SelectItemsManager m_selectItemsManager;
	private PictureLoader m_pictureLoader;
	private ShowReelScript m_showReel;
	private int m_currentRow;
	private List<RowScript> m_rows;
	private List<string> m_currentThemes;
	private List<GameObject> m_rightItems = new List<GameObject>();
	private AudioSource m_audioSource;
	private AudioClip m_endGameSound;
	private AudioClip m_slideSound;

	void Start () {
	
		Init();
	}
	
	private void Init() {
	
		m_objects = GameObject.Find(Constants.PARENT_OBJECT_NAME);
		m_rows = new List<RowScript>();

		m_selectItemsManager = GameObject.Find(Constants.SELECT_ITEMS_MANAGER_NAME).GetComponent<SelectItemsManager>();
		m_pictureLoader = GameObject.Find(Constants.PICTURE_LOADER_NAME).GetComponent<PictureLoader>();
		m_showReel = GameObject.Find(Constants.SHOW_REEL_NAME).GetComponent<ShowReelScript>();
		
		AddSounds();
	}
	
	private void AddSounds() {
	
		// Add AudioSource.
		m_audioSource = gameObject.AddComponent<AudioSource>();
		m_endGameSound = GameObject.Find(Constants.PARENT_OBJECT_NAME).GetComponent<Game>().EndGameSound;
		m_slideSound = GameObject.Find(Constants.PARENT_OBJECT_NAME).GetComponent<Game>().SlideSound;
	}
	
	/// <summary>
	/// Reset the rows and start gameplay again.
	/// </summary>
	public void ReSet() {
	
		// clear show reel from previous game.
		m_showReel.ClearItems();
		
		// initialize themes
		m_currentThemes = new List<string>();
		
		// reset current row.
		m_currentRow = 0;
	
		// add new rows.
		AddRows(Constants.NUMBER_OF_ROWS);
		
		// set global state.
		if (Game.SuperMode == Game.Difficulty.Hard) 
			Global.state = Global.State.Timer;
		else 
			StartCoroutine(OntoTheNextRow());
	}
	
	/// <summary>
	/// Setup all the rows in the current scene.
	/// </summary>
	private void AddRows(int n) {
		
		string themeName; // iterate.
		
		for (int i = 0; i < n; i++) {
			
			// select a theme.
			themeName = AddTheme();
			
			// create the row object.
			GameObject Row = new GameObject(Constants.ROW_NAME);
			
			// add the row script and initialize it.
			Row.AddComponent<RowScript>();
			Row.GetComponent<RowScript>().CreateItems(themeName, i, transform);
			
			// save it to the list.
			m_rows.Add(Row.GetComponent<RowScript>());
		}
	}
	
	/// <summary>
	/// Adds and returns a random theme.
	/// </summary>
	private string AddTheme() {
		
		string themeName;
		
		// Select random theme.
		do {
			themeName = m_pictureLoader.SelectRandomTheme();
		} 
		while (m_currentThemes.IndexOf(themeName) != -1);
		
		// save it to the list of current themes.
		m_currentThemes.Add(themeName);
		
		return themeName;
	}
	
	public RowScript GetCurrentRowScript() {
		
		return m_rows[m_currentRow];
	}
	
	public void NextRow() {
		
		// it's the last row -> end game.
		if (m_currentRow == m_rows.Count - 1) {
		
			StartCoroutine(EndGame());
		}
		// close current row -> go to next one.
		else {

            StartCoroutine(CloseCurrent());
		}
	}
	
	private IEnumerator EndGame() {
	
		// In medium, we remove clouds first, than close covers.
		if (Game.SuperMode == Game.Difficulty.Medium) {
			
			// destroy clouds.
			m_rows[m_currentRow].DestroyClouds();
			
			yield return new WaitForSeconds(Constants.CLOUD_TIME);
			
			// than, cover row.
			m_rows[m_currentRow].Cover();
		}
		else {
			m_rows[m_currentRow].Cover(); // cover the done row.
		}

		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME + Constants.INBETWEEN_COVERS_TIME);

        // close select items.
        m_selectItemsManager.CoverItems();

		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME + Constants.INBETWEEN_COVERS_TIME);
		
		m_audioSource.PlayOneShot(m_endGameSound);
		Global.lockMouse = true;
		
		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME + Constants.INBETWEEN_COVERS_TIME);
		
		// add item to show reel.
		m_showReel.Add(m_rightItems[0]);
        
        Global.state = Global.State.ShowReel;
        
        // start show reel.
		m_showReel.Begin(true);
	}

    public void DestroyRows(){
        if (m_currentRow == m_rows.Count - 1) m_currentRow = 0; // reset.
        else m_currentRow++;// the next element.

        for (int i = 0; i < m_rows.Count; i++)
        {

            // Destroy object with all the items inside it.
            Destroy(m_rows[i].gameObject);
        }

        // delete all entries.
        while (m_rows.Count > 0) m_rows.RemoveAt(m_rows.Count - 1);
        m_rightItems = new List<GameObject>();
    }

    private IEnumerator CloseCurrent()
    {
    	// In medium, we remove clouds first, than close covers.
    	if (Game.SuperMode == Game.Difficulty.Medium) {
    		
    		// destroy clouds.
    		m_rows[m_currentRow].DestroyClouds();
    		
			yield return new WaitForSeconds(Constants.CLOUD_TIME);
    		
    		// than, cover row.
			m_rows[m_currentRow].Cover();
    	}
    	else {
			m_rows[m_currentRow].Cover(); // cover the done row.
    	}

        if (m_currentRow == m_rows.Count - 1) m_currentRow = 0; // reset.
        else m_currentRow++;// the next element.

		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME + Constants.INBETWEEN_COVERS_TIME);

        m_selectItemsManager.CoverItems();

		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME + Constants.INBETWEEN_COVERS_TIME);
		
        Global.state = Global.State.ShowImg;
        
        // start show reel!
        m_showReel.Begin();
    }
	
	public IEnumerator OntoTheNextRow() {
	
		// Safety wait for other scripts.
		yield return new WaitForSeconds(Constants.SECONDS_BEFORE_INIT_SCRIPTS);
        
		// move the cover.
		m_rows[m_currentRow].UnCover();
		m_selectItemsManager.SetUp();
		
		m_audioSource.PlayOneShot(m_slideSound);
		
		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME + Constants.INBETWEEN_COVERS_TIME);
		
		// In Medium supermode, we show clouds on top of items to hide them.
		if (Game.SuperMode == Game.Difficulty.Medium) {
			
			// show clouds first.
			m_rows[m_currentRow].ShowClouds();
			
			yield return new WaitForSeconds(Constants.CLOUD_TIME + Constants.WAIT_BEFORE_CLOUDS);
			
			// than uncover select bar after clouds appear.
			m_selectItemsManager.UnCoverItems();
		}	
		else if (Game.SuperMode != Game.Difficulty.Hard) {
		
			m_selectItemsManager.UnCoverItems();
		}
		
		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME + Constants.INBETWEEN_COVERS_TIME);

		// activate timer for hard.
		if(Game.SuperMode == Game.Difficulty.Hard) 
			Global.timerRunning = true;
	}
	
	public void SwitchCovers() {
	
		StartCoroutine(Switch());
	}
	
	private IEnumerator Switch() {
		
		Global.lockMouse = true;
		
		if (m_rowIsShown) {
			
			m_rowIsShown = false;
			m_rows[m_currentRow].Cover();
			m_selectItemsManager.UnCoverItems();
		}
		else {
			m_rowIsShown = true;
			m_rows[m_currentRow].UnCover();
			m_selectItemsManager.CoverItems();
		}
		
		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME);
		
		Global.lockMouse = false;
	}
	
	private IEnumerator OpenAndThanCloseRow() {
	
		m_rows[m_currentRow].GetComponent<RowScript>().Cover();
		
		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME + 1);
		
		m_rows[m_currentRow].GetComponent<RowScript>().SelectMissingItem ();
		m_rows[m_currentRow].GetComponent<RowScript>().UnCover();
		
		yield return new WaitForSeconds(Constants.COVER_MOVE_TIME);
		
		m_selectItemsManager.UnCoverItems();
	}
}
