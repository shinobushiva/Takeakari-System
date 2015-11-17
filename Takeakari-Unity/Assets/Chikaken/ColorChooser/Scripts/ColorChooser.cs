/****************************************************************************
 * ColorChooser
 * Franz Reichel
 * 17.04.2014
 * 
 * Description:
 * It is a color chooser like the Photoshop/Gimp color chooser
 * The delivered guiskin is similar to the unity color chooser
 * 
 * The color conversion algorithms coming from:
 * http://www.cs.rit.edu/~ncs/color/t_convert.html
 */

using UnityEngine;
using System.Collections;

public class ColorChooser : SingletonMonoBehaviour<ColorChooser> 
{
	// The material reference, which will be assigned from outside
	private Material material;

	// Texture Maps for slider and maps
	private Texture2D map;
	private Texture2D tex_selectedColor;
	private Texture2D tex_selectedAlpha;

	private Texture2D mapHSV_V, mapS_V, mapV_V;
	private Texture2D mapHSV_H, mapS_H, mapV_H;

	private Texture2D mapR_V, mapG_V, mapB_V;
	private Texture2D mapR_H, mapG_H, mapB_H;

	private Texture2D mapA_H;

	// Texture imports for Gui
	private Texture2D tex_pipette;
	private Texture2D tex_choose1;
	private Texture2D tex_choose2;
	private Texture2D tex_closeX;
	private Texture2D tex_showMenu;
	private Texture2D tex_closeMenu;

	// Selected color
	private Color selectedColor = new Color(1,0,0,1);
	private Color selectedHColor = new Color(0,0,0);
	private static Color pipetteColor;

	// Skin
	private GUISkin mySkin;
	// Save the old skin
	private GUISkin oldSkin;

	// Selected Color Mode
	private int mode = 0;
	private int oldMode = 0;

	// Count of Color Modes
	private int modeCount = 6;

	// Mouse over map
	private bool overMap = false;
	// Hold for cursor if mouse is not over map anymore
	private bool hold = false;

	// Update RGB Values 
	private bool updateValuesRGB = false;
	// Update HSV Values 
	private bool updateValuesHSV = false;

	// Update for raycast blocking 
	public bool mouseOverWindow = false;

	// Helpers
	private float ftmpH = 0;
	private float ftmpS = 0;
	private float ftmpV = 0;

	private float ftmpR = 0;
	private float ftmpG = 0;
	private float ftmpB = 0;

	private float ftmpA = 1;

	private float fc_tmp = 0;
	private Color ctmp;
	private static float eps = 0.0000001f;
	private bool realColortransparency = false;

	// Testing Zoom
	//private Texture2D testPic;

	// Refresh color maps
	private bool colorModeRGB = true;
	private bool showColorMap = true;
	private bool showColorSliders = true;

	// Cursor
	private Vector2 cursorMapPosition = new Vector2();

	// Window information
	private Rect windowRect;
	public Vector2 windowPosition = new Vector2(10, 10);
	private float windowWidth = 200;
	private float windowHeight = 370;

	// Color map information
	private int mapWidth = 134;
	private int mapHeight = 134;

	// Color slider information
	private int sliderHWidth = 100;
	private int sliderHHeight = 1;

	private int sliderVWidth = 1;

	// Use this for initialization
	void Awake () 
	{
		pipMode = false;

		// Only allow ONE instance of this script
		ColorChooser [] cc = (ColorChooser[])GameObject.FindObjectsOfType(typeof(ColorChooser));

		if(cc.Length > 1)
		{
			for (int i = 1 ; i < cc.Length; i++)
			{
				Destroy(cc[i]);	// Destroy all ColorChooser on this object except the new one
			}
		}

		// Load textures
		tex_pipette = (Texture2D) Resources.Load("colorChooser/pipette");
		tex_choose1 = (Texture2D) Resources.Load("colorChooser/choose1");
		tex_choose2 = (Texture2D) Resources.Load("colorChooser/choose2");
		tex_closeX = (Texture2D) Resources.Load("colorChooser/close");
		tex_showMenu = (Texture2D) Resources.Load("colorChooser/arrowdown");
		tex_closeMenu = (Texture2D) Resources.Load("colorChooser/arrowright");
		//testPic = (Texture2D) Resources.Load("colorChooser/testPic");

		// Load skin
		mySkin = (GUISkin) Resources.Load("colorChooser/colorChooserSkin");

		// Initialize maps and sliders
		map = new Texture2D(mapWidth, mapHeight);
		tex_selectedColor = new Texture2D(mapWidth, 17);
		tex_selectedAlpha = new Texture2D(mapWidth, 3);

		mapHSV_V = new Texture2D(sliderVWidth, mapHeight);
		mapS_V = new Texture2D(sliderVWidth, mapHeight);
		mapV_V = new Texture2D(sliderVWidth, mapHeight);
		mapHSV_H = new Texture2D(mapWidth, sliderHHeight);
		mapS_H = new Texture2D(sliderHWidth, sliderHHeight);
		mapV_H = new Texture2D(sliderHWidth, sliderHHeight);

		mapR_V = new Texture2D(sliderVWidth, mapHeight);
		mapG_V = new Texture2D(sliderVWidth, mapHeight);
		mapB_V = new Texture2D(sliderVWidth, mapHeight);
		mapR_H = new Texture2D(sliderHWidth, sliderHHeight);
		mapG_H = new Texture2D(sliderHWidth, sliderHHeight);
		mapB_H = new Texture2D(sliderHWidth, sliderHHeight);

		mapA_H = new Texture2D(sliderHWidth, sliderHHeight);

		// Default position
		setPostion(windowPosition);
		refreshWindow();

		// Create HSV colorline 
		makeHSVColorLine();
		// Create preview texture
		makePreview();
		// Create alphaline
		makeAlphaLine();

		// Update all maps and sliders
		updateAll();
	}

