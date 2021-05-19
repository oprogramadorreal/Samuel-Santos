using UnityEngine;

namespace ss
{
    public sealed class CollidableMusicalMesh : MonoBehaviour
    {
        [SerializeField]
        private MusicalMesh musicalMesh;

        [SerializeField]
        private MeshRenderer meshRenderer;

        [SerializeField]
        private MeshFilter meshFilter;

        [SerializeField]
        private PolygonCollider2D meshCollider;

        [SerializeField]
        private Color color = Color.red;

        private SideScroller sideScroller;

        private void OnEnable()
        {
            musicalMesh.MeshUpdatedEvent += MusicalMesh_MeshUpdatedEvent;
        }

        private void OnDisable()
        {
            musicalMesh.MeshUpdatedEvent -= MusicalMesh_MeshUpdatedEvent;
        }

        private void Start()
        {
            sideScroller = GetComponent<SideScroller>();
            meshRenderer.material.SetColor("_RendererColor", color);
        }

        private void MusicalMesh_MeshUpdatedEvent(object sender, System.EventArgs e)
        {
            UpdateMeshRendererAndCollider();
        }

        private void UpdateMeshRendererAndCollider()
        {
            meshFilter.mesh = musicalMesh.Mesh;

            if (meshCollider != null)
            {
                meshCollider.SetPath(0, musicalMesh.MeshContour2D);
            }

            InitSideScrollerOnce();
        }

        private void InitSideScrollerOnce()
        {
            if (sideScroller != null)
            {
                if (sideScroller.Init())
                {
                    sideScroller = null;
                }
            }
        }
    }
}
