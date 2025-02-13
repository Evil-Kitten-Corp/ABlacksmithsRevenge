using UnityEngine;

namespace Tools
{
    public class VRBowString : MonoBehaviour
    {
        [SerializeField] private Transform beginning;
        [SerializeField] private Transform end;

        private LineRenderer _lr;

        private void Awake()
        {
            _lr = GetComponent<LineRenderer>();
        }

        public void CreateString(Vector3? midPos)
        {
            Vector3[] linePoints = new Vector3[midPos == null ? 2 : 3];
            linePoints[0] = beginning.localPosition;

            if (midPos != null)
            {
                linePoints[1] = transform.InverseTransformPoint(midPos.Value);
            }
            
            linePoints[^1] = end.localPosition;
            _lr.positionCount = linePoints.Length;
            _lr.SetPositions(linePoints);
        }

        private void Start()
        {
            CreateString(null);
        }
    }
}