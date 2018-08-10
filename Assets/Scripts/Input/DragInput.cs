using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragInput : MonoBehaviour {

    public Camera mainCamera;
	Touch curTouch;

    public delegate void Swipe(Vector3 touchPos);
    public delegate void TouchEnd();

    public Swipe OnSwipe;
    public TouchEnd OnTouchEnd;

    EventSystem curEventSystem;
    Vector3 touchPosition;
    bool swiping = false;
    float lastDragDist;

	// Use this for initialization
	void Start () {
        OnSwipe += ((Vector3 position) => { });
        OnTouchEnd += (() => { });
        curEventSystem = EventSystem.current;
    }
        
	// Update is called once per frame
	void Update () {

#if UNITY_EDITOR
        GameObject editor_selected = curEventSystem.currentSelectedGameObject;
        bool editor_overGUI = curEventSystem.IsPointerOverGameObject();

        if (editor_selected != null || editor_overGUI) {
            return;
        }

        if (Input.GetMouseButton (0)) {
            swiping = true;
            touchPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            touchPosition.z = 0;
            OnSwipe(touchPosition);
        } else {
            if (swiping) {
                OnTouchEnd();
                swiping = false;
            }
        }
        return;
#endif

        for (int i = 0; i < Input.touchCount; i++){
            
            // Current touch
            Touch t = Input.GetTouch (i);

            // We ignore all touches past the first touch
            if (i > 0) { return;}

            bool validTouch = false;
            GameObject selected = curEventSystem.currentSelectedGameObject;
            bool overGUI = curEventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

            if (selected == null && !overGUI) {
                validTouch = true;
            }

            if (validTouch) {
                if (t.phase == TouchPhase.Moved) {
                    touchPosition = mainCamera.ScreenToWorldPoint(t.position);
                    touchPosition.z = 0;
                    OnSwipe(touchPosition);
                }

                if (t.phase == TouchPhase.Ended) {
                    touchPosition = Vector3.zero;
                    OnTouchEnd();
                }
            }
        }
	
    }
}
