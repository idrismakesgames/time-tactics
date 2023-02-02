using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TimelineHelperMethods : MonoBehaviour
{
    #region Variables
    public static TimelineHelperMethods Instance { get; private set; } // Singleton to allow GameController access game wide
    #endregion
    
    #region Lifecycle
    private void Awake() 
    { 
        Instance = this; 
    }
    #endregion
    
    #region Smoothing Methods
    
    // Simple smooth method refactored from pathfinding. apply to simple Path
    public List<Vector3> LineSmoothSimple (
	    List<Vector3> path, bool uniformLength = true, float maxSegmentLength = 0.1f, 
	    int subdivisions = 4, int iterations = 4, float strength = 0.5f
	    ) 
    {
		if (path.Count < 2) return path;

		List<Vector3> subdivided;

		if (uniformLength) {
			// Clamp to a small value to avoid the path being divided into a huge number of segments
			maxSegmentLength = Mathf.Max(maxSegmentLength, 0.005f);

			float pathLength = 0;
			for (int i = 0; i < path.Count-1; i++) {
				pathLength += Vector3.Distance(path[i], path[i+1]);
			}

			int estimatedNumberOfSegments = Mathf.FloorToInt(pathLength / maxSegmentLength);
			// Get a list with an initial capacity high enough so that we can add all points
			subdivided = new List<Vector3>(estimatedNumberOfSegments+2);

			float distanceAlong = 0;

			// Sample points every [maxSegmentLength] world units along the path
			for (int i = 0; i < path.Count-1; i++) {
				var start = path[i];
				var end = path[i+1];

				float length = Vector3.Distance(start, end);

				while (distanceAlong < length) {
					subdivided.Add(Vector3.Lerp(start, end, distanceAlong / length));
					distanceAlong += maxSegmentLength;
				}

				distanceAlong -= length;
			}

			// Make sure we get the exact position of the last point
			subdivided.Add(path[^1]);
		} else {
			subdivisions = Mathf.Max(subdivisions, 0);

			if (subdivisions > 10) {
				Debug.LogWarning("Very large number of subdivisions. Cowardly refusing to subdivide every segment into more than " + (1 << subdivisions) + " subsegments");
				subdivisions = 10;
			}

			int steps = 1 << subdivisions;
			subdivided = new List<Vector3>((path.Count-1)*steps + 1);
			Polygon.Subdivide(path, subdivided, steps);
		}

		if (strength > 0) {
			for (int it = 0; it < iterations; it++) {
				Vector3 prev = subdivided[0];

				for (int i = 1; i < subdivided.Count-1; i++) {
					Vector3 tmp = subdivided[i];

					// prev is at this point set to the value that subdivided[i-1] had before this loop started
					// Move the point closer to the average of the adjacent points
					subdivided[i] = Vector3.Lerp(tmp, (prev+subdivided[i+1])/2F, strength);

					prev = tmp;
				}
			}
		}

		return subdivided;
	}
    #endregion
}
