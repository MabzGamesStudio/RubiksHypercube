using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The bottom layer center hypersticker mesh and box collider
/// </summary>
public class NearCenterMesh : MonoBehaviour
{

	/// <summary>
	/// Vertex positions of the hypersticker cuboid
	/// </summary>
	Vector3[] verticies;

	/// <summary>
	/// Triangles that make up the polygon mesh
	/// </summary>
	int[] triangles;

	/// <summary>
	/// The box collider of the hypersticker
	/// </summary>
	BoxCollider boxCollider;

	/// <summary>
	/// Initializes the mesh and box collider settings
	/// </summary>
	void Start()
	{

		// Create new mesh for this sticker
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		// Get the hypersticker stretch and spacing settings
		float verticalStretch = CubesMeshSettings.verticalStretch;
		float horizontalStretch = CubesMeshSettings.horizontalStretch;
		float spacing = CubesMeshSettings.spacing;

		// Set up the box collider to approximate the size of the cuboid
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.center = new Vector3(0, verticalStretch / 2, 0);
		boxCollider.size = new Vector3(0.5f * horizontalStretch + spacing, verticalStretch + spacing, 0.5f * horizontalStretch + spacing);

		// Dynamic vertex positions based on stretch and spacing settings
		verticies = new Vector3[]
		{

			// Bottom Front Left
			new Vector3(
				-0.5f,
				0,
				-0.5f),

			// Bottom Back Left
			new Vector3(
				-0.5f,
				0,
				0.5f),

			// Bottom Back Right
			new Vector3(
				0.5f,
				0,
				0.5f),

			// Bottom Front Right
			new Vector3(
				0.5f,
				0,
				-0.5f),

			// Top Front Left
			new Vector3(
				-0.5f * horizontalStretch,
				verticalStretch,
				-0.5f * horizontalStretch),

			// Top Back Left
			new Vector3(
				-0.5f * horizontalStretch,
				verticalStretch,
				0.5f * horizontalStretch),

			// Top Back Right
			new Vector3(
				0.5f * horizontalStretch,
				verticalStretch,
				0.5f * horizontalStretch),

			// Top Front Right
			new Vector3(
				0.5f * horizontalStretch,
				verticalStretch,
				-0.5f * horizontalStretch),
		};

		// Trangles of the cuboids made by indexing 3 vertecies
		triangles = new int[]
		{

			// Bottom face triangles
			0, 2, 1,
			2, 0, 3,

			// Top face triangles
			4, 5, 6,
			6, 7, 4,

			// Front face triangles
			0, 4, 7,
			7, 3, 0,

			// Left face triangles
			0, 1, 4,
			4, 1, 5,

			// Right face triangles
			3, 6, 2,
			6, 3, 7,

			// Back face triangles
			1, 2, 5,
			5, 2, 6,

		};

		// Update the mesh settings
		mesh.vertices = verticies;
		mesh.triangles = triangles;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

}
