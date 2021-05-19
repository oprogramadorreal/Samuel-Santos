using UnityEngine;

namespace ss
{
    public sealed class EventsManager : MonoBehaviour
    {
        [SerializeField]
        private SideScroller floorSideScroller;

        [SerializeField]
        private LevelManager levelPartsManager;

        private void OnEnable()
        {
            floorSideScroller.InitializedEvent += FloorSideScroller_InitializedEvent;
            floorSideScroller.ScrollLeftEvent += FloorSideScroller_ScrollLeftEvent;
            floorSideScroller.ScrollRightEvent += FloorSideScroller_ScrollRightEvent;
        }

        private void OnDisable()
        {
            floorSideScroller.InitializedEvent -= FloorSideScroller_InitializedEvent;
            floorSideScroller.ScrollLeftEvent -= FloorSideScroller_ScrollLeftEvent;
            floorSideScroller.ScrollRightEvent -= FloorSideScroller_ScrollRightEvent;
        }

        private void FloorSideScroller_InitializedEvent(object sender, SideScroller.ScrollEventArgs e)
        {
            levelPartsManager.OnSideScrollerInitializedEvent(e.PositionX, e.Offset);
        }

        private void FloorSideScroller_ScrollLeftEvent(object sender, SideScroller.ScrollEventArgs e)
        {
            levelPartsManager.OnScrollLeftEvent(e.PositionX, e.Offset);
        }

        private void FloorSideScroller_ScrollRightEvent(object sender, SideScroller.ScrollEventArgs e)
        {
            levelPartsManager.OnScrollRightEvent(e.PositionX, e.Offset);
        }
    }
}
