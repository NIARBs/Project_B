using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSight : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject head;

    [SerializeField] bool isTracking = true;

    void FixedUpdate()
    {
        TrackingMouse();
    }

    void TrackingMouse()
    {
        Vector3 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 playerPos = player.transform.position;
        Vector2 mousePos = new Vector2(cameraPos.x - playerPos.x, cameraPos.y - playerPos.y);

        float rad = Mathf.Atan2(mousePos.x, mousePos.y);
        float mouseRotate = (rad * 180) / Mathf.PI;

        head.transform.localEulerAngles = new Vector3(0, 0, (-mouseRotate + 90));
    }

    public void setTracking(bool tracking)
    {
        isTracking = tracking;
    }
}
