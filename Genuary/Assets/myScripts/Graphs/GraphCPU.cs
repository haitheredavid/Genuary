using System;
using UnityEngine;

public class GraphCPU : MonoBehaviour {
    [SerializeField]
    private GameObject pointPrefab;
    [SerializeField, Range( 1, 100 )]
    private int resolution = 10;
    [SerializeField]
    private GraphName function;
    [SerializeField]
    private TransitionMode transitionMode = TransitionMode.Cycle;
    [SerializeField, Min( 0f )]
    private float functionDuration = 1f, transitionDuration = 1f;

    public enum TransitionMode {
        Cycle,
        Random
    }

    private GameObject[ ] _points;
    private float _duration;
    private bool _transitioning;
    private GraphName _transitionFunction;

    private float StepSize => 2f / resolution;

    private void Awake( )
        {
            if ( pointPrefab == null ) {
                Debug.Log( "No prefab set" );
            } else {
                Vector3 scale = Vector3.one * StepSize;
                _points = new GameObject[ resolution * resolution ];

                for ( int i = 0; i < _points.Length; i++ ) {
                    GameObject point = Instantiate( pointPrefab, transform, false );
                    point.name = "Point Instance " + i;
                    point.transform.localScale = scale;
                    _points[ i ] = point;
                }
            }
        }

    private void Update( )
        {
            _duration += Time.deltaTime;
            if ( _transitioning ) {
                if ( _duration >= transitionDuration ) {
                    _duration -= transitionDuration;
                    _transitioning = false;
                }
            } else if ( _duration >= functionDuration ) {
                _duration -= functionDuration;
                _transitioning = true;
                _transitionFunction = function;
                PickNextFunction( );
            }
            if ( _transitioning ) {
                UpdateFunctionTransition( );
            } else {
                UpdateFunction( );
            }
        }

    private void PickNextFunction( )
        {
            function = transitionMode == TransitionMode.Cycle
                ? GraphLibrary.GetNextFunctionName( function )
                : GraphLibrary.GetRandomFunctionName( function );
        }

    private void UpdateFunctionTransition( )
        {
            GraphLibrary.Function
                from = GraphLibrary.GetFunction( _transitionFunction ),
                to = GraphLibrary.GetFunction( function );

            var progress = _duration / transitionDuration;
            var time = Time.time;
            var step = StepSize;
            var v = 0.5f * step - 1f;
            for ( int i = 0, x = 0, z = 0; i < _points.Length; i++, x++ ) {
                if ( x == resolution ) {
                    x = 0;
                    z += 1;
                    v = ( z + 0.5f ) * step - 1f;
                }
                float u = ( x + 0.5f ) * step - 1f;
                _points[ i ].transform.localPosition = GraphLibrary.Morph(
                    u, v, time, from, to, progress
                );
            }
        }

    private void UpdateFunction( )
        {
            var graphFunction = GraphLibrary.GetFunction( function );
            var time = Time.time;
            var step = StepSize;
            var v = 0.5f * step - 1f;
            for ( int i = 0, x = 0, z = 0; i < _points.Length; i++, x++ ) {
                if ( x == resolution ) {
                    x = 0;
                    z += 1;
                    v = ( z + 0.5f ) * step - 1f;
                }
                float u = ( x + 0.5f ) * step - 1f;
                _points[ i ].transform.localPosition = graphFunction( u, v, time );
            }
        }
}