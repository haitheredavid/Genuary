using System.Collections.Generic;
using UnityEngine;

namespace mySketches._01.TripleNestedLoops {
	public class TreeMap : MonoBehaviour {

		
	[SerializeField] private bool generate = false;
	[SerializeField , Range(1 , 10)] private float size = 1f;
	[SerializeField , Range(1 , 10)] private int treeCount = 1;
	[SerializeField , Range(1 , 10)] private int branches = 1;
	[SerializeField , Range(1 , 10)] private int stems = 1;


	private List<Vector3> _points = new List<Vector3>();
	private List<Tree> _trees = new List<Tree>();


	private void CreateTreeMap(float min , float max){

		var botLeft = new Vector3(min , min);
		var topLeft = new Vector3(min , min + max);

		_trees = new List<Tree>();

		var treeSpacing = max / (treeCount + 1);
		var treeSize = 0f;
		for (int i = 0; i < treeCount; i++) {

			
			treeSize += max / (treeCount + 1);

			var treeStart = new Vector3( botLeft.x + treeSize , botLeft.y);
			var treeEnd = new Vector3( topLeft.x + treeSize , topLeft.y);

			var tree = new Tree(treeStart , treeEnd);

			var branchCount = Random.Range(1 , branches);
			if(branchCount > 0) {
				var treeBranches = new List<Branch>();

				var branchDis = 0f;
				for (int b = 0; b < branchCount; b++) {

					branchDis += treeSize / (branchCount + 1);

					var branchStart = new Vector3(
						treeStart.x ,
						treeStart.y + branchDis
					);

					var branchEnd = new Vector3(
						treeStart.x + treeSpacing ,
						treeStart.y + branchDis
					);

					var branch = new Branch(branchStart , branchEnd);
					treeBranches.Add(branch);
				}
				tree.branches = treeBranches;
			}

			_trees.Add(tree);

		}

	}
	public void OnDrawGizmos(){

		var min = 0;
		var max = size;

		var botLeft = new Vector3(min , min);
		var topLeft = new Vector3(min , min + max);
		var topRight = new Vector3(min + max , min + max);
		var botRight = new Vector3(min + max , min);

		Gizmos.color = Color.black;
		Gizmos.DrawLine(botLeft , topLeft);

		Gizmos.color = Color.red;
		Gizmos.DrawLine(topLeft , topRight);

		Gizmos.color = Color.green;
		Gizmos.DrawLine(topRight , botRight);

		Gizmos.color = Color.grey;
		Gizmos.DrawLine(botRight , botLeft);

		if(generate){

			CreateTreeMap(min , max);
			generate = false;
		}

		foreach (var tree in _trees){

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(tree.segment.startPoint , tree.segment.endPoint);
			if(tree.branches != null && tree.branches.Count > 0){
				foreach (var branch in tree.branches){
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine(branch.segment.startPoint , branch.segment.endPoint);

				}
			}
		}


	}

	private struct Tree {

		public Segment segment;
		public List<Branch> branches;

		public Tree(Vector3 startPoint , Vector3 endPoint){
			segment = new Segment(startPoint , endPoint);
			branches = new List<Branch>();
		}

	}


	private struct Branch {

		public Segment segment;
		public List<Segment> stems;

		public Branch(Vector3 startPoint , Vector3 endPoint){
			segment = new Segment(startPoint , endPoint);
			stems = new List<Segment>();
		}

	}

	private struct Segment {

		public Vector3 startPoint;
		public Vector3 endPoint;

		public Segment(Vector3 startPoint , Vector3 endPoint){
			this.startPoint = startPoint;
			this.endPoint = endPoint;
		}

	}


	}
}