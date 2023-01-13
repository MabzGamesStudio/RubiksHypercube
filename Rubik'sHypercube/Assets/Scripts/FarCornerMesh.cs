using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarCornerMesh : MonoBehaviour
{
	Vector3[] verticies;
	int[] triangles;
	BoxCollider boxCollider;

	// Start is called before the first frame update
	void Start()
	{

		Mesh mesh = new Mesh();

		GetComponent<MeshFilter>().mesh = mesh;

		float verticalStretch = CubesMeshSettings.verticalStretch;
		float horizontalStretch = CubesMeshSettings.horizontalStretch;
		float spacing = CubesMeshSettings.spacing;

		boxCollider = GetComponent<BoxCollider>();
		boxCollider.center = new Vector3(spacing + 1.5f * horizontalStretch, 2 * spacing + verticalStretch * 2.5f, spacing + 1.5f * horizontalStretch);
		boxCollider.size = new Vector3(1.5f * horizontalStretch, verticalStretch, 1.5f * horizontalStretch);

		verticies = new Vector3[]
		{
			new Vector3(-0.5f + horizontalStretch + spacing, 2 * spacing + 2 * verticalStretch, -0.5f + horizontalStretch + spacing), // Bottom Front Left
			new Vector3(-0.5f + horizontalStretch + spacing, 2 * spacing + 2 * verticalStretch, -1.5f + 3 * horizontalStretch + spacing), // Bottom Back Left
			new Vector3(-1.5f + 3 * horizontalStretch + spacing, 2 * spacing + 2 * verticalStretch, -1.5f + 3 * horizontalStretch + spacing), // Bottom Back Right
			new Vector3(-1.5f + 3 * horizontalStretch + spacing, 2 * spacing + 2 * verticalStretch, -0.5f + horizontalStretch + spacing), // Bottom Front Right

			new Vector3(-1.0f + 1.5f * horizontalStretch + spacing, 2 * spacing + 3 * verticalStretch, -1f + 1.5f * horizontalStretch + spacing), // Top Front Left
			new Vector3(-1.0f + 1.5f * horizontalStretch + spacing, 2 * spacing + 3 * verticalStretch, -3f + 4.5f * horizontalStretch + spacing), // Top Back Left
			new Vector3(-3f + 4.5f * horizontalStretch + spacing, 2 * spacing + 3 * verticalStretch, -3f + 4.5f * horizontalStretch + spacing), // Top Back Right
			new Vector3(-3f + 4.5f * horizontalStretch + spacing, 2 * spacing + 3 * verticalStretch, -1f + 1.5f * horizontalStretch + spacing), // Top Front Right

		};

		triangles = new int[]
		{

			// Bottom
			0, 2, 1,
			2, 0, 3,

			// Top
			4, 5, 6,
			6, 7, 4,

			// Front
			0, 4, 7,
			7, 3, 0,

			// Left
			0, 1, 4,
			4, 1, 5,

			// Right
			3, 6, 2,
			6, 3, 7,

			// Back
			1, 2, 5,
			5, 2, 6,

		};


		mesh.vertices = verticies;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();

	}
}
