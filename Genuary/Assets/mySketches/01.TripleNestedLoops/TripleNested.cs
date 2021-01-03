using System;
using System.Collections.Generic;
using UnityEngine;


public class TripleNested : MonoBehaviour {


	[SerializeField] private GameObject obj;
	[SerializeField] private bool generate = false;
	[SerializeField , Range(0.0f , 10.0f)] private float size = 1f;
	[SerializeField , Range(1.0f , 10.0f)] private float transitionDuration = 1f;
	[SerializeField , Range(1 , 50)] private int xCount = 1 , yCount = 1 , zCount = 1;

	private List<GameObject> _objects = new List<GameObject>();

	private float _duration;
	private bool _transitioning;


	private void CreateGrid(){

		if(_objects != null && _objects.Count > 0){
			foreach (var t in _objects){
				Destroy(t.gameObject);
			}
		}

		if (obj == null) return;


		_objects = new List<GameObject>();
		var totalCount = xCount * yCount * zCount;

		for (int i = 0; i < totalCount; i++){
			var prefab = Instantiate(obj , transform);
			prefab.name = $"grid { i }";
			_objects.Add(prefab);
		}

	}

	private void Awake(){
		_reverse = false;
		_duration = 0f;
	}

	private bool _reverse;
	private void Update(){

		if (_duration >= transitionDuration) {
			_reverse = true;
		} else if(_duration <= 0f){
			_reverse = false;
		}

		if(_reverse){
			_duration -= Time.deltaTime;
		} else{
			_duration += Time.deltaTime;
		}


		var totalCount = xCount * yCount * zCount;

		if(_objects == null || _objects.Count != totalCount ){
			CreateGrid();

		}
		UpdateGrid();
	}

	private void UpdateGrid(){

		var totalCount = xCount * yCount * zCount;
		var maxSize = Vector3.one * size;
		var minSize = Vector3.one * 0.01f;
		var progress = _duration / transitionDuration;

		var index = 0;
		for (int x = 0; x < xCount; x++){
			var xPos = x * size;
			for (int y = 0; y < yCount; y++){
				var yPos =  y * size;
				for (int z = 0; z < zCount; z++ , index++){
					var zPos =  z * size;

					var weight = Mathf.Clamp01((float)(x * y * z) / totalCount + progress);
					var center = new Vector3(xPos , yPos , zPos);

					var item = _objects[index];
					item.transform.position = center;
					item.transform.localScale = Vector3.Lerp(minSize , maxSize , weight);
					item.GetComponent<MeshRenderer>().material.color =  Color.Lerp(Color.yellow , Color.red , weight);

				}
			}
		}
	}



}