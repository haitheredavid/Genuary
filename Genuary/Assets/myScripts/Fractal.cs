using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace myScripts {
	public class Fractal : MonoBehaviour {


		private struct FractalPart {

			public Vector3 direction , worldPosition;
			public Quaternion rotation , worldRotation;
			public float spinAngle;

		}


		[SerializeField , Range(1 , 8)]
		private int depth = 4;

		[SerializeField]
		private Mesh mesh = default;

		[SerializeField]
		private Material material = default;

		private FractalPart[][] _parts;
		private Matrix4x4[][] _matrices;
		private ComputeBuffer[] _matricesBuffers;

		private static Vector3[] directions = {
			Vector3.up , Vector3.right , Vector3.left , Vector3.forward , Vector3.back ,
		};

		private static Quaternion[] rotations = {
			quaternion.identity ,
			quaternion.Euler(0f , 0f , -90f) , quaternion.Euler(0f , 0f , 90f) ,
			quaternion.Euler(90f , 0f , 0f) , quaternion.Euler(-90f , 0f , 0f)
		};

		private static readonly int matricesId = Shader.PropertyToID("_Matrices");

		private static MaterialPropertyBlock _propertyBlock;

		private void OnValidate(){
			if(_parts != null && enabled){
				OnDisable();
				OnEnable();
			}
		}

		private void OnEnable(){
			_parts = new FractalPart[depth][];
			_matrices = new Matrix4x4[depth][];
			_matricesBuffers = new ComputeBuffer[depth];
			int stride = 16 * 4;
			for (int i = 0 , length = 1; i < _parts.Length; i++ , length *= 5){
				_parts[i] = new FractalPart[length];
				_matrices[i] = new Matrix4x4[length];
				_matricesBuffers[i] = new ComputeBuffer(length , stride);
			}

			_parts[0][0] = CreatePart(0);

			for (int li = 1; li < _parts.Length; li++){
				FractalPart[] levelParts = _parts[li];
				for (int fpi = 0; fpi < levelParts.Length; fpi += 5){
					for (int ci = 0; ci < 5; ci++){
						levelParts[fpi + ci] = CreatePart(ci);
					}
				}
			}
			if(_propertyBlock == null)
				_propertyBlock = new MaterialPropertyBlock();
		}


		private void OnDisable(){
			for (int i = 0; i < _matricesBuffers.Length; i++){
				_matricesBuffers[i].Release();
			}
			_parts = null;
			_matrices = null;
			_matricesBuffers = null;
		}

		private void Update(){

			float spinAngleDelta = 22.5f * Time.deltaTime;

			FractalPart rootPart = _parts[0][0];
			rootPart.spinAngle += spinAngleDelta;
			rootPart.worldRotation =
				transform.rotation *
				(rootPart.rotation * Quaternion.Euler(0f , rootPart.spinAngle , 0f));
			rootPart.worldPosition = transform.position;

			float objectScale = transform.lossyScale.x;
			_parts[0][0] = rootPart;
			_matrices[0][0] = Matrix4x4.TRS(rootPart.worldPosition , rootPart.worldRotation , objectScale * Vector3.one);

			float scale  = objectScale;
			for (int li = 1; li < _parts.Length; li++){
				scale *= 0.5f;
				FractalPart[] parentsParts = _parts[li - 1];
				FractalPart[] levelParts = _parts[li];
				Matrix4x4[] levelMatrices = _matrices[li];
				for (int fpi = 0; fpi < levelParts.Length; fpi++){
					FractalPart parent = parentsParts[fpi / 5];
					FractalPart part = levelParts[fpi];
					part.spinAngle += spinAngleDelta;
					part.worldRotation = parent.worldRotation * (part.rotation * Quaternion.Euler(0f , part.spinAngle , 0f));
					part.worldPosition =
						parent.worldPosition +
						parent.worldRotation * (1.5f * scale * part.direction);
					levelParts[fpi] = part;
					levelMatrices[fpi] = Matrix4x4.TRS(part.worldPosition , part.worldRotation , scale * Vector3.one);

				}
			}

			var bounds = new Bounds(rootPart.worldPosition , 3f * objectScale * Vector3.one);
			for (int i = 0; i < _matricesBuffers.Length; i++){
				var buffer = _matricesBuffers[i];
				buffer.SetData(_matrices[i]);
				material.SetBuffer(matricesId , buffer);
				_propertyBlock.SetBuffer(matricesId , buffer);
				Graphics.DrawMeshInstancedProcedural(mesh , 0 , material , bounds , buffer.count , _propertyBlock);
			}
		}

		private FractalPart CreatePart(  int childIndex ){
			return new FractalPart {
				direction = directions[childIndex] ,
				rotation = rotations[childIndex]
			};

		}

	}
}