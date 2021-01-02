using System;
using UnityEngine;

public class GraphGPU : MonoBehaviour {

	[SerializeField,Range(10,MaxRes)]
	private int resolution = 10;
	[SerializeField]
	private GraphName function;
	[SerializeField]
	private TransitionMode transitionMode = TransitionMode.Cycle;
	[SerializeField,Min(0f)]
	private float functionDuration = 1f,transitionDuration = 1f;
	[SerializeField]
	private ComputeShader computeShader = default;
	[SerializeField]
	private Material material = default;
	[SerializeField]
	private Mesh mesh = default;

	private const int MaxRes = 1000;

	public enum TransitionMode {

		Cycle,
		Random

	}

	private float _duration;
	private bool _transitioning;
	private GraphName _transitionFunction;

	private ComputeBuffer _positionBuffer;

	private static readonly int
		PositionId = Shader.PropertyToID("_Positions"),
		ResolutionId = Shader.PropertyToID("_Resolution"),
		StepId = Shader.PropertyToID("_Step"),
		TimeId = Shader.PropertyToID("_Time"),
		TransitionId = Shader.PropertyToID("_TransitionProgress");

	private void UpdateFunctionOnGPU(){
		float step = 2f / resolution;
		computeShader.SetInt(ResolutionId,resolution);
		computeShader.SetFloat(StepId,step);
		computeShader.SetFloat(TimeId,Time.time);

		if(_transitioning){
			computeShader.SetFloat(TransitionId,Mathf.SmoothStep(0f,1f,_duration / transitionDuration));
		}
		var kernelIndex = (int)function + (int)(_transitioning ? _transitionFunction : function) * GraphLibrary.GetFunctionCount;
		computeShader.SetBuffer(kernelIndex,PositionId,_positionBuffer);

		int groups = Mathf.CeilToInt(resolution / 8f);
		computeShader.Dispatch(kernelIndex,groups,groups,1);

		material.SetBuffer(PositionId,_positionBuffer);
		material.SetFloat(StepId,step);

		var bounds = new Bounds(Vector3.zero,Vector3.one * (2f + 2f / resolution));
		Graphics.DrawMeshInstancedProcedural(mesh,0,material,bounds,resolution * resolution);
	}

	private void PickNextFunction(){
		function = transitionMode == TransitionMode.Cycle
			? GraphLibrary.GetNextFunctionName(function)
			: GraphLibrary.GetRandomFunctionName(function);
	}

	// "Compute Buffers do not survive hot reloads"
	// With any change during play mode it will disappear
	private void OnEnable(){
		var memorySize = 3 * 4;
		var bufferSize = MaxRes * MaxRes;
		_positionBuffer = new ComputeBuffer(bufferSize,memorySize);
	}

	private void OnDisable(){
		_positionBuffer?.Release();
		_positionBuffer = null;
	}

	private void Update(){
		_duration += Time.deltaTime;
		if(_transitioning){
			if(_duration >= transitionDuration){
				_duration -= transitionDuration;
				_transitioning = false;
			}
		} else if(_duration >= functionDuration){
			_duration -= functionDuration;
			_transitioning = true;
			_transitionFunction = function;
			PickNextFunction();
		}
		UpdateFunctionOnGPU();
	}


}