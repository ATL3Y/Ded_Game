using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AxKLine
{
	public Vector3 pointOne;
	public Vector3 pointTwo;
	
	public AxKLine( Vector3 pointOne_INPUT, Vector3 pointTwo_INPUT )
	{
		pointOne = pointOne_INPUT;
		pointTwo = pointTwo_INPUT;
	}
}

public class AxKShape
{
	public List< AxKLine > lines = new List< AxKLine >();
	public float life = 0.0f;
	public Color color;
}

static public class AxKDebugLines
{
	public static AxKDebugLineManager m_manager;
	
	public static void AddLine( Vector3 pointOne, Vector3 pointTwo, Color color, float life = 0.0f )
	{
		AxKShape shape = Add( color, life );
		shape.lines.Add( new AxKLine( pointOne, pointTwo ) );
		m_manager.AddShape( shape );
	}
	
	public static void AddRay( Vector3 position, Vector3 direction, Color color, float life = 0.0f )
	{
		AxKShape shape = Add( color, life );
		shape.lines.Add( new AxKLine( position, position + direction ) );
		m_manager.AddShape( shape );
	}

    public static void AddRayField( Vector3 p1, Vector3 p2, Vector3 d1, Vector3 d2, Color c1, Color c2, float steps, float life = 0.0f )
    {
        float len = d1.magnitude * 100.0f;
        d1 = d1.normalized;
        d2 = d2.normalized;
        for ( float i = 0; i < steps; i++ )
        {
            float p = i / steps;
            Vector3 position = ( p1 * p + p2 * ( 1.0f - p ) );// *0.5f;
            AddLine(
                position,
                position + ( ( d1 * p + d2 * ( 1.0f - p ) ) * 0.5f ) * len,
                c1, life
            );
        }
    }
	
	private static void MakeCircle( ref AxKShape shape, Vector3 center, float radius, Vector3 normal )
	{
		normal = normal.normalized;
		Vector3 perp = Vector3.forward;
		if( Vector3.Magnitude( perp - normal ) < 0.01f ) { perp = Vector3.right; }
		perp = Vector3.Cross( perp, normal ).normalized;
		
		int numLines = 16;
		float angle = 0.0f;
		Vector3 point = center + perp * radius;
		
		for( int i = 0; i < numLines; i++ )
		{
			angle += 2.0f * Mathf.PI / ( float )numLines;
			Vector3 newPoint = center + ( Quaternion.AngleAxis( Mathf.Rad2Deg * angle, normal ) * perp ) * radius;
			shape.lines.Add( new AxKLine( point, newPoint ) );
			point = newPoint;        
		}
	}
	
	public static void AddCircle( Vector3 center, float radius, Vector3 normal, Color color, float life = 0.0f )
	{
		AxKShape shape = Add( color, life );
		MakeCircle( ref shape, center, radius, normal );
		m_manager.AddShape( shape );
	}
	
	public static void AddSphere( Vector3 center, float radius, Color color, float life = 0.0f )
	{
		AxKShape shape = Add( color, life );
		MakeCircle( ref shape, center, radius, Vector3.right );
		MakeCircle( ref shape, center, radius, Vector3.up );
		MakeCircle( ref shape, center, radius, Vector3.forward );
		m_manager.AddShape( shape );
	}
	
	public static void AddMesh( Transform transform, Mesh mesh, Color color, float life = 0.0f )
	{
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		AxKShape shape = Add( color, life );
		
		for ( int i = 0; i < triangles.Length / 3; i++ )
		{
			Vector3 pointOne = transform.TransformPoint( vertices[ triangles[ i * 3 ] ] );
			Vector3 pointTwo = transform.TransformPoint( vertices[ triangles[ i * 3 + 1 ] ] );
			Vector3 pointThree = transform.TransformPoint( vertices[ triangles[ i * 3 + 2 ] ] );
			//Vector3 pointOne = vertices[ triangles[ i * 3 ] ];
			//Vector3 pointTwo = vertices[ triangles[ i * 3 + 1 ] ];
			//Vector3 pointThree = vertices[ triangles[ i * 3 + 2 ] ];
			
			shape.lines.Add( new AxKLine( pointOne, pointTwo ) );
			shape.lines.Add( new AxKLine( pointTwo, pointThree ) );
			shape.lines.Add( new AxKLine( pointThree, pointOne ) );
		}
		
		m_manager.AddShape( shape );
	}

