using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using FluffyUnderware.Curvy;
using System.Collections.Generic;

public class BookController : MonoBehaviour 
{
	private XmlDocument _xmlDocument;
	public float sensitivity = 0.1f;
	//private GameObject _debugLineLord;
	private Vector3 _offset = Vector3.zero;
	private float _lead = .02f;
	private float _yInput = 0.0f;
	private string[] _words; //this is an array of each word
	private List<string> _lines = new List<string>();
	private float _rise = 0.15f;

	private float lineLengthLimit = 50.0f;
	public float fontSizeMult = 0.07f;
	private float tracking = 0.3f;
	private bool _laidOutText;

	public void StartATL ( SplineLord_one splineLord_one ) 
	{


		LoadText (splineLord_one);
		//_debugLineLord = GameObject.Find ("DebugLineLord");
		//take scroll speed off the accelerometer
		_offset = new Vector3(0.00025f, 0.0f, 0.00025f); //offset 2nd text right and down from 1st
	}

	public void UpdateATL (PlayerController_one player_one, SplineLord_one splineLord_one) 
	{
		
//		if (GameObject.Find ("DebugLineLord") != null) 
//		{
//			_yInput += Input.mouseScrollDelta.y;
//			//print (_yInput);
//
//			_offset = new Vector3 (0.0f, _yInput * sensitivity, 0.0f);
//
//			//SplineLord splineLord = GameObject.FindObjectOfType< SplineLord > ();
//			CurvySpline curSpline = splineLord.GetSpline ();
//
//			float t = curSpline.GetNearestPointTF( player_one.transform.position ); //weird to get t back from the player 
//			_debugLineLord.transform.position = curSpline.Interpolate (t + _lead) + _debugLineLord.transform.up * _offset.y;
//
//			_debugLineLord.transform.rotation = Quaternion.Slerp( _debugLineLord.transform.rotation, curSpline.GetOrientationFast( t + _lead ), Time.deltaTime * 5.0f );
//
//		}

		if (splineLord_one.GetSpline ().IsInitialized && !_laidOutText) {
			float tLineSpacing = 1.0F / _lines.Count; // the height of a line
			_laidOutText = true;
			for (int i = 0; i < _lines.Count; i++) 
			{
				Vector3 upAtT = splineLord_one.GetSpline ().GetOrientationUpFast (tLineSpacing * i);
				Vector3 alongSplineAtT = splineLord_one.GetSpline ().Interpolate (tLineSpacing * ( i + 1.0f ) ) - splineLord_one.GetSpline ().Interpolate (tLineSpacing * i);
				Vector3 right = Vector3.Cross (alongSplineAtT, upAtT);
				Vector3 normal2 = Vector3.Cross (right, alongSplineAtT);

				Vector3 position = splineLord_one.GetSpline ().Interpolate (tLineSpacing * i) + upAtT * _rise + right * .3f;
				Vector3 normal = splineLord_one.GetSpline ().GetOrientationUpFast (tLineSpacing * i);
				normal = Quaternion.AngleAxis (-50.0f, right) * normal;

				AxKDebugLines.AddText (_lines [i], position, normal, right, upAtT, fontSizeMult, tracking, lineLengthLimit, Color.white, 9999999999999); 
				AxKDebugLines.AddText (_lines [i], position + _offset, normal, right, upAtT, fontSizeMult, tracking, lineLengthLimit, Color.white, 9999999999999); //to make the line thicker
				AxKDebugLines.AddText (_lines [i], position + _offset * 2.0f, normal, right, upAtT, fontSizeMult, tracking, lineLengthLimit, Color.white, 9999999999999); 
				AxKDebugLines.AddText (_lines [i], position + _offset * 3.0f, normal, right, upAtT, fontSizeMult, tracking, lineLengthLimit, Color.white, 9999999999999); 
				AxKDebugLines.AddText (_lines [i], position + _offset * 4.0f, normal, right, upAtT, fontSizeMult, tracking, lineLengthLimit, Color.white, 9999999999999); 

			}
		}
	}

	//note: 10-yr-old garbage collector in Unity will run randomly and halt code: boehem gc
	//solution: don't produce too much garbage OR allocate on stack versus heap OR clear the garbage often (which can also make issues)
	public void LoadText(SplineLord_one splineLord_one)
	{

		string text = "";
		//use markdown? for tags: find a markdown parser 
		text = System.IO.File.ReadAllText (Application.dataPath + "/Text/TextTest.xml");

		_xmlDocument = new XmlDocument ();
		_xmlDocument.LoadXml (text);

		_words = text.Split (' ', '\t', System.Environment.NewLine.ToCharArray()[ 0 ] );

		System.Text.StringBuilder line = new System.Text.StringBuilder ();

		for (int i = 0; i < _words.Length; i++) 
		{
			if (line.Length + _words [i].Length + 1 > lineLengthLimit)
			{
				_lines.Add (line.ToString ());
				line.Remove (0, line.Length);
			}

			line.Append (_words[ i ] + " " );

			//print (_words[ i ]);
			if (_words [i] == "{testing}") //finds tag in text 
			{
				print ("lol");
			}
		}
			

		//if use <>= in text, use special character to escape that code
		//XmlNodeList pageBreak = _xmlDocument.GetElementsByTagName ("page");
		//child of page is text or whatever 
		//monospace font so you can specify the line width 

		//AxKDebugLines.AddText parameters: string text, Vector3 position, Vector3 normal, float size, float length, Color color, float life = 0.0f
		//to put in the whole chapter
		//AxKDebugLines.AddText( pageBreak[0].ChildNodes[0].InnerText, transform.position, fontSizeMult, lineLengthLimit, Color.white, 9999999999999 );

	}
}
