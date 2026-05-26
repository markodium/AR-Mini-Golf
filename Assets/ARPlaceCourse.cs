using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceCourse : MonoBehaviour
{
    public GameObject[] levelPrefabs;
    public GameObject winPanel;

    private int currentLevelIndex = 0;
    private ARRaycastManager raycastManager;
    private GameObject spawnedLevel;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    void Update()
    {
        if (spawnedLevel != null)
            return;

        Vector2 inputPosition;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Began)
                return;

            inputPosition = touch.position;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;
        }
        else
        {
            return;
        }

        if (raycastManager.Raycast(inputPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Debug.Log("Plane hit, spawning level");
            Pose hitPose = hits[0].pose;

            spawnedLevel = Instantiate(
                levelPrefabs[currentLevelIndex],
                hitPose.position,
                hitPose.rotation
            );
        } else
        {
            Debug.Log("No plane hit");
        }
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    public void NextLevel()
    {
        if (spawnedLevel != null)
            Destroy(spawnedLevel);

        currentLevelIndex++;

        if (currentLevelIndex >= levelPrefabs.Length)
        {
            currentLevelIndex = 0;
        }

        if (winPanel != null)
            winPanel.SetActive(false);

        spawnedLevel = null;
    }
}