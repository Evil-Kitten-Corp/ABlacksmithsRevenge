using UnityEngine;

namespace Tools
{
    public class VRArrowController : MonoBehaviour
    {
        [SerializeField] private GameObject midPointVisual, arrowPrefab, arrowSpawnPt;
        [SerializeField] private float arrowMaxSpeed = 10;
        [SerializeField] private AudioSource bowReleasedAudioSource;
        [SerializeField] private float aimAssistRadius = 30f;
        [SerializeField] private bool debug;
        
        public void PrepareArrow()
        {
            midPointVisual.SetActive(true);
        }

        private void DrawDebugBox()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            float boxSize = aimAssistRadius;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            Vector2 topLeft = screenCenter + new Vector2(-boxSize / 2, boxSize / 2);
            Vector2 topRight = screenCenter + new Vector2(boxSize / 2, boxSize / 2);
            Vector2 bottomLeft = screenCenter + new Vector2(-boxSize / 2, -boxSize / 2);
            Vector2 bottomRight = screenCenter + new Vector2(boxSize / 2, -boxSize / 2);

            Vector3 worldTopLeft = cam.ScreenToWorldPoint(new Vector3(topLeft.x, topLeft.y, cam.nearClipPlane + 0.1f));
            Vector3 worldTopRight = cam.ScreenToWorldPoint(new Vector3(topRight.x, topRight.y, cam.nearClipPlane + 0.1f));
            Vector3 worldBottomLeft = cam.ScreenToWorldPoint(new Vector3(bottomLeft.x, bottomLeft.y, cam.nearClipPlane + 0.1f));
            Vector3 worldBottomRight = cam.ScreenToWorldPoint(new Vector3(bottomRight.x, bottomRight.y, cam.nearClipPlane + 0.1f));

            Debug.DrawLine(worldTopLeft, worldTopRight, Color.yellow, 0.05f);
            Debug.DrawLine(worldTopRight, worldBottomRight, Color.yellow, 0.05f);
            Debug.DrawLine(worldBottomRight, worldBottomLeft, Color.yellow, 0.05f);
            Debug.DrawLine(worldBottomLeft, worldTopLeft, Color.yellow, 0.05f);
        }

        private void Update()
        {
            if (debug)
            {
                DrawDebugBox();
            }
        }

        public void ReleaseArrow(float strength)
        {
            bowReleasedAudioSource.Play();
            midPointVisual.SetActive(false);

            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.position = arrowSpawnPt.transform.position;
            arrow.transform.rotation = midPointVisual.transform.rotation;
            
            Transform target = FindTarget();
        
            VRArrowRotation arrowController = arrow.GetComponent<VRArrowRotation>();
            
            if (target != null)
            {
                arrowController.SetHomingTarget(target);
            }
            else
            {
                Rigidbody rb = arrow.GetComponent<Rigidbody>();
                rb.linearVelocity = midPointVisual.transform.forward * strength * arrowMaxSpeed;
                //rb.AddForce(midPointVisual.transform.forward * strength * arrowMaxSpeed);
            }
            
            Debug.Log($"Bow strength is {strength}, Target: {(target != null ? target.name : "None")}");
        }
        
        private Transform FindTarget()
        {
            Camera cam = Camera.main;
            if (cam == null) return null;

            float boxSize = aimAssistRadius; //assuming 1080p resolution
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    
            Transform bestTarget = null;
            float closestDistance = float.MaxValue;

            int rayCount = 5; //number of rays per axis (25 total in a 5x5 grid)
            float step = boxSize / (rayCount - 1);

            for (int x = 0; x < rayCount; x++)
            {
                for (int y = 0; y < rayCount; y++)
                {
                    Vector2 screenPos = screenCenter + new Vector2(x * step - boxSize / 2, y * step - boxSize / 2);
                    Ray ray = cam.ScreenPointToRay(screenPos);

                    if (Physics.Raycast(ray, out RaycastHit hit, 100f)) //100f being max range
                    {
                        if (hit.collider.CompareTag("Enemy"))
                        {
                            float distance = Vector3.Distance(cam.transform.position, hit.point);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                bestTarget = hit.transform;
                            }
                        }

                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 0.05f);
                    }
                    else
                    {
                        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 0.05f); //missed rays
                    }
                }
            }

            return bestTarget;
        }
    }
}