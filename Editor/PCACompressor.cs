using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using PowerEditor;
using PowerEditor.Math;
using Unity.Rendering.HybridV2;
using UnityEngine;
using UnityEditor;

public class PCACompressor : EditorWindow
{
	private static Texture2D A;
	private static Texture2D B;
	private static string savePath = "Assets";
	private static string name = "packed";

	[System.Serializable]
	public class PCAParams
	{
		public PCAPlane A;
		public PCAPlane B;

		public PCAParams(PCAPlane A, PCAPlane B)
		{
			this.A = A;
			this.B = B;
		}
	}

	[System.Serializable]
	public class PCAPlane
	{
		public Vector3 normal;
		public float distance;

		public static implicit operator PCAPlane(Plane plane)
		{
			return new PCAPlane(plane);
		}

		public static implicit operator Plane(PCAPlane plane)
		{
			return new Plane(plane.normal, plane.distance);
		}

		public PCAPlane(Plane plane)
		{
			normal = plane.normal;
			distance = plane.distance;
		}

		public Vector4 decodeParams
		{
			get
			{
				Vector3 N = normal.Abs();
				//int axis = 2;
				int axis = 0;
				if (N.x > N.y && N.x > N.z)
				{
					axis = 0;
				}
				else if(N.y > N.z)
				{
					axis = 1;
				}
				else
				{
					axis = 2;
				}
				Vector3 decode = new Vector3(-normal[(axis + 1) % 3], -normal[(axis + 2) % 3], distance) / normal[axis];
				return new Vector4(decode.x, decode.y, decode.z, axis);
			}
		}
	}

	[MenuItem("Tools/PCACompressor")]
	public static void Entry()
	{
		GetWindow<PCACompressor>().Show();
	}

	void OnGUI()
	{
		A = EditorGUILayout.ObjectField(A, typeof(Texture2D), false) as Texture2D;
		B = EditorGUILayout.ObjectField(B, typeof(Texture2D), false) as Texture2D;

		if (GUILayout.Button("Pack"))
		{
			string path = EditorUtility.SaveFilePanelInProject("Save To", name, "asset", "Save Packed Texture To", savePath);
			if (string.IsNullOrEmpty(path))
			{
				return;
			}
			Texture2D packed = Pack(A, B, out PCAParams pcaParams);
			//string pathFS = AssetDatabaseUtils.InProjectPathToFileSystemPath(path);
			//File.WriteAllBytes(pathFS, packed.EncodeToPNG());
			AssetDatabase.CreateAsset(packed, path);
			AssetDatabase.Refresh();
			AssetImporter importer = AssetImporter.GetAtPath(path);
			importer.RegisterAssetDependency(A, B);
			importer.SetUserData("PCAParams", pcaParams);
			importer.SaveAndReimport();
		}
	}
	
	public static Texture2D Pack(Texture2D A, Texture2D B, out PCAParams pcaParams)
	{
		if (!Textureutils.CompareTextureFormat(Textureutils.TexturePropertyMask.All, A, B))
		{
			pcaParams = default(PCAParams);
			return null;
		}

		Vector3[] pixelsA = A.GetPixels(0).Select(c => new Vector3(c.r, c.g, c.b)).ToArray();
		Vector3[] pixelsB = B.GetPixels(0).Select(c => new Vector3(c.r, c.g, c.b)).ToArray();
		Plane planeA, planeB;
		PCA.ProjectionPlane projectionPlaneA, projectionPlaneB;
		Vector2[] encodedPixelsA = PCA.EncodeVector3(pixelsA, out planeA, out projectionPlaneA);
		Vector2[] encodedPixelsB = PCA.EncodeVector3(pixelsB, out planeB, out projectionPlaneB);

		int width = A.width;
		int height = A.height;
		TextureFormat format = TextureFormat.ARGB32;
		bool mip = A.mipmapCount > 1;
		Texture2D packed = new Texture2D(width, height, format, mip, true);
		
		Color[] pixels = new Color[pixelsA.Length];
		for (int i = 0; i < pixelsA.Length; ++i)
		{
			Vector2 encodedPixelA = encodedPixelsA[i];
			Vector2 encodedPixelB = encodedPixelsB[i];
			pixels[i] = new Color(
				encodedPixelA.x, 
				encodedPixelA.y, 
				encodedPixelB.x, 
				encodedPixelB.y);
		}
		packed.SetPixels(pixels);
		packed.Apply();

		pcaParams = new PCAParams(planeA, planeB);
		return packed;
	}
}