	//---------------------------------------------------------------------------------------------------- Public functions
	// Set window position
	public void setPostion(Vector2 pos)
	{
		int xCorrection = Screen.width - (int)(pos.x + windowWidth);
		int yCorrection = Screen.height - (int)(pos.y + windowHeight);

		if (xCorrection >= 0)
		{
			xCorrection = 0;
		}
		else
		{	// Switch Side on the cursor
			xCorrection = - (20 + (int)windowWidth);
		}

		if (yCorrection >= 0)
		{
			yCorrection = 0;
		}

		windowRect = new Rect(pos.x + xCorrection, pos.y + yCorrection, windowWidth, windowHeight);
	}

	// Set initial color through material
	public void setColor(Material mat) 
	{
		material = mat;

		ftmpR = material.color.r;
		ftmpG = material.color.g;
		ftmpB = material.color.b;
		ftmpA = material.color.a;

		// Lets do an update after assignment
		updateValuesRGB= true;
	}

	public void setColor(Color c){
		ftmpR = c.r;
		ftmpG = c.g;
		ftmpB = c.b;
		ftmpA = c.a;
		
		// Lets do an update after assignment
		updateValuesRGB= true;
	}

	// Get choosen color
	public Color getColor()
	{
		return selectedColor;
	}

	// returns true if in pipette mode
	public bool isPipetteMode()
	{
		if (mode == 6)
		{
			return true;
		}

		return false;
	}

	// You can check here if raycast should be blocked
	// because the cursor is over the GUI or the color chooser is in pipette mode
	public bool blockRayCast()
	{
		if (mouseOverWindow || isPipetteMode())
		{
			return true;
		}

		return false;
	}

	//---------------------------------------------------------------------------------------------------- Update
	// Update is called once per frame
	void Update () 
	{
		// Mouse Down
		if (Input.GetMouseButtonDown(0))
		{
			if(isPipetteMode())
			{
				// reset mode
				mode = oldMode;
				// set the old mode to 6 to use him as switcher for the close button
				oldMode = 6;

				ftmpR = pipetteColor.r;
				ftmpG = pipetteColor.g;
				ftmpB = pipetteColor.b;
				ftmpA = pipetteColor.a;
				
				updateValuesRGB = true;
			}

			if (overMap)
			{
				hold = true;
			}
		}
		// Mouse Pressed
		if (Input.GetMouseButton(0) && (hold || overMap))
		{
			switch(mode)
			{
			case 1:
				ftmpH = cursorMapPosition.x / map.height;
				ftmpV = cursorMapPosition.y / map.width;
				updateValuesRGB = false;
				updateValuesHSV = true;
				break;
			case 2:
				ftmpH = cursorMapPosition.x / map.height;
				ftmpS = cursorMapPosition.y / map.width;
				updateValuesRGB = false;
				updateValuesHSV = true;
				break;
			case 3:
				ftmpB = cursorMapPosition.x / map.height;
				ftmpG = cursorMapPosition.y / map.width;
				updateValuesRGB = true;
				updateValuesHSV = false;
				break;
			case 4:
				ftmpB = cursorMapPosition.x / map.height;
				ftmpR = cursorMapPosition.y / map.width;
				updateValuesRGB = true;
				updateValuesHSV = false;
				break;
			case 5:
				ftmpR = cursorMapPosition.x / map.height;
				ftmpG = cursorMapPosition.y / map.width;
				updateValuesRGB = true;
				updateValuesHSV = false;
				break;
			default:
				ftmpS = cursorMapPosition.x / map.height;
				ftmpV = cursorMapPosition.y / map.width;
				updateValuesRGB = false;
				updateValuesHSV = true;
				break;
			}

			updateCursorInColorMap();
		}

		// Mouse Up
		if (Input.GetMouseButtonUp(0))
		{
			hold = false;

			// Reset oldMode
			oldMode = 0;
		}

		if (isPipetteMode())
		{
			updateColorMap();
		}
	}

	//---------------------------------------------------------------------------------------------------- GUI Window refresh for movement
	private void refreshWindow()
	{
		windowRect = new Rect(windowRect.x, windowRect.y, windowWidth, windowHeight);
	}
	
	//---------------------------------------------------------------------------------------------------- GUI
	void OnGUI()
	{
		oldSkin = GUI.skin;
		GUI.skin = mySkin;

		windowRect = GUI.Window(0, windowRect, chooser, "");

		mouseOverWindow = windowRect.Contains(new Vector3(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 0));
		// TestPicture for Zoom
		//GUI.Label(new Rect(10,10,3,3), testPic);

		GUI.skin = oldSkin;
	}