    public static void AddScreenSpaceRect( Vector3 center, Vector3 extents, Color color, float life = 0.0f )
    {
        AxKShape shape = Add ( color, life );
        Camera cam = Camera.main;
		
		Vector3 c = cam.ScreenToWorldPoint( center );
        AxKDebugLines.AddFancySphere(c, 0.5f, Color.red);
		Vector3 u1 = cam.ScreenToWorldPoint( cam.transform.right * extents.x );
		Vector3 u2 = cam.ScreenToWorldPoint( cam.transform.up * extents.y );
		
		Vector3[] points = new Vector3[ 4 ];
		points[ 0 ] = c + u1 + u2;
		points[ 1 ] = c + u1 - u2;
		points[ 2 ] = c - u1 - u2;
		points[ 3 ] = c - u1 + u2;
		
		shape.lines.Add( new AxKLine( points[ 0 ], points[ 1 ] ) );
		shape.lines.Add( new AxKLine( points[ 1 ], points[ 2 ] ) );
		shape.lines.Add( new AxKLine( points[ 2 ], points[ 3 ] ) );
		shape.lines.Add( new AxKLine( points[ 3 ], points[ 0 ] ) );
		
		m_manager.AddShape( shape );
    }
	
	public static void AddText( string text, Vector3 position, Vector3 normal, Vector3 right, Vector3 up, float size, float tracking, float length, Color color, float life = 0.0f )
	{
		MakeText( text, position, normal, right, up, size, tracking, length, color, life );
	}
	
	public static void AddText( string text, Vector3 position, Vector3 right, Vector3 up, float size, float tracking, float length, Color color, float life = 0.0f )
	{
		//MakeText( text, position, Vector3.Normalize( position - ( Camera.current != null ? Camera.current.transform.position : Camera.main.transform.position ) ) * -1, size, color, life );
		MakeText(text, position, Vector3.Normalize(position - Camera.main.transform.position) * -1, right, up, size, tracking, length, color, life);
	}
	
	delegate void CreateAxKDebugChar( char name, params int[] points );

