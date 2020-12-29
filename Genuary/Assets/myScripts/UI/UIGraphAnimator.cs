using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Toolkit.Ui {
    public class UIGraphAnimator : MonoBehaviour {

        public UILineRender[ ] lines;

        public float time = 1f;

        private void OnEnable( )
            {
                AnimateLines( );
            }

        public void AnimateLines( )
            {
                foreach ( var line in lines ) {
                    AnimateLine( line );
                }
            }
        
        
        private void AnimateLine( UILineRender line )
            {
                List<Vector2> points = line.points.ToList( );
                Animate( line, points );
            }
        private void Animate( UILineRender line, List<Vector2> points )
            {
                line.points =new List<Vector2>();
                for ( int i = 0; i < points.Count; i++ ) {
                    AnimatePoint(line, i, new Vector2( 0,4 ), points[i]  );
                }
            }
        private void AnimatePoint( UILineRender line, int index, Vector2 start, Vector2 end )
            {

                if ( index > 0 ) {
                    start = line.points[ index - 1 ];
                    line.points.Add( start );
                } else {
                    line.points.Add( start );
                }
                
                iTween.Vector2Update( line.points[ index ], end, 1f );


            }

    }
}