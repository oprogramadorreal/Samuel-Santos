using UnityEngine;

namespace ss
{
    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private PlayerController player;

        [SerializeField]
        private GameObject[] initialLevelPartsPrefabs; // tutorial

        [SerializeField]
        private GameObject[] levelPartsPrefabs;

        private GameObject leftPart = null;
        private GameObject currentPart = null;
        private GameObject rightPart = null;

        private int partSize = 0;

        public void OnSideScrollerInitializedEvent(float xPosition, float offset)
        {
            partSize = Mathf.FloorToInt(offset);

            leftPart = CreateLevelPartFor(xPosition - offset);
            currentPart = CreateLevelPartFor(xPosition);
            rightPart = CreateLevelPartFor(xPosition + offset);
        }

        public void OnScrollLeftEvent(float xPosition, float offset)
        {
            if (partSize == 0)
            {
                return; // not initialized yet
            }

            if (rightPart != null)
            {
                Destroy(rightPart);
            }

            rightPart = currentPart;
            currentPart = leftPart;
            leftPart = CreateLevelPartFor(xPosition - offset);
        }

        public void OnScrollRightEvent(float xPosition, float offset)
        {
            if (partSize == 0)
            {
                return; // not initialized yet
            }

            if (leftPart != null)
            {
                Destroy(leftPart);
            }

            leftPart = currentPart;
            currentPart = rightPart;
            rightPart = CreateLevelPartFor(xPosition + offset);
        }

        private GameObject CreateLevelPartFor(float xPosition)
        {
            Debug.Log(xPosition);

            if (xPosition < 0.0f)
            {
                return null;
            }

            var newLevelPart = Instantiate(ChooseLevelPartPrefabFor(xPosition), transform);
            var newLevelPartComp = newLevelPart.GetComponent<LevelPart>();

            newLevelPart.transform.position = new Vector3(
                xPosition + newLevelPartComp.GetXOffset(),
                newLevelPart.transform.position.y,
                newLevelPart.transform.position.z
            );

            SetupAndDetachLevelPartObjects(newLevelPart);

            return newLevelPart;
        }

        private void SetupAndDetachLevelPartObjects(GameObject newLevelPart)
        {
            var objects = newLevelPart.GetComponentsInChildren<LevelPartObject>();

            foreach (var o in objects)
            {
                o.Setup(newLevelPart, player);
                o.transform.SetParent(null, true);
            }
        }

        private GameObject ChooseLevelPartPrefabFor(float xPosition)
        {
            var xPositionAsInt = Mathf.FloorToInt(xPosition);

            if (xPosition < initialLevelPartsPrefabs.Length * partSize)
            {
                return initialLevelPartsPrefabs[xPositionAsInt / partSize];
            }

            return levelPartsPrefabs[(xPositionAsInt / partSize) % levelPartsPrefabs.Length];
        }
    }
}
