using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toolkit.Ui {
    public class UICellRender : Graphic {

        // TODO Look into moving quads into textures
        // TODO Mouse over function for each quad
        // TODO Animate gradient coloring 

        public UIGridRenderer grid;
        public Gradient gradient;
        [Range( 0, 10f )] public float timer = 1f;
        [Range( 0, 1f )] public float cellSize = 1f;

        private float _width;
        private float _height;

        private Vector2Int _gridSize;
        private float _cellWidth;
        private float _cellHeight;
        private float _quadWidth;
        private float _quadHeight;

        private float _time = 0f;

        private void Update( )
            {
                
                if ( _time >= timer ) {
                    _time = 0;
                } else {
                    _time += Time.deltaTime;
                }

                if ( grid == null ) return;

                if ( _gridSize == grid.gridSize ) return;

                _gridSize = grid.gridSize;
                SetVerticesDirty( );
                
            }

        protected override void OnPopulateMesh( VertexHelper vh )
            {
                vh.Clear( );

                if ( cellSize <= 0 || grid == null ) return;

                var rect = rectTransform.rect;

                _width = rect.width;
                _height = rect.height;

                _cellWidth = _width / _gridSize.x;
                _cellHeight = _height / _gridSize.y;

                float thickness = grid.thickness;
                float distanceSqr = thickness * thickness / 2f;
                float borderSize = Mathf.Sqrt( distanceSqr );

                _quadWidth = ( _cellWidth - borderSize ) * cellSize;
                _quadHeight = ( _cellHeight - borderSize ) * cellSize;

                int count = 0;

             
                for ( int y = 0; y < _gridSize.y; y++ ) {
                    // Get each rows color
                    var step = (float) y / _gridSize.y;
                    var xColor = gradient.Evaluate( step );

                    for ( int x = 0; x < _gridSize.x; x++ ) {
                        var xNorm = (float) x / _gridSize.x;
                        var stepNorm = ( xNorm - _time ) / ( timer - _time );
                        var lerpColor = Color.Lerp( xColor, Color.white, stepNorm );
                        Debug.Log( $"Step Norm {stepNorm} : "  );

                        DrawCell( x, y, count, lerpColor, borderSize, vh );
                        count++;
                    }
                }
            }

        private void DrawCell( int x, int y, int index, Color color, float size, VertexHelper vh )
            {
                float xPos = _cellWidth * x;
                float yPos = _cellHeight * y;

                UIVertex vertex = UIVertex.simpleVert;

                // bot left
                vertex.position = new Vector3( xPos + size, yPos + size );
                vertex.color = color;
                vh.AddVert( vertex );

                // top left
                vertex.position = new Vector3( xPos + size, yPos + _quadHeight );
                vertex.color = color;
                vh.AddVert( vertex );

                // top right
                vertex.position = new Vector3( xPos + _quadWidth, yPos + _quadHeight );
                vertex.color = color;
                vh.AddVert( vertex );

                // bot right
                vertex.position = new Vector3( xPos + _quadWidth, yPos + size );
                vertex.color = color;
                vh.AddVert( vertex );

                int offset = index * 4;

                // Left Edge
                vh.AddTriangle( offset + 0, offset + 1, offset + 2 );
                vh.AddTriangle( offset + 2, offset + 3, offset + 0 );
            }

    }
}