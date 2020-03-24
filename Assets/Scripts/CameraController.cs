using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update

    public float panSpeed = 50;
    public float zoomSpeed = 50;
    public float camHeight = 32;

    public Transform cameraOrigin;
    public float panEdgeDistance = 10;

    public bool disableMouse = false;

    void Start()
    {
        if (cameraOrigin == null)
        {
            Debug.LogError("I need camera origin");
            return;
        }

        Vector3 cameraPos = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(cameraPos.x, camHeight, cameraPos.z);
    }

    private void FixedUpdate()
    {

    }

    private void Update()
    {
 
        Vector3 camOrigin = cameraOrigin.position;
        Vector3 mouseScreenPos = Input.mousePosition;

        bool goDown = mouseScreenPos.y < panEdgeDistance && !disableMouse;
        bool goUp = mouseScreenPos.y > Screen.height - panEdgeDistance && !disableMouse;

        bool goLeft = mouseScreenPos.x < panEdgeDistance && !disableMouse;
        bool goRight = mouseScreenPos.x > Screen.width - panEdgeDistance && !disableMouse;

        Vector3 newMovementDirection = new Vector3();

        Vector3 camToOrigin = Camera.main.transform.position - cameraOrigin.transform.position;
        Vector3 leftRelToCam = (Quaternion.AngleAxis(90, Vector3.up) * (camToOrigin)).normalized;
        Vector3 rightRelToCam = (Quaternion.AngleAxis(-90, Vector3.up) * (camToOrigin)).normalized;

        if (Input.GetKey(KeyCode.W) || goUp)
        {
            newMovementDirection += -(camToOrigin).normalized * panSpeed;
        }
        if (Input.GetKey(KeyCode.S) || goDown)
        {
            newMovementDirection += (camToOrigin).normalized * panSpeed;
        }
        if (Input.GetKey(KeyCode.A) || goLeft)
        {
            newMovementDirection += leftRelToCam * panSpeed;
        }
        if (Input.GetKey(KeyCode.D) || goRight)
        {
            newMovementDirection += rightRelToCam * panSpeed;
        }
        newMovementDirection.y = 0;

        cameraOrigin.transform.position = Vector3.Lerp(cameraOrigin.transform.position, camOrigin + newMovementDirection, Time.deltaTime);

        bool noUIcontrolsInUse = EventSystem.current.currentSelectedGameObject == null;
        if (!noUIcontrolsInUse) return;

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            camHeight += zoomSpeed * Time.deltaTime;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            camHeight -= zoomSpeed * Time.deltaTime;
        }

        // Debug.Log(newMovementDirection);

        Camera.main.transform.position = cameraOrigin.transform.position + (camToOrigin.normalized * camHeight);
        Camera.main.transform.rotation = Quaternion.LookRotation(-camToOrigin);




        if (Input.GetMouseButton(2))
        {
            Camera.main.transform.RotateAround(cameraOrigin.transform.position, transform.right, Input.GetAxis("Mouse Y") * 200 * Time.deltaTime);
            Camera.main.transform.RotateAround(cameraOrigin.transform.position, transform.up, Input.GetAxis("Mouse X") * 200 * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {

        var angle = Vector3.Angle(Camera.main.transform.position - cameraOrigin.transform.position,  Vector3.up);
        if (angle < 20 || angle > 80)
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, 5, Camera.main.transform.position.z);
        }
    }
}
