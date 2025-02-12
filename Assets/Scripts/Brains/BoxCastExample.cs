using UnityEngine;

namespace Brains
{
    public class BoxCastExample: MonoBehaviour
    {
        bool m_Started;
        public LayerMask m_LayerMask;

        public Vector3 halfExtents;
        public Vector3 originOffset;

        void Start()
        {
            if (halfExtents == Vector3.zero)
            {
                halfExtents = transform.localScale / 2;
            }
            
            m_Started = true;
        }

        void FixedUpdate()
        {
            MyCollisions();
        }

        void MyCollisions()
        {
            //Use the OverlapBox to detect if there are any other colliders within this box area.
            //Use the GameObject's centre, half the size (as a radius) and rotation.
            //This creates an invisible box around your GameObject.
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position + originOffset, 
                halfExtents, Quaternion.identity, m_LayerMask);
            int i = 0;
            
            //Check when there is a new collider coming into contact with the box
            while (i < hitColliders.Length)
            {
                Debug.Log("Hit : " + hitColliders[i].name + i);
                i++;
            }
        }

        //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            if (m_Started)
                //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
                Gizmos.DrawWireCube(transform.position + originOffset, halfExtents * 2);
        }
    }
}