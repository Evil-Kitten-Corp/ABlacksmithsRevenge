using UnityEngine;

namespace Tools
{
    public class BowStringUpdater : MonoBehaviour
    {
        [SerializeField] private Transform start, end;
        [SerializeField] private LineRenderer lineRenderer;

        private void Start()
        {
            CreateString();
        }

        public void CreateString(Vector3? position = null)
        {
            Vector3[] pts = new Vector3[position == null ? 2 : 3];
            pts[0] = start.localPosition;

            if (position != null)
            {
                pts[1] = transform.InverseTransformPoint(position.Value);
            }
            
            pts[^1] = end.localPosition;
            lineRenderer.positionCount = pts.Length;
            lineRenderer.SetPositions(pts);
        }
    }
}