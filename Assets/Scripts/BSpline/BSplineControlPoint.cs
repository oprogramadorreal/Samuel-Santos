using UnityEngine;

namespace ss
{
    public sealed class BSplineControlPoint : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private Material controlPointSelectedMaterial;

        private Material controlPointMaterial;

        private void Awake()
        {
            controlPointMaterial = meshRenderer.material;
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                meshRenderer.material = controlPointMaterial;
            }
        }

        private void OnMouseEnter()
        {
            meshRenderer.material = controlPointSelectedMaterial;
        }

        private void OnMouseExit()
        {
            if (!Input.GetMouseButton(0))
            {
                meshRenderer.material = controlPointMaterial;
            }
        }
    }
}
