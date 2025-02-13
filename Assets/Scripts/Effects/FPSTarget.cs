using UnityEngine;
using UnityEngine.Serialization;

namespace Effects
{
    public class FPSTarget : MonoBehaviour
    {
        [FormerlySerializedAs("FPS")] [SerializeField] private int fps = 72;

        private void Start()
        {
            Application.targetFrameRate = fps;
        }
    }
}