	private static void MakeText( string text, Vector3 position, Vector3 normal, Vector3 right, Vector3 up, float size, float tracking, float length, Color color, float life = 0.0f )
	{
		string textOriginal = text; //create copy of original to retain uppercase info	
		text = text.ToLower();
		if ( AxKDebugChars.Count == 0  )
		{
			CreateAxKDebugChar q = delegate( char name, int[] points ) 
			{ 
				AxKDebugChars.Add( new AxKDebugChar( (int)name ) );
				AxKDebugChars[ AxKDebugChars.Count - 1 ].points.AddRange( points );
			};

			//this is a grid of 9. could expand this to a higher res grid
			q( '0', 0, 2, 2, 8, 8, 6, 6, 0, 2, 6 );
			q( '1', 1, 7 );
			q( '2', 0, 2, 2, 5, 5, 3, 3, 6, 6, 8 );
			q( '3', 0, 2, 2, 8, 8, 6, 5, 3 );
			q( '4', 0, 3, 3, 5, 2, 8 );
			q( '5', 2, 0, 0, 3, 3, 5, 5, 8, 8, 6 );
			q( '6', 2, 0, 0, 6, 6, 8, 8, 5, 5, 3 );
			q( '7', 0, 2, 2, 8 );
			q( '8', 0, 6, 2, 8, 0, 2, 3, 5, 6, 8 );
			q( '9', 6, 8, 8, 2, 2, 0, 0, 3, 3, 5 );
			q( 'a', 6, 0, 0, 2, 2, 8, 3, 5 );
			q( 'b', 0, 2, 6, 8, 1, 7, 2, 8, 4, 5 );
			q( 'c', 0, 2, 0, 6, 6, 8 );
			q( 'd', 0, 2, 6, 8, 1, 7, 2, 8 );
			q( 'e', 0, 6, 0, 2, 3, 5, 6, 8 );
			q( 'f', 0, 6, 0, 2, 3, 5 );
			q( 'g', 2, 0, 0, 6, 6, 8, 8, 5, 5, 4 );
			q( 'h', 0, 6, 2, 8, 3, 5 );
			q( 'i', 0, 2, 1, 7, 6, 8 );
			q( 'j', 2, 8, 8, 6, 6, 3 );
			q( 'k', 0, 6, 2, 3, 3, 8 );
			q( 'l', 0, 6, 6, 8 );
			q( 'm', 6, 0, 0, 4, 4, 2, 2, 8 );
			q( 'n', 6, 0, 0, 8, 8, 2 );
			q( 'o', 0, 2, 2, 8, 8, 6, 6, 0 );
			q( 'p', 6, 0, 0, 2, 2, 5, 5, 3 );
			q( 'q', 0, 2, 2, 8, 8, 6, 6, 0, 8, 4 );
			q( 'r', 6, 0, 0, 2, 2, 5, 5, 3, 3, 8 );
			q( 's', 2, 0, 0, 3, 3, 5, 5, 8, 8, 6 );
			q( 't', 0, 2, 1, 7 );
			q( 'u', 0, 6, 6, 8, 8, 2 );
			q( 'v', 0, 7, 7, 2 );
			q( 'w', 0, 6, 6, 4, 4, 8, 8, 2 );
			q( 'x', 0, 8, 2, 6 );
			q( 'y', 0, 3, 3, 5, 2, 8, 8, 6 );
			q( 'z', 0, 2, 2, 6, 6, 8 );
			q( '.', 4, 7 );
			q( ',', 7, 5 );
			q( '(', 1, 3, 3, 7 );
			q( ')', 1, 5, 5, 7 );
			q( '-', 3, 5 );
		}
		
		AxKShape shape = Add( color, life );
		
		normal = normal.normalized;
		Vector3 perp = Vector3.forward;
		if ( Vector3.Magnitude( perp - normal ) < 0.01f ) { perp = Vector3.right; }
		perp = Vector3.Cross( perp, normal ).normalized;
		
		// Debug.Log( position );
		Vector3 currentPosition = position - Camera.main.transform.right * ( ( size * 0.5f + size * tracking ) * ( text.Length / 2.0f ) );
		//Vector3 currentPosition = position - right * ( ( size * 0.5f + size * tracking ) * ( text.Length / 2.0f ) );

		//set position and currentPosition at position - cam.right * length / 2 rather than entire line length/2
		position = position - Camera.main.transform.right * ( size * 0.5f + size * tracking ) * length / 2.0f;
		//position = position - right * ( size * 0.5f + size * tracking ) * length / 2.0f;

		currentPosition = position;
		mathIsHardSometimes.transform.position = currentPosition;


		//AxKDebugLines.AddRay (position, normal, Color.cyan, 999);
		mathIsHardSometimes.transform.LookAt( currentPosition + normal );
		
		for ( int i = 0; i < text.Length; i++ )
		{
			float sizeOriginal = size;
			Vector3 offset = new Vector3(0, 0, 0);

			if (textOriginal [i] != text [i]) 
			{
				size *= 1.5f;
				offset = up * .041f;
			} 
			
			int temp = (int)text[ i ];
			if ( temp != 32 )
			{
				AxKDebugChar debugChar = null;
				for ( int j = 0; j < AxKDebugChars.Count; j++ ) { if ( AxKDebugChars[ j ].name == temp ) debugChar = AxKDebugChars[ j ]; }

				if (debugChar == null) 
				{
					//Debug.Log (text [i]);
					continue;
				}
				for ( int k = 0; k < debugChar.points.Count / 2; k++ )
				{
					//shape.lines.Add( new AxKLine( currentPosition + mathIsHardSometimes.transform.TransformDirection( charVerts[ debugChar.points[ k * 2 + 0 ] ] * size ),
					                             //currentPosition + mathIsHardSometimes.transform.TransformDirection( charVerts[ debugChar.points[ k * 2 + 1 ] ] * size ) ) );
				
					shape.lines.Add( new AxKLine( offset + currentPosition + mathIsHardSometimes.transform.TransformDirection( charVerts[ debugChar.points[ k * 2 + 0 ] ] * size ),
						offset + currentPosition + mathIsHardSometimes.transform.TransformDirection( charVerts[ debugChar.points[ k * 2 + 1 ] ] * size ) ) );
				}
			}
			right = right.normalized;
			//currentPosition += Camera.main.transform.right * ( ( size * 0.5f + size * tracking ) ); //HERE!!
			currentPosition += right * ( ( size * 0.5f + size * tracking ) ); //Tweak line direction here!
		
			mathIsHardSometimes.transform.position = currentPosition;
			mathIsHardSometimes.transform.LookAt( currentPosition + normal );
			//AxKDebugLines.AddRay (position, normal, Color.yellow, 999);

			//currentPosition = currentPositionOriginal;
			size = sizeOriginal;
		}
		
		m_manager.AddShape( shape );
	}
	