	public void DrawChooser(){

		colorChooserHead();
		
		GUILayout.BeginArea (new Rect (10, 30, windowWidth-10, windowHeight-5));
		
		GUILayout.Space(2);
		GUILayout.BeginHorizontal ();
		
		GUILayout.BeginVertical ();
		
		colorMenu();
		colorChooser_MapAndSideBar();
		GUILayout.Space(1);
		
		horizontalColorSliderHead();
		
		// HSV Horizontal Sliders
		if (!colorModeRGB && showColorSliders)
		{
			horizontalColorSlider("H", ref ftmpH, mapHSV_H, 360, 0);
			horizontalColorSlider("S", ref ftmpS, mapS_H, 255, 0);
			horizontalColorSlider("V", ref ftmpV, mapV_H, 255, 0);
		}
		
		// Update RGB
		if (updateValuesHSV)
		{
			selectedHColor = HSVtoRGB(ftmpH * (360-1), 1, 1);
			selectedColor = HSVtoRGB(ftmpH * (360-1), ftmpS, ftmpV);
			
			ftmpR = selectedColor.r;
			ftmpG = selectedColor.g;
			ftmpB = selectedColor.b;
		}
		
		// RGB Horizontal Sliders
		if (colorModeRGB && showColorSliders)
		{
			horizontalColorSlider("R", ref ftmpR, mapR_H, 255, 1);
			horizontalColorSlider("G", ref ftmpG, mapG_H, 255, 1);
			horizontalColorSlider("B", ref ftmpB, mapB_H, 255, 1);
		}
		// Update HSV
		if (updateValuesRGB)
		{
			HSVColor cc = RGBtoHSV(ftmpR, ftmpG, ftmpB);
			ftmpH = cc.H/360f;
			ftmpS = cc.S;
			ftmpV = cc.V;
			selectedHColor = HSVtoRGB(ftmpH * (360-1), 1, 1);
		}
		
		if (showColorSliders)
		{
			horizontalColorSlider("A", ref ftmpA, mapA_H, 255, 1);
		}
		
		if (updateValuesRGB || updateValuesHSV)
		{
			selectedColor = new Color(ftmpR, ftmpG, ftmpB, ftmpA);
			// Update Material
			if (material != null)
			{
				material.color = selectedColor ;
			}
			updateAll();
		}
		
		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();
		
		GUILayout.EndArea ();
		
		updateValuesRGB = false;
		updateValuesHSV = false;
		
		// Drag Window
		//GUI.DragWindow(new Rect(0, 0, 190, 35));

	}
	
	//---------------------------------------------------------------------------------------------------- GUI ColorChooser
	private void chooser(int id)
	{
		colorChooserHead();

		GUILayout.BeginArea (new Rect (10, 30, windowWidth-10, windowHeight-5));

		GUILayout.Space(2);
		GUILayout.BeginHorizontal ();

		GUILayout.BeginVertical ();

		colorMenu();
		colorChooser_MapAndSideBar();
		GUILayout.Space(1);

		horizontalColorSliderHead();

		// HSV Horizontal Sliders
		if (!colorModeRGB && showColorSliders)
		{
			horizontalColorSlider("H", ref ftmpH, mapHSV_H, 360, 0);
			horizontalColorSlider("S", ref ftmpS, mapS_H, 255, 0);
			horizontalColorSlider("V", ref ftmpV, mapV_H, 255, 0);
		}

		// Update RGB
		if (updateValuesHSV)
		{
			selectedHColor = HSVtoRGB(ftmpH * (360-1), 1, 1);
			selectedColor = HSVtoRGB(ftmpH * (360-1), ftmpS, ftmpV);

			ftmpR = selectedColor.r;
			ftmpG = selectedColor.g;
			ftmpB = selectedColor.b;
		}

		// RGB Horizontal Sliders
		if (colorModeRGB && showColorSliders)
		{
			horizontalColorSlider("R", ref ftmpR, mapR_H, 255, 1);
			horizontalColorSlider("G", ref ftmpG, mapG_H, 255, 1);
			horizontalColorSlider("B", ref ftmpB, mapB_H, 255, 1);
		}
		// Update HSV
		if (updateValuesRGB)
		{
			HSVColor cc = RGBtoHSV(ftmpR, ftmpG, ftmpB);
			ftmpH = cc.H/360f;
			ftmpS = cc.S;
			ftmpV = cc.V;
			selectedHColor = HSVtoRGB(ftmpH * (360-1), 1, 1);
		}

		if (showColorSliders)
		{
			horizontalColorSlider("A", ref ftmpA, mapA_H, 255, 1);
		}

		if (updateValuesRGB || updateValuesHSV)
		{
			selectedColor = new Color(ftmpR, ftmpG, ftmpB, ftmpA);
			// Update Material
			if (material != null)
			{
				material.color = selectedColor ;
			}
			updateAll();
		}

		GUILayout.EndVertical ();
		GUILayout.EndHorizontal ();

		GUILayout.EndArea ();

		updateValuesRGB = false;
		updateValuesHSV = false;

		// Drag Window
		GUI.DragWindow(new Rect(0, 0, 190, 35));
	}

	//---------------------------------------------------------------------------------------------------- GUI Menu Head
	private void colorChooserHead()
	{
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label("Color Chooser", GUILayout.Height(20));
		
		GUILayout.FlexibleSpace();


		if(GUILayout.Button("", GUILayout.Height(20)))
		{
			if (oldMode != 6){
				//Destroy(this);
			}
		}

		
		GUILayout.EndHorizontal ();
	}

	//---------------------------------------------------------------------------------------------------- GUI Color Menu
	private void colorMenu()
	{
		GUILayout.BeginHorizontal ();
		
		if(GUILayout.Button(tex_pipette, GUILayout.Width(30), GUILayout.Height(30)))
		{
			oldMode = mode;
			mode = 6;
		}
		
		GUILayout.FlexibleSpace ();
		
		GUILayout.BeginVertical();
		ctmp = GUI.color;
		
		if (realColortransparency)
		{
			GUI.color = selectedColor;
		}
		else
		{
			GUI.color = new Color(selectedColor.r, selectedColor.g, selectedColor.b);
		}
		
		GUILayout.Label (tex_selectedColor, GUILayout.Width(tex_selectedColor.width), GUILayout.Height(tex_selectedColor.height));
		GUI.color = ctmp;
		if (!realColortransparency)
			GUILayout.Label (tex_selectedAlpha, GUILayout.Width(tex_selectedAlpha.width), GUILayout.Height(tex_selectedAlpha.height));
		GUILayout.EndVertical();
		
		GUILayout.Space(8);
		GUILayout.EndHorizontal ();
	}
	
