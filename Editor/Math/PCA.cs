using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Rendering.HybridV2;
using UnityEngine;
using UnityEngine.EventSystems;
using PowerEditor;

namespace PowerEditor.Math
{
	public static class PCA
	{
		public enum ProjectionPlane
		{
			YZ = 0,
			ZX = 1,
			XY = 2,
		};

		public static Plane FitPlane(Vector3[] points)
		{
			Vector3 centroid = new Vector3(0, 0, 0);
			Vector3 normal;
			foreach (var point in points)
			{
				centroid += point;
			}
			centroid *= 1.0f / points.Length;
			
			float xx = 0, yy = 0, xy = 0, xz = 0, yz = 0, zz = 0;
			foreach (var point in points)
			{
				xx += point.x * point.x;
				yy += point.y * point.y;
				xy += point.x * point.y;
				xz += point.x * point.z;
				yz += point.y * point.z;
				zz += point.z * point.z;
			}

			float detX = yy * zz - yz * yz;
			float detY = xx * zz - xz * xz;
			float detZ = xx * yy - xy * xy;
			float maxDet = Mathf.Max(detX, detY, detZ);
			if (maxDet == detX)
			{
				normal = new Vector3(detX, xz * yz - xy * zz, xy * yz - xz * yy).normalized;
			}
			else if (maxDet == detY)
			{
				normal = new Vector3(xz * yz - xy * zz, detY, xy * xz - yz * xx).normalized;
			}
			else
			{
				normal = new Vector3(xy * xz - xz * yy, xy * xz - yz * xx, detZ).normalized;
			}
			
			return new Plane(normal, centroid);
		}
		
		public static Vector2[] EncodeVector3(this Vector3[] points, out Plane plane, out ProjectionPlane projectionPlane)
		{
			plane = FitPlane(points);
			Vector2[] encodedPoints = new Vector2[points.Length];

			Vector3 N = plane.normal.Abs();
			if (N.x > N.y && N.x > N.z)
			{
				projectionPlane = ProjectionPlane.YZ;
			}
			else if (N.y > N.z)
			{
				projectionPlane = ProjectionPlane.ZX;
			}
			else
			{
				projectionPlane = ProjectionPlane.XY;
			}
			//projectionPlane = ProjectionPlane.XY;

			int axis = (int)projectionPlane;
			Vector2Int channelID = new Vector2Int((axis + 1) % 3, (axis + 2) % 3);
			
			for (int i = 0; i < points.Length; ++i)
			{
				Vector3 flattenPoint = plane.ClosestPointOnPlane(points[i]);
				encodedPoints[i] = new Vector2(flattenPoint[channelID[0]], flattenPoint[channelID[1]]);
			}

			return encodedPoints;
		}
	}
}
