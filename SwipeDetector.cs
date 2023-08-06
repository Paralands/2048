using UnityEngine;

public class SwipeDetector : MonoBehaviour
{   
    public static event OnSwipeInput SwipeEvent;
    public delegate void OnSwipeInput(Vector2 direction);
    private Vector2 tapPosition;
    private Vector2 swipeDelta;
    private readonly float deadZone = 30;
    private bool IsSwiping;
    private bool IsMobile;
    void Start()
    {
        IsMobile = Application.isMobilePlatform;
    }
    void Update()
    {
        if (GameManager.Instance.GameStarted)
        {
            if(!IsMobile)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    IsSwiping = true;
                    tapPosition = Input.mousePosition;
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    ResetSwipe();
                }
            }
            else if(Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began) //аналог GetMouseButtonDown
                    {
                        IsSwiping = true;
                        tapPosition = Input.GetTouch(0).position;
                    }
                    else if(Input.GetTouch(0).phase == TouchPhase.Canceled || Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        ResetSwipe();
                    }
                }

            CheckSwipe(); 
        }
    }
    private void CheckSwipe()
    {
        swipeDelta = Vector2.zero;
        if (IsSwiping)
        {
            if(!IsMobile && Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - tapPosition;
            else if (IsMobile && Input.touchCount > 0)
                swipeDelta = Input.GetTouch(0).position - tapPosition;
        }

        if (swipeDelta.magnitude > deadZone)
        {
            if (SwipeEvent != null)
            {
                if(Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    SwipeEvent(swipeDelta.x > 0 ? Vector2.right : Vector2.left);
                else SwipeEvent(swipeDelta.y > 0 ? Vector2.up : Vector2.down);
            }

            ResetSwipe();
        }
        
    }
    private void ResetSwipe()
    {
        IsSwiping = false;
        tapPosition = Vector2.zero;
        swipeDelta = Vector2.zero;
    }
}
