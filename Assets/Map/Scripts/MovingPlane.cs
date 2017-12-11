using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlane : MonoBehaviour {
    public GameObject[] routePoints;
    private int curRoutePoint = 0;

    public float speed = 15f;

    public float bobSpeed = 2;
    public  float bobAngleMax = 5;
    private bool isBobAngleIncreasing;
	// Use this for initialization
	void Start () {
        transform.localEulerAngles.Set(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        isBobAngleIncreasing = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (routePoints.Length == 0)
        {
            return;
        }

        if (Vector3.Distance(transform.position, routePoints[curRoutePoint].transform.position) < 0.1f)
        {
            curRoutePoint = (curRoutePoint + 1) % routePoints.Length;
        }

        Vector3 needToMove = routePoints[curRoutePoint].transform.position - transform.position;
        float distanceCanMove = speed * Time.deltaTime;
        float distance = needToMove.magnitude;
        float distanceToMove = Mathf.Min(distanceCanMove, distance);
        Vector3 toMove = needToMove.normalized * distanceToMove;
        transform.Translate(toMove);

        if ((transform.rotation.eulerAngles.x > bobAngleMax && (transform.rotation.eulerAngles.x < 180)
            || (transform.rotation.eulerAngles.x > 180 && transform.rotation.eulerAngles.x < 360 - bobAngleMax)))
        {
            isBobAngleIncreasing = !isBobAngleIncreasing;
        }

        float toBob = isBobAngleIncreasing ? Time.deltaTime * bobSpeed : -1 * Time.deltaTime * bobSpeed;
        transform.RotateAround(transform.position, transform.right, toBob);
    }
}