	private static GameObject mathIsHardSometimes = new GameObject();
	private static Vector3[] charVerts = new Vector3[ 9 ] { new Vector3( 0.25f, 0.5f, 0.0f ), new Vector3( 0.0f, 0.5f, 0.0f ), new Vector3( -.25f, 0.5f, 0.0f ),
		new Vector3( 0.25f, 0.0f, 0.0f ), new Vector3( 0.0f, 0.0f, 0.0f ), new Vector3( -0.25f, 0.0f, 0.0f ),
		new Vector3( 0.25f, -0.5f, 0.0f ), new Vector3( 0.0f, -0.5f, 0.0f ), new Vector3( -0.25f, -0.5f, 0.0f ) };
	
	private static List<AxKDebugChar> AxKDebugChars = new List<AxKDebugChar>();
	private class AxKDebugChar
	{
		public int name;
		public List< int > points;
		
		public AxKDebugChar( int name_INPUT )
		{
			name = name_INPUT;
			points = new List<int>();
		}
	}
	
	public static void four20( Vector3 center, float radius, Vector3 normal, float life = 0.0f )
	{
		AxKShape shape = Add ( Color.green, life );
		
		normal = normal.normalized;
		Vector3 perp = Vector3.forward;
		if( Vector3.Magnitude( perp - normal ) < 0.01f ) { perp = Vector3.right; }
		perp = Vector3.Cross( perp, normal ).normalized;
		
		mathIsHardSometimes.transform.position = center;
		mathIsHardSometimes.transform.LookAt( center + normal );
		
		Vector3 a1 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( 0, .517f, 0 ) ) * radius;
		Vector3 a2 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .097f, .245f, 0 ) ) * radius;
		Vector3 a3 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( 0, .01f, 0 ) ) * radius;
		Vector3 a4 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.097f, .245f, 0 ) ) * radius;
		shape.lines.Add ( new AxKLine( a1, a2 ) );
		shape.lines.Add ( new AxKLine( a3, a2 ) );
		shape.lines.Add ( new AxKLine( a1, a4 ) );
		shape.lines.Add ( new AxKLine( a3, a4 ) );
		
		Vector3 b1 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .348f, .355f, 0 ) ) * radius;
		Vector3 b2 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .231f, .153f, 0 ) ) * radius;
		Vector3 b3 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .008f, .006f, 0 ) ) * radius;
		Vector3 b4 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .148f, .226f, 0 ) ) * radius;
		shape.lines.Add ( new AxKLine( b1, b2 ) );
		shape.lines.Add ( new AxKLine( b3, b2 ) );
		shape.lines.Add ( new AxKLine( b1, b4 ) );
		shape.lines.Add ( new AxKLine( b3, b4 ) );
		
		Vector3 c1 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.348f, .355f, 0 ) ) * radius;
		Vector3 c2 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.231f, .153f, 0 ) ) * radius;
		Vector3 c3 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.008f, .006f, 0 ) ) * radius;
		Vector3 c4 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.148f, .226f, 0 ) ) * radius;
		shape.lines.Add ( new AxKLine( c1, c2 ) );
		shape.lines.Add ( new AxKLine( c3, c2 ) );
		shape.lines.Add ( new AxKLine( c1, c4 ) );
		shape.lines.Add ( new AxKLine( c3, c4 ) );
		
		Vector3 d1 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .410f, .098f, 0 ) ) * radius;
		Vector3 d2 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .248f, .011f, 0 ) ) * radius;
		Vector3 d3 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .010f, -0.002f, 0 ) ) * radius;
		Vector3 d4 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .23f, .108f, 0 ) ) * radius;
		shape.lines.Add ( new AxKLine( d1, d2 ) );
		shape.lines.Add ( new AxKLine( d3, d2 ) );
		shape.lines.Add ( new AxKLine( d1, d4 ) );
		shape.lines.Add ( new AxKLine( d3, d4 ) );
		
		Vector3 e1 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.410f, .098f, 0 ) ) * radius;
		Vector3 e2 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.248f, .011f, 0 ) ) * radius;
		Vector3 e3 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.010f, -0.002f, 0 ) ) * radius;
		Vector3 e4 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.23f, .108f, 0 ) ) * radius;
		shape.lines.Add ( new AxKLine( e1, e2 ) );
		shape.lines.Add ( new AxKLine( e3, e2 ) );
		shape.lines.Add ( new AxKLine( e1, e4 ) );
		shape.lines.Add ( new AxKLine( e3, e4 ) );
		
		Vector3 f1 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .249f, -.092f, 0 ) ) * radius;
		Vector3 f2 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .119f, -.072f, 0 ) ) * radius;
		Vector3 f3 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .009f, -0.012f, 0 ) ) * radius;
		Vector3 f4 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( .129f, -.032f, 0 ) ) * radius;
		shape.lines.Add ( new AxKLine( f1, f2 ) );
		shape.lines.Add ( new AxKLine( f3, f2 ) );
		shape.lines.Add ( new AxKLine( f1, f4 ) );
		shape.lines.Add ( new AxKLine( f3, f4 ) );
		
		Vector3 g1 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.249f, -.092f, 0 ) ) * radius;
		Vector3 g2 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.119f, -.072f, 0 ) ) * radius;
		Vector3 g3 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.009f, -0.012f, 0 ) ) * radius;
		Vector3 g4 = center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( -.129f, -.032f, 0 ) ) * radius;
		shape.lines.Add ( new AxKLine( g1, g2 ) );
		shape.lines.Add ( new AxKLine( g3, g2 ) );
		shape.lines.Add ( new AxKLine( g1, g4 ) );
		shape.lines.Add ( new AxKLine( g3, g4 ) );
		
		shape.lines.Add ( new AxKLine( center + new Vector3( 0, 0, 0 ) * radius, center + mathIsHardSometimes.transform.InverseTransformDirection( new Vector3( 0, -.18f, 0 ) ) * radius ) );
		
		
		m_manager.AddShape( shape );
	}
	
	public static void AddFancySphere( Vector3 center, float radius, Color color, float life = 0.0f )
	{
		AxKShape shape = Add( color, life );
		MakeCircle( ref shape, center, radius, Vector3.right );
		MakeCircle( ref shape, center, radius, Vector3.up );
		MakeCircle( ref shape, center, radius, Vector3.forward );
		MakeCircle( ref shape, center, radius, Vector3.right + Vector3.up );
		MakeCircle( ref shape, center, radius, Vector3.right + Vector3.forward );
		MakeCircle( ref shape, center, radius, Vector3.forward + Vector3.up );
		MakeCircle( ref shape, center, radius, Vector3.right - Vector3.up );
		MakeCircle( ref shape, center, radius, Vector3.right - Vector3.forward );
		MakeCircle( ref shape, center, radius, Vector3.forward - Vector3.up );
		m_manager.AddShape( shape );
	}
	
	public static void AddCapsule( Vector3 start, Vector3 end, float radius, Color color, float life = 0.0f )
	{
		AxKShape shape = Add ( color, life );
		MakeCircle( ref shape, start, radius, Vector3.right );
		MakeCircle( ref shape, start, radius, Vector3.up );
		MakeCircle( ref shape, start, radius, Vector3.forward );
		MakeCircle( ref shape, end, radius, Vector3.right );
		MakeCircle( ref shape, end, radius, Vector3.up );
		MakeCircle( ref shape, end, radius, Vector3.forward );
		MakeCylinder( ref shape, start, end, radius );
		m_manager.AddShape( shape );
	}
	
	public static void AddCylinder( Vector3 start, Vector3 end, float radius, Color color, float life = 0.0f )
	{
		AxKShape shape = Add ( color, life );
		MakeCylinder( ref shape, start, end, radius );
		m_manager.AddShape( shape );
	}
	
	private static void MakeCylinder( ref AxKShape shape, Vector3 start, Vector3 end, float radius  )
	{
		Vector3 startToEnd = Vector3.Normalize( end - start );
		Vector3 perp = radius * Vector3.Normalize( OrthoVector( startToEnd ) );
		MakeCircle( ref shape, start, radius, startToEnd );
		MakeCircle( ref shape, end, radius, startToEnd );
		
		int numLines = 12;
		for ( int i = 0; i < numLines; i++ )
		{
			Vector3 rotPerp = Quaternion.AngleAxis( ( i * ( Mathf.PI * 2 ) / ( float ) numLines ) * Mathf.Rad2Deg, startToEnd ) * perp;
			shape.lines.Add ( new AxKLine( start + rotPerp, end + rotPerp ) );
		}
	}
	
	private static Vector3 OrthoVector( Vector3 vector )
	{
		return Vector3.Cross( vector, vector.x < 0.001f ? Vector3.right : Vector3.up );
	}
	
	public static void AddBounds( Bounds bounds, Color color, float life = 0.0f )
	{
		AddBoundingBox( bounds.center, bounds.extents, color, life );
	}
	
	public static void AddBox( Vector3 position, Vector3 size, Color color, float life = 0.0f )
	{
		AddBoundingBox( position, size / 2.0f, color, life );
	}

    public static void AddBox( float minX, float maxX, float minY, float maxY, float minZ, float maxZ, Color color, float life = 0.0f )
    {
        Vector3 position = new Vector3( ( minX + maxX ) / 2.0f, ( minY + maxY ) / 2.0f, ( minZ + maxZ ) / 2.0f );
        Vector3 size = new Vector3(Mathf.Abs(maxX - position.x), Mathf.Abs(maxY - position.y), Mathf.Abs(maxZ - position.z));
        AddBoundingBox(position, size, color, life);
    }
	
	private static void AddBoundingBox( Vector3 position, Vector3 extents, Color color, float life )
	{
		AxKShape shape = Add ( color, life );
		
		Vector3 c = position;
		Vector3 u0 = Vector3.forward * extents.z;
		Vector3 u1 = Vector3.right * extents.x;
		Vector3 u2 = Vector3.up * extents.y;
		
		Vector3[] points = new Vector3[ 8 ];
		points[ 0 ] = c + u0 + u1 + u2;
		points[ 1 ] = c - u0 + u1 + u2;
		points[ 2 ] = c - u0 - u1 + u2;
		points[ 3 ] = c + u0 - u1 + u2;
		points[ 4 ] = c + u0 + u1 - u2;
		points[ 5 ] = c - u0 + u1 - u2;
		points[ 6 ] = c - u0 - u1 - u2;
		points[ 7 ] = c + u0 - u1 - u2;
		
		shape.lines.Add( new AxKLine( points[ 0 ], points[ 1 ] ) );
		shape.lines.Add( new AxKLine( points[ 1 ], points[ 2 ] ) );
		shape.lines.Add( new AxKLine( points[ 2 ], points[ 3 ] ) );
		shape.lines.Add( new AxKLine( points[ 3 ], points[ 0 ] ) );
		
		shape.lines.Add( new AxKLine( points[ 4 ], points[ 5 ] ) );
		shape.lines.Add( new AxKLine( points[ 5 ], points[ 6 ] ) );
		shape.lines.Add( new AxKLine( points[ 6 ], points[ 7 ] ) );
		shape.lines.Add( new AxKLine( points[ 7 ], points[ 4 ] ) );
		
		shape.lines.Add( new AxKLine( points[ 4 ], points[ 0 ] ) );
		shape.lines.Add( new AxKLine( points[ 5 ], points[ 1 ] ) );
		shape.lines.Add( new AxKLine( points[ 6 ], points[ 2 ] ) );
		shape.lines.Add( new AxKLine( points[ 7 ], points[ 3 ] ) );
		
		m_manager.AddShape( shape );
	}
	
	private static void CheckCoroutine()
	{
		if ( m_manager == null )
		{
			GameObject gameObject = new GameObject();
			m_manager = gameObject.AddComponent< AxKDebugLineManager >();
			m_manager.AxKStart();
		}

		if ( mathIsHardSometimes == null )
		{
			mathIsHardSometimes = new GameObject();
		}
	}
	
	private static AxKShape Add( Color color, float life )
	{
		CheckCoroutine();
		
		AxKShape shape = new AxKShape();
		shape.color = color;
		shape.life = life;
		
		return shape;
	}
}