	//---------------------------------------------------------------------------------------------------- GUI Map and vertical slider
	private void colorChooser_MapAndSideBar()
	{
		GUILayout.BeginHorizontal ();
		
		if(GUILayout.Button(showColorMap ? tex_showMenu : tex_closeMenu, GUILayout.Height(30)))
		{
			changeMapAndSideBarMenuState();
		}
		
		if(GUILayout.Button("Colors", GUILayout.Width(40), GUILayout.Height(30)))
		{
			changeMapAndSideBarMenuState();
		}
		
		GUILayout.FlexibleSpace ();
		
		// Mode Switcher
		if(GUILayout.Button(tex_choose1, GUILayout.Width(40), GUILayout.Height(30)))
		{
			mode++;
			if (mode >= modeCount)
				mode = 0;
			
			updateColorMap();
		}
		
		GUILayout.EndHorizontal ();
		
		if (!showColorMap)
			return;
		
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label (map, GUILayout.Width(map.width), GUILayout.Height(map.height));
		
		// Mouse Control
		if (Event.current.type == EventType.Repaint)
		{
			if (hold || GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
			{
				cursorMapPosition.x = Event.current.mousePosition.x - GUILayoutUtility.GetLastRect().x;
				cursorMapPosition.y = GUILayoutUtility.GetLastRect().height - (Event.current.mousePosition.y - GUILayoutUtility.GetLastRect().y);
				
				// Cursor lock
				if (cursorMapPosition.x > map.width)
					cursorMapPosition.x = map.width;
				else if (cursorMapPosition.x < 0)
					cursorMapPosition.x = 0;
				
				if (cursorMapPosition.y > map.height)
					cursorMapPosition.y = map.height;
				else if (cursorMapPosition.y < 0)
					cursorMapPosition.y = 0;
				
				overMap = true;
			}
			else
			{
				overMap = false;
			}
		}
		
		GUILayout.FlexibleSpace ();
		
		// Change Vertical Slider
		switch(mode)
		{
		case 1:
			verticalColorSlider(ref ftmpS, mapS_V, 0);
			break;
		case 2:
			verticalColorSlider(ref ftmpV, mapV_V, 0);
			break;
		case 3:
			verticalColorSlider(ref ftmpR, mapR_V, 1);
			break;
		case 4:
			verticalColorSlider(ref ftmpG, mapG_V, 1);
			break;
		case 5:
			verticalColorSlider(ref ftmpB, mapB_V ,1);
			break;
		default:
			verticalColorSlider(ref ftmpH, mapHSV_V, 0);
			break;
		}
		GUILayout.Space(3);
		GUILayout.EndHorizontal ();
		
		// Testing cursor coordinates
		//GUILayout.Label (cursorMapPosition.x + "");
		//GUILayout.Label (cursorMapPosition.y + "");
	}
	
	//---------------------------------------------------------------------------------------------------- GUI horizontal slider
	private void horizontalColorSliderHead()
	{
		GUILayout.BeginHorizontal ();
		
		if(GUILayout.Button(showColorSliders ? tex_showMenu : tex_closeMenu, GUILayout.Height(30)))
		{
			changeSliderMenuState();
		}
		
		if(GUILayout.Button("Sliders", GUILayout.Width(45), GUILayout.Height(30)))
		{
			changeSliderMenuState();
		}
		
		GUILayout.FlexibleSpace ();
		
		// Mode Switcher
		if(GUILayout.Button(tex_choose2, GUILayout.Width(40), GUILayout.Height(30)))
		{
			colorModeRGB = colorModeRGB ? false : true;
		}
		
		GUILayout.EndHorizontal ();
	}

	//---------------------------------------------------------------------------------------------------- Menu visibility
	private void changeMapAndSideBarMenuState()
	{
		showColorMap = showColorMap ? false : true;
		
		if (showColorMap)
		{
			windowHeight += 140;
		}
		else
		{
			windowHeight -= 140;
		}
		refreshWindow();
	}
	
	void changeSliderMenuState()
	{
		showColorSliders = showColorSliders ? false : true;
		
		if (showColorSliders)
		{
			windowHeight += 90;
		}
		else
		{
			windowHeight -= 90;
		}
		refreshWindow();
	}

	//---------------------------------------------------------------------------------------------------- GUI vertical slider
	void verticalColorSlider(ref float value, Texture2D ColMap, int update)
	{
		fc_tmp = value;
		mySkin.verticalSlider.normal.background = ColMap;
		value = GUILayout.VerticalSlider (value, 1, 0, GUILayout.Width(20), GUILayout.Height(ColMap.height));
		
		if (update == 0)
			updateValuesHSV = Mathf.Abs(fc_tmp - value) > eps ? true : updateValuesHSV;
		else
			updateValuesRGB = Mathf.Abs(fc_tmp - value) > eps ? true : updateValuesRGB;
	}

	//---------------------------------------------------------------------------------------------------- GUI horizontal slider
	void horizontalColorSlider(string name, ref float value, Texture2D ColMap, int exchange, int update)
	{
		GUILayout.BeginHorizontal ();
		
		GUILayout.Label (name);

		GUILayout.Space(5);

		fc_tmp = value;
		mySkin.horizontalSlider.normal.background = ColMap;
		value = GUILayout.HorizontalSlider (value, 0, 1, GUILayout.Width(100), GUILayout.Height(20));
		
		GUILayout.FlexibleSpace ();
		
		int var = (int)(value * exchange);
		float rest = (value * exchange) % var;
		if (float.IsNaN(rest))
			rest = 0;
		int.TryParse(GUILayout.TextField (var + "", GUILayout.Width(30)), out var);
		value = Mathf.Clamp(var + rest, 0, exchange) / exchange;
		
		if (update == 0)
		{
			updateValuesHSV = Mathf.Abs(fc_tmp - value) > eps ? true : updateValuesHSV;
		}
		else
		{
			updateValuesRGB = Mathf.Abs(fc_tmp - value) > eps ? true : updateValuesRGB;
		}

		GUILayout.Space(10);
		GUILayout.EndHorizontal ();
	}
	
	//---------------------------------------------------------------------------------------------------- Create preview
	private void makePreview()
	{
		for (int y = 0 ; y < tex_selectedColor.height  ; y++)
		{
			for (int x = 0 ; x < tex_selectedColor.width  ; x++)
			{
				tex_selectedColor.SetPixel(x, y, Color.white);
			}
		}	

		tex_selectedColor.Apply();
	}

	//---------------------------------------------------------------------------------------------------- Create alphaline
	private void makeAlphaLine()
	{
		Color c = new Color(0,0,0);
		
		// Horizontal
		for (int x = 0 ; x < mapA_H.width  ; x++)
		{
			c = Color.Lerp(Color.black, Color.white, (float)x/mapA_H.width);
			mapA_H.SetPixel (x, 1, c);
		}	
		
		mapA_H.Apply(true);
	}

	//---------------------------------------------------------------------------------------------------- Create HSV Colorline
	private void makeHSVColorLine()
	{
		Color c = new Color(0,0,0);

		// Horizontal
		for (int x = 0 ; x < mapHSV_H.width  ; x++)
		{
			if (x < mapHSV_H.width * 1/6f)
				c = Color.Lerp(Color.red, Color.yellow, x/( mapHSV_H.width/ 6f) - 0);
			else if (x < mapHSV_H.width * 2/6f)
				c = Color.Lerp(Color.yellow, Color.green, x/(mapHSV_H.width / 6f) - 1);
			else if (x < mapHSV_H.width * 3/6f)
				c = Color.Lerp(Color.green, Color.cyan, x/(mapHSV_H.width / 6f) - 2);
			else if (x < mapHSV_H.width * 4/6f)
				c = Color.Lerp(Color.cyan, Color.blue, x/(mapHSV_H.width / 6f) - 3);
			else if (x < mapHSV_H.width * 5/6f)
				c = Color.Lerp(Color.blue, Color.magenta, x/(mapHSV_H.width / 6f) - 4);
			else if (x < mapHSV_H.width * 6/6f)
				c = Color.Lerp(Color.magenta, Color.red, x/(mapHSV_H.width / 6f) - 5);
			
			mapHSV_H.SetPixel (x, 1, c);
		}
		mapHSV_H.Apply(true);

		// Vertical
		for (int y = 0 ; y < mapHSV_V.height  ; y++)
		{
			if (y < mapHSV_V.height * 1/6f)
				c = Color.Lerp(Color.red, Color.yellow, y/( mapHSV_V.height/ 6f) - 0);
			else if (y < mapHSV_V.height * 2/6f)
				c = Color.Lerp(Color.yellow, Color.green, y/(mapHSV_V.height / 6f) - 1);
			else if (y < mapHSV_V.height * 3/6f)
				c = Color.Lerp(Color.green, Color.cyan, y/(mapHSV_V.height / 6f) - 2);
			else if (y < mapHSV_V.height * 4/6f)
				c = Color.Lerp(Color.cyan, Color.blue, y/(mapHSV_V.height / 6f) - 3);
			else if (y < mapHSV_V.height * 5/6f)
				c = Color.Lerp(Color.blue, Color.magenta, y/(mapHSV_V.height / 6f) - 4);
			else if (y < mapHSV_V.height * 6/6f)
				c = Color.Lerp(Color.magenta, Color.red, y/(mapHSV_V.height / 6f) - 5);
			
			mapHSV_V.SetPixel (1, y, c);
		}
		
		mapHSV_V.Apply(true);
	}

	//---------------------------------------------------------------------------------------------------- Update Colormap
	// Update Color Map
	private void updateColorMap()
	{
		// switch mode
		switch(mode)
		{
		case 1:
			// Mode 1 Saturation
			for (int y = 0 ; y < map.height ; y++)
			{
				for (int x = 0 ; x < map.width ; x++)
				{
					int xFactor = (int)(mapHSV_H.width / (float)map.width * x);
					Color c = Color.Lerp(Color.white, mapHSV_H.GetPixels(xFactor, 0, 1, 1)[0], ftmpS);
					Color t = Color.Lerp(Color.black, c, (float)y/mapHSV_H.width);
					map.SetPixel (x, y, t);
				}
			}
			break;
			
		case 2:
			// Mode 2 Value
			for (int y = 0 ; y < map.height ; y++)
			{
				for (int x = 0 ; x < map.width ; x++)
				{
					int xFactor = (int)(mapHSV_H.width / (float)map.width * x);
					Color c = mapHSV_H.GetPixels(xFactor, 0, 1, 1)[0] * ftmpV;
					Color t = Color.Lerp(Color.white * ftmpV, c, (float)y / mapHSV_H.width);
					map.SetPixel (x, y, Color.black + t);
				}
			}
			break;
		case 3:
			// Mode 3  Red
			for (int y = 0 ; y < map.height ; y++)
			{
				for (int x = 0 ; x < map.width ; x++)
				{
					Color t = new Color(ftmpR, (float)y/map.height, (float)x/map.width);
					map.SetPixel (x, y, t);
				}
			}
			break;
		case 4:
			// Mode 4  Green
			for (int y = 0 ; y < map.height ; y++)
			{
				for (int x = 0 ; x < map.width ; x++)
				{
					Color t = new Color((float)y/map.height, ftmpG, (float)x/map.width);
					map.SetPixel (x, y, t);
				}
			}
			break;
		case 5:
			// Mode 5  Blue
			for (int y = 0 ; y < map.height ; y++)
			{
				for (int x = 0 ; x < map.width ; x++)
				{
					Color t = new Color((float)x/map.height, (float)y/map.width, ftmpB);
					map.SetPixel (x, y, t);
				}
			}
			break;
		case 6:
			// Mode 6  Pipette
			StartCoroutine(smallScreenshot(map, Input.mousePosition, 19, 19));
			break;
		default:
			// Mode 1 = default  H
			for (int y = 0 ; y < map.height ; y++)
			{
				for (int x = 0 ; x < map.width ; x++)
				{
					Color t = Color.Lerp(Color.white, selectedHColor, (float)x/map.width) * Color.Lerp(Color.black, Color.white, (float)y/map.width);
					map.SetPixel (x, y, t);
				}
			}
			break;
		}
		map.Apply( true );
		
		updateCursorInColorMap();
	}
	
	//---------------------------------------------------------------------------------------------------- Update S V R G B
	// Horizontal & Vertical Color Map Saturation
	private void updateMapS()
	{
		Color c = new Color(0,0,0);

		// Horizontal
		for (int x = 0 ; x < mapS_H.width  ; x++)
		{
			c = Color.Lerp(Color.white, selectedHColor, (float)x/mapS_H.width) * Color.Lerp(Color.black, Color.white, ftmpV);
			mapS_H.SetPixel (x, 1, c);
		}	
		mapS_H.Apply(true);

		// Vertical
		for (int x = 0 ; x < mapS_V.height  ; x++)
		{
			c = Color.Lerp(Color.white, selectedHColor, (float)x/mapS_V.height) * Color.Lerp(Color.black, Color.white, ftmpV);
			mapS_V.SetPixel (1, x, c);
		}	
		mapS_V.Apply(true);
	}
	
	// Horizontal & Vertical Color Map Value
	private void updateMapV()
	{
		Color c = new Color(0,0,0);

		// Horizontal
		for (int y = 0 ; y < mapV_H.width  ; y++)
		{
			c = Color.Lerp(Color.white, selectedHColor, ftmpS) * Color.Lerp(Color.black, Color.white, (float)y/mapV_H.width);
			mapV_H.SetPixel (y, 1, c);
		}
		mapV_H.Apply(true);

		// Vertical
		for (int y = 0 ; y < mapV_V.height  ; y++)
		{
			c = Color.Lerp(Color.white, selectedHColor, ftmpS) * Color.Lerp(Color.black, Color.white, (float)y/mapV_V.height);
			mapV_V.SetPixel (1, y, c);
		}	
		mapV_V.Apply(true);
	}
	
	// Horizontal & Vertical Color Map Red
	private void updateMapR()
	{
		Color c = new Color(0,0,0);
		Color c1 = new Color(0, selectedColor.g, selectedColor.b);
		Color c2 = new Color(1, selectedColor.g, selectedColor.b);

		// Horizontal
		for (int x = 0 ; x < mapR_H.width  ; x++)
		{
			
			c = Color.Lerp(c1, c2, (float)x/mapR_H.width);
			mapR_H.SetPixel (x, 1, c);
		}	
		mapR_H.Apply(true);

		// Vertical
		for (int x = 0 ; x < mapR_V.height ; x++)
		{
			c = Color.Lerp(c1, c2, (float)x/mapR_V.height);
			mapR_V.SetPixel (1, x, c);
		}	
		mapR_V.Apply(true);
	}
	
	// Horizontal & Vertical Color Map Green
	private void updateMapG()
	{
		Color c = new Color(0,0,0);
		Color c1 = new Color(selectedColor.r, 0, selectedColor.b);
		Color c2 = new Color(selectedColor.r, 1, selectedColor.b);

		// Horizontal
		for (int x = 0 ; x < mapG_H.width  ; x++)
		{
			c = Color.Lerp(c1, c2, (float)x/mapG_H.width);
			mapG_H.SetPixel (x, 1, c);
		}	
		mapG_H.Apply(true);

		// Vertical
		for (int x = 0 ; x < mapG_V.height ; x++)
		{
			c = Color.Lerp(c1, c2, (float)x/mapG_V.height);
			mapG_V.SetPixel (1, x, c);
		}	
		mapG_V.Apply(true);
	}
	
	// Horizontal & Vertical Color Map Blue
	private void updateMapB()
	{
		Color c = new Color(0,0,0);
		Color c1 = new Color(selectedColor.r, selectedColor.g, 0);
		Color c2 = new Color(selectedColor.r, selectedColor.g, 1);

		// Horizontal
		for (int x = 0 ; x < mapB_H.width  ; x++)
		{
			c = Color.Lerp(c1, c2, (float)x/mapB_H.width);
			mapB_H.SetPixel (x, 1, c);
		}	
		mapB_H.Apply(true);

		// Vertical
		for (int x = 0 ; x < mapB_V.height ; x++)
		{
			c = Color.Lerp(c1, c2, (float)x/mapB_V.height);
			mapB_V.SetPixel (1, x, c);
		}	
		mapB_V.Apply(true);
	}

	//---------------------------------------------------------------------------------------------------- Update Alpha preview colorline
	private void updateMapPreviewA()
	{
		// Horizontal
		for (int x = 0 ; x < tex_selectedAlpha.width ; x++)
		{
			for (int y = 0 ; y < tex_selectedAlpha.height ; y++)
			{
				if (x < ftmpA * tex_selectedAlpha.width)
					tex_selectedAlpha.SetPixel(x, y, Color.white);
				else
					tex_selectedAlpha.SetPixel(x, y, Color.black);
			}
		}	
		tex_selectedAlpha.Apply(true);
	}

	//---------------------------------------------------------------------------------------------------- Update all maps
	private void updateAll()
	{
		updateColorMap();
		updateMapV();
		updateMapS();
		updateMapR();
		updateMapG();
		updateMapB();
		updateMapPreviewA();
	}

	//---------------------------------------------------------------------------------------------------- Update Cursor
	// Draw Cursor in ColorMap
	private void updateCursorInColorMap()
	{
		float valX = 0;
		float valY = 0;
		
		// Switch the color mode
		switch(mode)
		{
		case 1:
			valX = ftmpH;
			valY = ftmpV;
			break;
		case 2:
			valX = ftmpH;
			valY = ftmpS;
			break;
		case 3:
			valX = ftmpB;
			valY = ftmpG;
			break;
		case 4:
			valX = ftmpB;
			valY = ftmpR;
			break;
		case 5:
			valX = ftmpR;
			valY = ftmpG;
			break;
		default:
			valX = ftmpS;
			valY = ftmpV;
			break;
		}
		
		// Draw Cursor
		for (int x = 0 ; x < map.width ; x++)
		{
			if (x % 4 == 0)
				map.SetPixel(x, (int)(valY * (map.height-1)), Color.white);
		}
		
		for (int y = 0 ; y < map.height ; y++)
		{
			if (y % 4 == 0)
				map.SetPixel((int)(valX * (map.width-1)), y, Color.white);
		}
		
		map.Apply( true );
	}

	//---------------------------------------------------------------------------------------------------- Screenshot for pipette mode
	private static IEnumerator smallScreenshot(Texture2D map, Vector3 pos, int xCount, int yCount)
	{
		// wait for graphics to render
		yield return new WaitForEndOfFrame();
		
		// calculate texture width and height
		int width = (int)Mathf.RoundToInt((map.width - (xCount + 2)) / (float)xCount);
		int height = (int)Mathf.RoundToInt((map.height - (yCount + 2)) / (float)yCount);
		
		// create a texture
		Texture2D texture = new Texture2D(xCount, yCount);
		
		// Read pixels on mouseposition
		int xxPos = (int)Mathf.Clamp((pos.x - xCount /2f), -5, Screen.width - xCount);
		int yyPos = (int)Mathf.Clamp((pos.y - yCount /2f), -5, Screen.height - yCount);
		
		texture.ReadPixels(new Rect(xxPos, yyPos, xCount, yCount), 0, 0);
		
		texture.Apply();
		
		// split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
		yield return 0;
		
		int xfactor = (int)Mathf.RoundToInt((map.width - 2)/ (float)xCount);
		int yfactor = (int)Mathf.RoundToInt((map.height - 2)/ (float)yCount);
		
		int xBorder = 1 + (int)(((float)map.width % (width * xCount + 2 + xCount -1))/2f);
		int yBorder = 1 + (int)(((float)map.height % (height * yCount + 2 + yCount -1))/2f);
		
		Color[] originalColor;
		originalColor = new Color[width*height];
		
		// Set all black
		for (int y = 0 ; y < map.height ; y++)
		{
			for (int x = 0 ; x < map.width ; x++)
			{
				if (x > Mathf.Round(map.width/2f - width/2f)-2 && x < Mathf.Round(map.width/2f + width/2f)+1 && 
				    y > Mathf.Round(map.height/2f - height/2f)-2 && y < Mathf.Round(map.height/2f + height/2f)+1)
					map.SetPixel(x, y, Color.white);
				else
					map.SetPixel(x, y, new Color(0, 0, 0, 0.3f));
			}
		}
		map.Apply();
		
		// Make Zoom
		for (int y = 0 ; y < yCount ; y++)
		{
			for (int x = 0 ; x < xCount ; x++)
			{
				for (int i = 0 ; i < width*height ; i++)
				{
					originalColor[i] = Color.black + texture.GetPixels(x, y, 1, 1)[0];
				}
				
				if ((x == Mathf.Round(xCount/2f)-1) && (y == Mathf.Round(yCount/2f)-1))
				{
					pipetteColor = originalColor[0];
				}
				
				map.SetPixels(xBorder + x * xfactor, yBorder + y * yfactor, width, height, originalColor);
			}
		}
		
		map.Apply();
		
	}
	
	//---------------------------------------------------------------------------------------------------- Screenshot for pipette mode
	public static IEnumerator mousePointScreenshot(Vector3 pos)
	{
		// wait for graphics to render
		yield return new WaitForEndOfFrame();

		Texture2D texture = new Texture2D(1, 1);
		// Read Pixels on mouse position
		texture.ReadPixels(new Rect(pos.x, pos.y, 1, 1), 0, 0);
		texture.Apply();
		
		// split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
		yield return 0;

		selCol = Color.black + texture.GetPixel(0, 0);
	}

	//---------------------------------------------------------------------------------------------------- HSV TO RGB calculator
	private Color HSVtoRGB(float h, float s, float v)
	{
		int i;
		float f, p, q, t;
		float r, g, b;

		if(s == 0) 
		{
			// achromatic (grey)
			r = g = b = v;
			return new Color(r, g, b);
		}
		h /= 60;			// sector 0 to 5
		i = (int)Mathf.Floor(h);
		f = h - i;			// factorial part of h
		p = v * (1 - s);
		q = v * (1 - s * f );
		t = v * (1 - s * (1 - f));

		switch( i ) 
		{
			case 0:
				r = v;
				g = t;
				b = p;
				break;
			case 1:
				r = q;
				g = v;
				b = p;
				break;
			case 2:
				r = p;
				g = v;
				b = t;
				break;
			case 3:
				r = p;
				g = q;
				b = v;
				break;
			case 4:
				r = t;
				g = p;
				b = v;
				break;
			default:		// case 5:
				r = v;
				g = p;
				b = q;
				break;
		}

		// Clamp the return values between 0 and 1
		return new Color(Mathf.Clamp(r, 0, 1),
		                 Mathf.Clamp(g, 0, 1),
		                 Mathf.Clamp(b, 0, 1));
	}

	//---------------------------------------------------------------------------------------------------- RGB To HSV calculator
	private HSVColor RGBtoHSV(float r, float g, float b)
	{
		float h, s, v;
		float min, max, delta;

		min = Mathf.Min(r, g, b);
		max = Mathf.Max(r, g, b);

		v = max;					// ----	v
		delta = max - min;

		if(max != 0)
		{
			s = delta / max;		// ----	s
		}
		else 
		{
			// r = g = b = 0		// s = 0, v is undefined
			s = ftmpS; 				// use previous s
			h = ftmpH; 				// use previous h
			return new HSVColor(h, s, v);
		}

		if(r == max)
		{
			h = (g - b) / delta;		// between yellow & magenta
		}
		else if(g == max)
		{
			h = 2 + (b - r) / delta;	// between cyan & yellow
		}
		else
		{
			h = 4 + (r - g) / delta;	// between magenta & cyan
		}

		h *= 60;						// degrees

		if(h < 0)
		{
			h += 360;
		}

		// If h is NAN set to 0
		if (float.IsNaN(h))
		{
			h = 0;
		}

		return new HSVColor(h, s, v);
	}

	//----------------------------------------------------------------------------------------------------
	static Texture2D tex = null;
	static Texture2D texAlpha = null;
	static Color selCol;
	public static GUISkin myGUISkin = null;
	public static GUISkin myOldGUISkin = null;
	public static Texture2D texPip = null;
	public static bool pipMode = false;
	public static int selectedIndex = 0;
	//---------------------------------------------------------------------------------------------------- GUI Menu Small
	public static void colorMenu(Material mat, int width, int height, int idx)
	{
		Color myColor = mat.color;

		myOldGUISkin = GUI.skin;

		if (myGUISkin == null)
		{
			myGUISkin = (GUISkin) Resources.Load("colorChooser/colorChooserSkin");
		}

		GUI.skin = myGUISkin;

		if (texPip == null)
		{
			texPip = (Texture2D) Resources.Load("colorChooser/pipette");
		}

		if (tex == null)
		{
			tex = new Texture2D(width, height - 3);
			texAlpha = new Texture2D(width, 3);

			for (int y = 0 ; y < tex.height ; y++)
			{
				for (int x = 0 ; x < tex.width ; x++)
				{
					tex.SetPixel(x, y, Color.white);

					if (y < texAlpha.height)
					{
						texAlpha.SetPixel(x, y, Color.white);
					}
				}
			}
			tex.Apply(true);
			texAlpha.Apply(true);
		}

		GUILayout.BeginHorizontal ();

		GUILayout.BeginVertical();

		Color ctmp = GUI.color;
		if (pipMode && selectedIndex == idx)
			GUI.color = selCol;
		else
			GUI.color = new Color(myColor.r, myColor.g, myColor.b);

		GUILayout.Label (tex, GUILayout.Width(tex.width), GUILayout.Height(tex.height));

		GUI.color = ctmp;

		for (int x = 0 ; x < texAlpha.width ; x++)
		{
			for (int y = 0 ; y < texAlpha.height ; y++)
			{
				if (x < myColor.a * texAlpha.width)
					texAlpha.SetPixel(x, y, Color.white);
				else
					texAlpha.SetPixel(x, y, Color.black);
			}
		}	
		texAlpha.Apply(true);

		GUILayout.Label (texAlpha, GUILayout.Width(texAlpha.width), GUILayout.Height(texAlpha.height));

		GUILayout.EndVertical();

		GUILayout.EndHorizontal ();

		GUI.skin = myOldGUISkin;

		if (Input.GetMouseButton(0) && idx == selectedIndex && pipMode == true)
		{
			mat.color = selCol;
		}
		else
		{
			mat.color = myColor;
		}
	}
}

//---------------------------------------------------------------------------------------------------- HSV Color Class
public class HSVColor
{
	private float h;
	private float s;
	private float v;
	
	public HSVColor(float h, float s, float v)
	{
		this.h = h;
		this.s = s;
		this.v = v;
	}

	public float H {
		get {
			return h;
		}
		set {
			h = value;
		}
	}

	public float S {
		get {
			return s;
		}
		set {
			s = value;
		}
	}

	public float V {
		get {
			return v;
		}
		set {
			v = value;
		}
	}
}