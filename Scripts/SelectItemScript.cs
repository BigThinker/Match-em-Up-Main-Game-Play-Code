using UnityEngine;
using System.Collections;

/// <summary>
/// SelectItemScript is a script that describes the behavior of an item 
/// that is on the right selection manager area.
/// </summary>
public class SelectItemScript : MonoBehaviour {

	private RowsManager 		m_rowsManager;
    private ShowReelScript 		m_showReel;
	private Vector3 			m_missingItemPosition;
    private SelectItemsManager 	m_selectItemManager;
    private Vector3 			m_startPoint;
	
	void Start () {
        
		m_selectItemManager = GameObject.Find(Constants.SELECT_ITEMS_MANAGER_NAME).GetComponent<SelectItemsManager>();
		m_rowsManager = GameObject.Find(Constants.ROW_MANAGER_NAME).GetComponent<RowsManager>();
	}
	
	public void SetUpItem(string name, string themeName, int z, Transform parent, ShowReelScript showReel) {
		
		// set name.
		gameObject.name = name;
		// set texture
        renderer.material.mainTexture = (Texture) Resources.Load("Pictures/" + themeName + "/" + name); 
		// set position.
		transform.position = new Vector3(9f, Constants.ITEM_GROUND_LEVEL, 2.125f * -z - 1.3125f); // set position.
		// set starting position.
		m_startPoint = transform.position;
		// save a reference to show reel.
        m_showReel = showReel;
        // set parent.
		transform.parent = parent;
	}
	
	/// <summary>
	/// Puts the correct item in the missing position on the current row automatically.
	/// </summary>
	public IEnumerator PutItem() {
        
		Global.lockMouse = true;
		iTween.MoveTo(this.gameObject, 
						new Vector3(transform.position.x, Constants.SELECT_ITEM_HIGHER_Y, transform.position.z), 
						Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, m_missingItemPosition, Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, 
						new Vector3(transform.position.x, Constants.ITEM_GROUND_LEVEL, transform.position.z), 
						Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
        m_selectItemManager.CreateRowCompleteParticles();
        
		yield return new WaitForSeconds(Constants.WAIT_BEFORE_UNCLOCK_MOUSE);
        
        m_selectItemManager.DestroyParticles();
        m_showReel.Add(this.gameObject);
		m_rowsManager.NextRow();
	}
	
	/// <summary>
	/// Puts the correct item in the missing position on the current row after 
	/// user manually moved on top of the missing item spot.
	/// </summary>
	public IEnumerator PutRightItem() {
		
		Global.lockMouse = true;
		iTween.MoveTo(this.gameObject, m_missingItemPosition, Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, 
						new Vector3(transform.position.x, Constants.ITEM_GROUND_LEVEL, transform.position.z), 
						Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		m_selectItemManager.CreateRowCompleteParticles();
		
		yield return new WaitForSeconds(Constants.WAIT_BEFORE_UNCLOCK_MOUSE);
		
		m_selectItemManager.DestroyParticles();
		m_showReel.Add(this.gameObject);
		m_rowsManager.NextRow();
		Global.lockMouse = false;
	}
	
	/// <summary>
	/// Puts the wrong item in the missing position on the current row 
	/// automatically and puts it back to original position.
	/// </summary>
	public IEnumerator PutAutomaticWrongItem() {
		
		// lock mouse
		Global.lockMouse = true;
		iTween.MoveTo(this.gameObject, 
						new Vector3(transform.position.x, Constants.SELECT_ITEM_HIGHER_Y, transform.position.z), 
						Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, m_missingItemPosition, Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, 
						new Vector3(transform.position.x, Constants.ITEM_GROUND_LEVEL, transform.position.z), 
						Constants.SELECT_ITEM_MOVE_TIME);
		
		// wait a little bit.
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, 
						new Vector3(transform.position.x, Constants.SelectItemUpYPos, transform.position.z), 
						Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, 
						new Vector3(m_startPoint.x, Constants.SelectItemUpYPos, m_startPoint.z), 
						Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, m_startPoint, Constants.SELECT_ITEM_MOVE_TIME);
		
		// wait a little before unlocking mouse.
		yield return new WaitForSeconds(Constants.WAIT_BEFORE_UNCLOCK_MOUSE);
		
		Global.lockMouse = false;
	}
	
	/// <summary>
	/// Puts the wrong item in the original position after manually moved by the user.
	/// </summary>
	public IEnumerator PutWrongItem(){
		
		Global.lockMouse = true;
		iTween.MoveTo(this.gameObject, 
						new Vector3(m_startPoint.x, gameObject.transform.position.y, m_startPoint.z), 
						Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		iTween.MoveTo(this.gameObject, m_startPoint, Constants.SELECT_ITEM_MOVE_TIME);
		
		yield return new WaitForSeconds(Constants.SELECT_ITEM_MOVE_TIME);
		
		m_showReel.Add(this.gameObject);
		
		// wait a little before unlocking mouse.
		yield return new WaitForSeconds(Constants.WAIT_BEFORE_UNCLOCK_MOUSE);
		
		Global.lockMouse = false;
	}
}
