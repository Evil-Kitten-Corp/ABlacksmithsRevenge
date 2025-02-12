using UnityEngine;

namespace Game_Systems
{
    [RequireComponent(typeof(BoxCollider))]
    public class AreaBoundLimits : MonoBehaviour
    {
        [Header("Needs a trigger collider")] 
        public Transform playerRig;
        public Vector3 offset;

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Tool"))
            {
                other.transform.position = playerRig.position + offset;
            }
        }
    }
}