public class AxKDebugLineManager : MonoBehaviour
{
	List< AxKShape > shapes = new List< AxKShape >();
	List< Vector3 > verts = new List< Vector3 >();
	List< Color > colors = new List< Color >();
	
	public void AddShape( AxKShape shape )
	{
		shapes.Add( shape );
	}
	
	public void AxKStart()
	{
		gameObject.AddComponent< MeshFilter >();
		gameObject.AddComponent< MeshRenderer >();
		gameObject.name = "DebugLineLord";
		

#pragma warning disable 0618
		Material newMaterial = new Material (Shader.Find ("AxKDebugLines"));
        GetComponent<Renderer>().material = newMaterial;
#pragma warning restore 0618
	}
	
	public void LateUpdate()
	{
		ClearMesh();
		verts.Clear();
		colors.Clear();
		
		for( int i = 0; i < shapes.Count; i++ )
		{
			RenderShape( shapes[ i ] );
			shapes[ i ].life -= Time.deltaTime;
			if ( shapes[ i ].life <= 0.0f )
			{
				shapes.Remove( shapes[ i ] );
				i--;
			}
		}
		
		MakeMesh();


	}
	
	private void RenderShape( AxKShape shape )
	{
		foreach ( AxKLine line in shape.lines )
		{
			RenderLine( line, shape.color );
		}
	}
	
	private void RenderLine( AxKLine line, Color color )
	{
		if ( verts.Count > 64990 ) return;
		
		verts.Add( line.pointOne );
		verts.Add( line.pointTwo );
		
		colors.Add( color );
		colors.Add( color );
	}
	
	private void MakeMesh()
	{
		Mesh mesh = GetComponent< MeshFilter >().mesh;
		
		int[] triangles = new int[ verts.Count ];
		for ( int i = 0; i < triangles.Length; i++ )
		{
			triangles[ i ] = i;
		}
		
		mesh.vertices = verts.ToArray();
		mesh.SetIndices( triangles, MeshTopology.Lines, 0 );
		mesh.colors = colors.ToArray();
	}
	
	private void ClearMesh()
	{
		Mesh mesh = GetComponent< MeshFilter >().mesh;
		mesh.Clear();
	}
}