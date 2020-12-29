using UnityEngine;
using UnityEngine.UI;


namespace Toolkit.Ui {
    public class UIGridRenderer : Graphic {

        public Vector2Int gridSize = new Vector2Int( 1, 1 );
        public float thickness = 10f;

        private float _width;
        private float _height;
        private float _cellWidth;
        private float _cellHeight;

        // when meshes are drawn to screen
        protected override void OnPopulateMesh( VertexHelper vh )
            {
                // clear cache
                vh.Clear( );

                // define dim of our area
                _width = rectTransform.rect.width;
                _height = rectTransform.rect.height;

                _cellWidth = _width / gridSize.x;
                _cellHeight = _height / gridSize.y;

                int count = 0;
                for ( int y = 0; y < gridSize.y; y++ ) {
                    for ( int x = 0; x < gridSize.x; x++ ) {
                        DrawCell( x, y, count, vh );
                        count++;
                    }
                }
            }

        private void DrawCell( int x, int y, int index, VertexHelper vh )
            {
                float xPos = _cellWidth * x;
                float yPos = _cellHeight * y;

                // setup vert style 
                UIVertex vertex = UIVertex.simpleVert;
                vertex.color = color;

                // plot verts
                vertex.position = new Vector3( xPos, yPos );
                vh.AddVert( vertex );

                vertex.position = new Vector3( xPos, yPos + _cellHeight );
                vh.AddVert( vertex );

                vertex.position = new Vector3( xPos + _cellWidth, yPos + _cellHeight );
                vh.AddVert( vertex );

                vertex.position = new Vector3( xPos + _cellWidth, yPos );
                vh.AddVert( vertex );

                // finding interior mesh thickness
                float widthSqr = thickness * thickness;
                float distanceSqr = widthSqr / 2f;
                float distance = Mathf.Sqrt( distanceSqr );

                vertex.position = new Vector3( xPos + distance, yPos + distance );
                vh.AddVert( vertex );

                vertex.position = new Vector3( xPos + distance, yPos + ( _cellHeight - distance ) );
                vh.AddVert( vertex );

                vertex.position = new Vector3( xPos + ( _cellWidth - distance ), yPos + ( _cellHeight - distance ) );
                vh.AddVert( vertex );

                vertex.position = new Vector3( xPos + ( _cellWidth - distance ), yPos + distance );
                vh.AddVert( vertex );

                // offset index for each cell with 8 verts
                int offset = index * 8;

                // Left Edge
                vh.AddTriangle( offset + 0, offset + 1, offset + 5 );
                vh.AddTriangle( offset + 5, offset + 4, offset + 0 );
                // Top Edge
                vh.AddTriangle( offset + 1, offset + 2, offset + 6 );
                vh.AddTriangle( offset + 6, offset + 5, offset + 1 );
                // Right Edge
                vh.AddTriangle( offset + 2, offset + 3, offset + 7 );
                vh.AddTriangle( offset + 7, offset + 6, offset + 2 );
                // Bottom Edge
                vh.AddTriangle( offset + 3, offset + 0, offset + 4 );
                vh.AddTriangle( offset + 4, offset + 7, offset + 3 );
            }

    }
}