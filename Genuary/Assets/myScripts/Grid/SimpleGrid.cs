using UnityEngine;

public class SimpleGrid {

    private int _width;
    private int _height;
    private float _cellSize;
    private float _thickness;
    private Vector3 _origin;
    private int[ , ] _gridArray;

    public SimpleGrid( int width, int height, float size, float thickness, Vector3 origin )
        {
            _width = width;
            _height = height;
            _cellSize = size;
            _thickness = thickness;
            _gridArray = new int[ width, height ];
            _origin = origin;

            for ( int x = 0; x < _gridArray.GetLength( 0 ); x++ ) {
                for ( int y = 0; y < _gridArray.GetLength( 1 ); y++ ) {
                    
                }
            }
        }

    public int GetHeight => _height;

    public int GetWidth => _width;

    private Vector3 GetPosition( int x, int y )
        {
            return new Vector3( x, y ) * _cellSize + _origin;
        }

    private float GetThicknessDist( float thickness )
        {
            return Mathf.Sqrt( thickness * thickness / 2f );
        }

    public void GetXY( Vector3 pos, out int x, out int y )
        {
            x = Mathf.FloorToInt( pos.x - _origin.x / _cellSize );
            y = Mathf.FloorToInt( pos.y - _origin.y / _cellSize );
        }

    public void SetValue( Vector3 pos, int value )
        {
            GetXY( pos, out var x, out var y );
            SetValue( x, y, value );
        }

    public void SetValue( int x, int y, int value )
        {
            if ( x >= 0 && y >= 0 && x < _width && y < _height ) {
                _gridArray[ x, y ] = value;
            }
        }

    public int GetValue( int x, int y )
        {
            int value = 0;
            if ( x >= 0 && y >= 0 && x < _width && y < _height ) {
                value = _gridArray[ x, y ];
            }
            return value;
        }

    public int GetValue( Vector3 pos )
        {
            GetXY( pos, out var x, out var y );
            int value = 0;
            if ( x >= 0 && y >= 0 && x < _width && y < _height ) {
                value = _gridArray[ x, y ];
            }
            return value;
        }

}