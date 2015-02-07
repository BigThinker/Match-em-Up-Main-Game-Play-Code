// =================================================================================================
//
// Zoek 'm Op (Match 'm Up) Main Game Play Scripts.
// Copyright (c) 2014 Aldo Leka
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// =================================================================================================

using UnityEngine;
using System.Collections;

/// <summary>
/// Script used to add other important scripts to the game objects of the scene.
/// It has a reference of the current difficulty level of the game (super mode) 
/// and of the level (mode).
/// The scripts use Kinect Input from Kinect for Windows SDK and object tweening via iTween (itween.pixelplacement.com).
/// All the scripts are written by me (Aldo Leka), except the ones that use Kinect SDK.
/// </summary>
public class Game : MonoBehaviour {

	public enum Difficulty {Easy, Medium, Hard};
	
	/// <summary>
	/// Reference to the current level difficulty.
	/// See level narrative in Global.GetVariability code.
	/// </summary>
	public static Difficulty Mode;
	
	/// <summary>
	/// There are 3 difficulties of the game:
	/// Easy has no extra game-play features. It's the basic initial setup.
	/// Medium has added clouds (particle effect) to hide the items after a while (player has to remember).
	/// Hard has a timer where the player can focus on the items, after which one of them is removed and player has
	/// to remember which item it was. 
	/// </summary>
	public static Difficulty SuperMode; 

	private RowsManager 		m_rowsManager;
	private SelectItemsManager 	m_selectItemsManager;
	private PictureLoader 		m_pictureLoader;
	private ShowReelScript 		m_showReel;
	private GameObject			m_kinectMouseInput;
	private GameObject			m_logo;
	
	// Variables edited in Inspector / Unity Editor.
	public AudioClip CoverSound;
	public AudioClip UncoverSound;
	public AudioClip EndGameSound;
	public AudioClip SlideSound;
	public float LogoFadeTime = 1f;
	public float WaitForLogoTime = 1.5f;
	public float LogoOnScreenTime = 3.5f;
	
	void Start () {
		
		// set initial global state to menu.
        Global.state = Global.State.Menu;
        
		ShowGameLogo();
		Initialize();
	}
	
	void ShowGameLogo() {
		
		m_logo = GameObject.Find(Constants.LOGO_NAME);
		
		// make invisible immediately when starting up.
		iTween.FadeTo(m_logo, 0f, 0f);
		
		// and than make it appear / disappear again.
		StartCoroutine(AnimateLogo());
	}
	
	private IEnumerator AnimateLogo() {
		
		yield return new WaitForSeconds(WaitForLogoTime);
		iTween.FadeTo(m_logo, 1f, LogoFadeTime);
		
		yield return new WaitForSeconds(LogoOnScreenTime);
		iTween.FadeTo(m_logo, 0f, LogoFadeTime);
	}
	
	private void Initialize() {
		
		StartCoroutine(StartScripts());
	}
	
	private IEnumerator StartScripts() {
		
		yield return new WaitForSeconds(Constants.SECONDS_BEFORE_INIT_SCRIPTS);
		
		// point to a game object.
		GameObject sampleObject;
		
		// Start Picture Loader.
		sampleObject = new GameObject(Constants.PICTURE_LOADER_NAME);
		sampleObject.AddComponent<PictureLoader>();
		m_pictureLoader = sampleObject.GetComponent<PictureLoader>();
		
		// Start Rows Manager.
		sampleObject = new GameObject(Constants.ROW_MANAGER_NAME);
		sampleObject.AddComponent<RowsManager>();
		m_rowsManager = sampleObject.GetComponent<RowsManager>();
		sampleObject.transform.parent = transform;
		
		// Start Select Items Manager.
		sampleObject = new GameObject(Constants.SELECT_ITEMS_MANAGER_NAME);
		sampleObject.AddComponent<SelectItemsManager>();
		sampleObject.transform.parent = transform;
		m_selectItemsManager = sampleObject.GetComponent<SelectItemsManager>();
		
		// Start Show Reel.
		sampleObject = new GameObject(Constants.SHOW_REEL_NAME);
		sampleObject.AddComponent<ShowReelScript>();
		sampleObject.transform.parent = transform;
		m_showReel = sampleObject.GetComponent<ShowReelScript>();
	
		// Start Kinect Input.
	    m_kinectMouseInput = new GameObject(Constants.KINECT_MOUSE_INPUT_NAME);
	    m_kinectMouseInput.AddComponent<KinectMouseInput>();
        
        // and finally, lock mouse.
		Global.lockMouse = true;
	}
}
