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
            
            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.AddForce(midPointVisual.transform.forward * strength * arrowMaxSpeed, ForceMode.Impulse);
            
            Debug.Log($"Bow strength is {strength}");
        }
    }
}