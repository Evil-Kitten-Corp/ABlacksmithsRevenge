using UnityEngine;

namespace Tools
{
    public class Arrow : MonoBehaviour
    {
        public Transform bowSocket;
        
        //call on exit selection
        public void UpdateRotation()
        {
            Debug.Log("Tried to rotate arrow");
            Vector3 directionToBowStart = bowSocket.position - transform.position;
            transform.rotation = Quaternion.LookRotation(directionToBowStart);
        }
    }
}