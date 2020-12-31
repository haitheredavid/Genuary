using UnityEngine;

public class PointsToGraph : MonoBehaviour {

    public int xMin = 1;
    public float stepSize = 2f;
    public string testId;

    [Range( 0, 100 )]
    public int pointCount = 10;
    [Range( 0.00f, 1.00f )]
    public float maskedPointCount;

    public GraphName function;
    public Material graphMaterial;

    private LineRenderer _line;

    private const float Tolerance = 0.001f;
    private GameObject[ ] _points;
    private int PointCount => _points.Length;
    private int _maskedCount;

    private void Awake( )
        {
            // set count as grid  
            _points = new GameObject[ pointCount ];
            // check the local tolerance 
            _maskedCount = CheckMaskedCount( PointCount );

            // create prefab for scene 
            var prefab = GameObject.CreatePrimitive( PrimitiveType.Cube );
            prefab.GetComponent<MeshRenderer>( ).material = graphMaterial;

            // get game object points
            _points = ResultsAsPrefab( _maskedCount, transform, stepSize, prefab );
        }

    private void Update( )
        {
            float time = Time.time;
            var f = GraphLibrary.GetFunction( function );

            var step = 2f / _maskedCount;
            for ( int i = 0, z = 0; z < _maskedCount; z++ ) {
                float v = ( z + 0.5f ) * step - 1f;
                for ( int x = 0; x < _maskedCount; x++, i++ ) {
                    float u = ( x + 0.5f ) * step - 1f;
                    _points[ i ].transform.localPosition = f( u, v, time );
                }
            }
        }

    private int CheckMaskedCount( int pointCount )
        {
            int maskedPoints = 0;
            // masked count is set to 0, so show all points
            if ( maskedPointCount <= Tolerance )
                maskedPoints = pointCount;
            else if ( maskedPointCount > Tolerance ) // if within range of slider 
                maskedPoints = Mathf.RoundToInt( pointCount * maskedPointCount );
            else if ( maskedPointCount <= 1f ) // if value is 1 mask all points  
                maskedPoints = 0;
            return maskedPoints;
        }
    private static GameObject[ ] ResultsAsPrefab( int count, Transform parent, float sSize, GameObject prefab )
        {
            // list of new points 
            GameObject[ ] points = new GameObject[ count ];
            // step size over max count
            var step = sSize / count;

            Vector3 scale = Vector3.one * step;
            Vector3 position = new Vector3( );

            for ( int index = 0, z = 0; z < count; z++ ) {
                position.z = ( z + 0.5f ) * step - 1f;
                for ( int x = 0; x < count; x++, index++ ) {
                    // create instance
                    GameObject point = Instantiate( prefab, parent, false );
                    point.name = "Point Instance " + index;

                    // graph position
                    position.x = ( x + 0.5f ) * step - 1f;
                    position.y = position.x;

                    // assign values
                    point.transform.localPosition = position;
                    point.transform.localScale = scale;
                    points[ index ] = point;
                }
            }

            Destroy( prefab );
            return points;
        }
    private void ResultsAsLine( )
        {
            // if ( _line != null )
            //     Destroy( _line );
            //
            // _line = gameObject.AddComponent<LineRenderer>( );
            //
            // Get.SplicedResults( testId, 1, out var points, out var values, out var maxValues );
            //
            // var tempCount = Mathf.RoundToInt( points.Count * 0.01f );
            // _line.positionCount = tempCount;
            //
            // var positions = new Vector3[ tempCount ];
            // var step = stepSize / positions.Length;
            // var y = 0f;
            // var max = maxValues[ 0 ];
            // // go through each value and map the value change on y 
            // for ( int i = 0; i < tempCount; i++ ) {
            //     if ( values.TryGetValue( (uint) i, out var val ) )
            //         if ( val != null )
            //             y = (float) val[ 0 ] / max;
            //
            //     positions[ i ] = new Vector3(
            //         i * step - xMin,
            //         y,
            //         0 );
            // }
            // _line.SetPositions( positions );
        }

}