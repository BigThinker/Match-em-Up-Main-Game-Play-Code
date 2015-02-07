using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ShowReelScript is responsible for showing a list of pictures in
/// various frames and providing ability to move through the pictures and exit.
/// </summary>
public class ShowReelScript : MonoBehaviour {
	
	private List<GameObject> 	m_frames;
	private GameObject 			m_picturePlane;
	private List<GameObject> 	m_showReelItems;
	private int 				m_currentPicture;
	private RowsManager 		m_rowsManager;
	private bool 				m_started;
	private bool 				m_canMoveReel;
	private KinectGestureListener m_kinectGestureListener;
	
	void Start () {
	
		m_frames = new List<GameObject>();
		
		// save all game objects here, to instantiate later.
		for (int i = 0; i < Constants.SHOW_REEL_FRAME_NAMES.Count; i++) {
			m_frames.Add( (GameObject) Resources.Load(Constants.SHOW_REEL_FRAME_NAMES[i]) );
		}
		
		// save the plane game object here.
		m_picturePlane = (GameObject) Resources.Load(Constants.FRAME_PLANE_NAME);
		m_started = false;
		m_showReelItems = new List<GameObject>();
		m_rowsManager = GameObject.Find(Constants.ROW_MANAGER_NAME).GetComponent<RowsManager>();
		m_currentPicture = 0;
		m_kinectGestureListener = Camera.main.GetComponent<KinectGestureListener>();
	}
	
	/// <summary>
	/// Start the show reel.
	/// </summary>
	public void Begin(bool isEnd = false) {
        
		m_started = true;
		m_canMoveReel = true;
		
		float startX = 2f + Constants.SHOWREEL_ITEM_XOFFSET;
		
		for (int i = 0; i < m_showReelItems.Count; i++) {
			GameObject showReelItem = m_showReelItems[i];
			showReelItem.transform.position = new Vector3(startX + i * Constants.SHOWREEL_ITEM_XOFFSET, 9f, -3.9f);
		}
		
		// show all items from start.
		if (isEnd) 
		{
			MoveItems(-Constants.SHOWREEL_ITEM_XOFFSET);
		}
		// show last item and other items on left.
		else 
		{
			m_currentPicture = m_showReelItems.Count - 1;
			
			// move left until final item is in front of the camera.
			MoveItems(-Constants.SHOWREEL_ITEM_XOFFSET * (m_currentPicture + 1), true);
			
			// move final item on the right and slide it in.
			Vector3 lastItemPos = m_showReelItems[m_currentPicture].transform.position;
			m_showReelItems[m_currentPicture].transform.position = new Vector3(startX + Constants.SHOWREEL_ITEM_XOFFSET * (m_currentPicture + 1), 
																				lastItemPos.y, 
																				lastItemPos.z);
			iTween.MoveBy(m_showReelItems[m_currentPicture], 
			              new Vector3(Constants.SHOWREEL_ITEM_XOFFSET, 0, 0), 
			              Constants.SHOWREEL_MOVE_TIME);
		}
	}
	
	/// <summary>
	/// Put an item in the show reel. (It is appended)
	/// </summary>
	public void Add(GameObject item){
		
		// it already started, can't add items.
		if (m_started || item == null) return;
		
		// get item's texture (picture).
		Texture itemTexture = item.renderer.material.mainTexture;
		
		// Instantiate plane.
		GameObject picturePlane = (GameObject) Instantiate(m_picturePlane);
		picturePlane.renderer.material.mainTexture = itemTexture;
		picturePlane.transform.parent = transform;
		
		// Instantiate random frame.
		GameObject frame = (GameObject) Instantiate( m_frames[Random.Range(0, m_frames.Count)] );
		frame.transform.parent = picturePlane.transform;
		
		// Add this to the show reel items.
		m_showReelItems.Add(picturePlane);
	}
	
	/// <summary>
	/// Control the show reel with keyboard input.
	/// </summary>
	void Update () {
	
		if (m_started && m_canMoveReel)
		{
        	float moveX = 0;
        	
        	// replace with button.
            if (Input.GetKeyDown(KeyCode.A))
            {
				moveX = Constants.SHOWREEL_ITEM_XOFFSET;
				m_currentPicture--;
				
				// constraint
				if (m_currentPicture < 0) m_currentPicture = 0;
				else MoveItems(moveX);
				
				StartCoroutine(SwitchMoveReelFlag());
			}
			// replace with button.
            else if (Input.GetKeyDown(KeyCode.D)) 
			{
				moveX = -Constants.SHOWREEL_ITEM_XOFFSET;
				m_currentPicture++;
				
				// constraint
				if (m_currentPicture > m_showReelItems.Count - 1) m_currentPicture = m_showReelItems.Count - 1;
				else MoveItems(moveX);
				
				StartCoroutine(SwitchMoveReelFlag());
			}
        }
    }
    
    public void ClearItems() {
    	
		while (m_showReelItems.Count > 0) {
			Destroy(m_showReelItems[0]);
			m_showReelItems.RemoveAt(0);
		}
    }
    
	private IEnumerator SwitchMoveReelFlag() {
    	
    	m_canMoveReel = false;
    	Global.lockMouse = true;
    	
		yield return new WaitForSeconds(Constants.SHOWREEL_MOVE_TIME);
    	
    	m_canMoveReel = true;
		Global.lockMouse = false;
	}
    
    private void MoveItems(float moveX, bool instant = false) {
    
		float time = instant ? 0f : Constants.SHOWREEL_MOVE_TIME;
		iTween.MoveTo(gameObject, new Vector3(moveX + transform.position.x, 0, 0), time);
	}
	
	/// <summary>
	/// Call to reset the game, starting from show reel.
	/// </summary>
	public void EndShow(bool reset) {
		StartCoroutine(ResetGame(reset));
	}
	
	private IEnumerator ResetGame(bool reset) {
	
		m_started = false;
		
		iTween.MoveBy(m_showReelItems[m_currentPicture], 
		              new Vector3(Constants.SHOWREEL_ITEM_XOFFSET, 0, 0), Constants.SHOWREEL_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SHOWREEL_MOVE_TIME);

        if (!reset)
        {
            m_currentPicture = 0;
            m_rowsManager.DestroyRows();
            m_started = false;
        }

        if (Global.state == Global.State.ShowReel)
        {
			yield return new WaitForSeconds(Constants.COVER_MOVE_TIME);

            this.gameObject.transform.position = Vector3.zero;
            
            m_currentPicture = 0;
            // reset game.
            m_rowsManager.DestroyRows();
            if (reset == true) { m_rowsManager.ReSet(); Global.state = Global.State.Playing;}
            else Global.state = Global.State.Menu;

        }
        
        if (reset) 
        {   
            if (Global.state == Global.State.ShowImg && Game.SuperMode != Game.Difficulty.Hard) 
            {
			    StartCoroutine (m_rowsManager.OntoNextRow ());
			    Global.state = Global.State.Playing;
	        }
	        else if (Global.state == Global.State.ShowImg && Game.SuperMode == Game.Difficulty.Hard) 
	        {
			    Global.state = Global.State.Timer;
		    }
        }
	}
}
