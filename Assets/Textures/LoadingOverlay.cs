using UnityEngine;
using UnityEngine.Serialization;

namespace Textures
{
    [RequireComponent(typeof(MeshFilter))]
    public class LoadingOverlay : MonoBehaviour 
    {
        private bool _fading;
        private float _fadeTimer;

        [FormerlySerializedAs("in_alpha")] public float inAlpha = 1.0f;
        [FormerlySerializedAs("out_alpha")] public float outAlpha = 0.0f;

        private Color _fromColor;
        private Color _toColor;
        private Material _material;

        private void Start()
        {
            ReverseNormals(gameObject);
            _fading = false;
            _fadeTimer = 0;

            _material = gameObject.GetComponent<Renderer>().material;
            _fromColor = _material.color;
            _toColor = _material.color;
        }

        private void Update()
        {
            if (_fading == false)
                return;

            _fadeTimer += Time.deltaTime;
            _material.color = Color.Lerp(_fromColor, _toColor, _fadeTimer);
        
            if (_material.color == _toColor)
            {
                _fading = false;
                _fadeTimer = 0;
            }
        }

        public void FadeOut()
        {
            // Fade the overlay to `out_alpha`.
            _fromColor.a = inAlpha;
            _toColor.a = outAlpha;
        
            if (_toColor != _material.color)
            {
                _fading = true;
            }
        }

        public void FadeIn()
        {
            // Fade the overlay to `in_alpha`.
            _fromColor.a = outAlpha;
            _toColor.a = inAlpha;
        
            if (_toColor != _material.color)
            {
                _fading = true;
            }
        }

        public static void ReverseNormals(GameObject gameObject)
        {
            // Renders interior of the overlay instead of exterior.
            // Included for ease-of-use. 
        
            MeshFilter filter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
        
            if (filter != null)
            {
                Mesh mesh = filter.mesh;
                Vector3[] normals = mesh.normals;
            
                for (int i = 0; i < normals.Length; i++)
                    normals[i] = -normals[i];
            
                mesh.normals = normals;

                for (int m = 0; m < mesh.subMeshCount; m++)
                {
                    int[] triangles = mesh.GetTriangles(m);
                
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        (triangles[i + 0], triangles[i + 1]) = (triangles[i + 1], triangles[i + 0]);
                    }
                
                    mesh.SetTriangles(triangles, m);
                }
            }
        }
    }
}
