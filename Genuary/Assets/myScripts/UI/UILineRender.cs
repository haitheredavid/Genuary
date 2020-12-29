using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Ui {
    public class UILineRender : Graphic {

        public Vector2Int gridSize;
        public List<Vector2> points;
        public UIGridRenderer grid;

        public float thickness = 10f;

        private float _width;
        private float _height;
        private float _unitWidth;
        private float _unitHeight;

        protected override void OnPopulateMesh( VertexHelper vh )
            {
                vh.Clear( );

                _width = rectTransform.rect.width;
                _height = rectTransform.rect.height;

                _unitWidth = _width / gridSize.x;
                _unitHeight = _height / gridSize.y;

                if ( points.Count < 2 )
                    return;

                float angle = 0;
                for ( int i = 0; i < points.Count; i++ ) {
                    var p = points[ i ];
                    if ( i < points.Count - 1 ) {
                        angle = GetAngle( points[ i ], points[ i + 1 ] ) + 45f;
                    }
                    DrawVerticesForPoint( p, angle, vh );
                }

                // subtract last point since only drawing line
                for ( int i = 0; i < points.Count - 1; i++ ) {
                    int index = i * 2;
                    vh.AddTriangle( index + 0, index + 1, index + 3 );
                    vh.AddTriangle( index + 3, index + 2, index + 0 );
                }
            }

        public static float GetAngle( Vector2 me, Vector2 target )
            {
                return Mathf.Atan2( target.y - me.y, target.x - me.x ) * ( 180 / Mathf.PI );
            }

        private void DrawVerticesForPoint( Vector2 point, float angle, VertexHelper vh )
            {
                UIVertex vertex = UIVertex.simpleVert;
                vertex.color = color;

                vertex.position = Quaternion.Euler( 0, 0, angle ) * new Vector3( -thickness / 2, 0 );
                vertex.position += new Vector3( _unitWidth * point.x, _unitHeight * point.y );
                vh.AddVert( vertex );

                vertex.position = Quaternion.Euler( 0, 0, angle ) * new Vector3( thickness / 2, 0 );
                vertex.position += new Vector3( _unitWidth * point.x, _unitHeight * point.y );
                vh.AddVert( vertex );
            }

        private void Update( )
            {
                if ( grid != null ) {
                    if ( gridSize != grid.gridSize ) {
                        gridSize = grid.gridSize;
                        SetVerticesDirty( );
                    }
                }
            }

    }
}