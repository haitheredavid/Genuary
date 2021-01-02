using Unity.Mathematics;
using static UnityEngine.Mathf;

public enum GraphName {
    Sine,
    Sine2D,
    MultiSine,
    MultiSine2D,
    Ripple,
    UVSphere,
    UVSphereRipple,
    UVSphereTwist,
    Torus
}

public static class GraphLibrary {
    private const float Pi = PI;

    private static float Sine( float x, float time )
        {
            return Sin( Pi * ( x + time ) );
        }

    public delegate float3 Function( float u, float v, float time );

    private static readonly Function[ ] Functions = {
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSineFunction2D, RippleFunction, UVSphereFunction, UVRippleSphereFunction, UVTwistedSphereFunction, TorusFunction
    };

    public static int GetFunctionCount => Functions.Length;
    
    public static GraphName GetNextFunctionName( GraphName name )
        {
            return (int) name < Functions.Length - 1
                ? name + 1
                : 0;
        }

    public static GraphName GetRandomFunctionName( GraphName name )
        {
            var choice = (GraphName) UnityEngine.Random.Range( 1, Functions.Length );
            return choice == name
                ? 0
                : choice;
        }

    public static Function GetFunction( GraphName graph )
        {
            return Functions[ (int) graph ];
        }

    public static float3 Morph( float u, float v, float t, Function from, Function to, float progress )
        {
            return math.lerp( from( u, v, t ), to( u, v, t ), SmoothStep( 0f, 1f, progress ) );
        }

    public static float3 SineFunction( float x, float z, float time )
        {
            return new float3( x, Sine( x, time ), z );
        }

    public static float3 Sine2DFunction( float x, float z, float time )
        {
            float y = Sine( x, time );
            y += Sine( z, time );
            y *= 0.5f; // scale down
            return new float3( x, y, z );
        }

    public static float3 MultiSineFunction( float x, float z, float time )
        {
            float y = Sine( x, time ); // typical sine wave
            y += Sin( 2f * Pi * ( x + time ) ) * 0.5f; // double frequency 
            y *= 2f / 3f; // fit between -1 1 
            return new float3( x, y, z );
        }

    public static float3 MultiSineFunction2D( float x, float z, float time )
        {
            float y = 4f * Sine( x + z, time * 0.5f );
            y += Sine( x, time );
            y += Sin( 2f * Pi * ( z + 2f * time ) ) * 0.5f;
            y *= 1f / 5.5f;
            return new float3( x, y, z );
        }

    public static float3 RippleFunction( float x, float z, float time )
        {
            float freq = 4f;
            float distFromOrg = Sqrt( x * x + z * z );
            float y = Sin( freq * Pi * distFromOrg - time );
            y /= 1f + 10f * distFromOrg;
            return new float3( x, y, z );
        }

    public static float3 UVSphereFunction( float u, float v, float t )
        {
            var p = new float3( );
            var r = 0.5f + 0.5f * Sin( Pi * t );
            var s = r * Cos( 0.5f * Pi * v );

            p.x = s * Sin( Pi * u );
            p.y = r * Sin( Pi * 0.5f * v );
            p.z = s * Cos( Pi * u );
            return p;
        }

    public static float3 UVRippleSphereFunction( float u, float v, float t )
        {
            var p = new float3( );
            var r = 0.9f + 0.1f * Sin( Pi * ( 8f * v + t ) );
            var s = r * Cos( 0.5f * Pi * v );

            p.x = s * Sin( Pi * u );
            p.y = r * Sin( Pi * 0.5f * v );
            p.z = s * Cos( Pi * u );

            return p;
        }

    public static float3 UVTwistedSphereFunction( float u, float v, float t )
        {
            var p = new float3( );
            var r = 0.9f + 0.1f * Sin( Pi * ( 12f * u + 8f * v + t ) );
            var s = r * Cos( 0.5f * Pi * v );

            p.x = s * Sin( Pi * u );
            p.y = r * Sin( Pi * 0.5f * v );
            p.z = s * Cos( Pi * u );

            return p;
        }

    public static float3 TorusFunction( float u, float v, float t )
        {
            var p = new float3( );
            var r1 = 0.7f + 0.1f * Sin( Pi * ( 8f * u + 0.5f * t ) );
            var r2 = 0.15f + 0.05f * Sin( Pi * ( 16f * u + 8f * v + 3f * t ) );
            var s = r1 + r2 * Cos( Pi * v );

            p.x = s * Sin( Pi * u );
            p.y = r2 * Sin( Pi * v );
            p.z = s * Cos( Pi * u );

            return p;
        }
}