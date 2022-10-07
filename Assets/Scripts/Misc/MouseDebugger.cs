using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseDebugger : MonoBehaviour
{
    Canvas canvas;
    EventSystem eventSystem;
    GraphicRaycaster raycaster;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        canvas = FindObjectOfType<Canvas>();
        eventSystem = FindObjectOfType<EventSystem>();
        raycaster = FindObjectOfType<GraphicRaycaster>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            List<RaycastResult> results = new List<RaycastResult>();

            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.position = Input.mousePosition;
            raycaster.Raycast(eventData, results);

            foreach (RaycastResult result in results)
            {
                GameObject obj = result.gameObject;
                Debug.Log("Hit: " + obj.name + " of " + obj.transform.parent.gameObject.name);
            }
        }
    }

    void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        canvas = FindObjectOfType<Canvas>();
        eventSystem = FindObjectOfType<EventSystem>();
        raycaster = FindObjectOfType<GraphicRaycaster>();
    }
}
