using UnityEngine;

namespace Tools
{
    public class VRArrowController : MonoBehaviour
    {
        [SerializeField] private GameObject midPointVisual, arrowPrefab, arrowSpawnPt;
        [SerializeField] private float arrowMaxSpeed = 10;
        [SerializeField] private AudioSource bowReleasedAudioSource;
        
        public void PrepareArrow()
        {
            midPointVisual.SetActive(true);
        }

        public void ReleaseArrow(float strength)
        {
            bowReleasedAudioSource.Play();
            midPointVisual.SetActive(false);

            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.position = arrowSpawnPt.transform.position;
            arrow.transform.rotation = midPointVisual.transform.rotation;
            
            Transform target = FindBestTarget(arrowSpawnPt.transform.forward, 
                arrowSpawnPt.transform.position);
        
            VRArrowRotation arrowController = arrow.GetComponent<VRArrowRotation>();
            
            if (target != null)
            {
                arrowController.SetHomingTarget(target);
            }
            else
            {
                arrowController.SetSpeed(strength * arrowMaxSpeed);
            }
            
            Debug.Log($"Bow strength is {strength}, Target: {(target != null ? target.name : "None")}");
        }
        
        private Transform FindBestTarget(Vector3 arrowDirection, Vector3 arrowPosition)
        {
            float maxAngle = 30f; 
            float maxDistance = 50f; 

            Transform bestTarget = null;
            float bestDistance = Mathf.Infinity;

            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                Vector3 toEnemy = (enemy.transform.position - arrowPosition).normalized;
                float angle = Vector3.Angle(arrowDirection, toEnemy);

                //is the enemy within the allowed angle?
                if (angle < maxAngle)
                {
                    float distance = Vector3.Distance(arrowPosition, enemy.transform.position);

                    if (distance < bestDistance && distance <= maxDistance)
                    {
                        bestTarget = enemy.transform;
                        bestDistance = distance;
                    }
                }
            }

            return bestTarget;
        }
    }
}