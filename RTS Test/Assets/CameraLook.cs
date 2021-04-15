using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CameraLook : MonoBehaviour
{
    private Transform target;
    [SerializeField] private float distance;
    private Camera cam;

    // Start is called before the first frame update
    private void Awake()
    {
        //target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        int layer_mask = LayerMask.GetMask("Ground");
        //Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, layer_mask))
        {
            //distance = Vector3.Distance(hit.point, target.transform.position);
            //if (distance < 5)
            //{
            //    RayClose();
            //}
            //else if (distance > 50)
            //{
            //    //RayNoHit();
            //}
            //else
            //{
            transform.parent = null;
            transform.position = (hit.point);
            //}
        }
        else
        {
            //RayNoHit();
        }
    }

    //private void RayNoHit()
    //{
    //    bool Aiming = target.GetComponent<Move>().AimingThroughSights;
    //    if (!Aiming)
    //    {
    //        transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
    //        transform.parent = cam.transform;
    //        transform.localPosition = new Vector3(0.23f, -0.17f, 4.5f);
    //    }
    //    else
    //    {
    //        RayClose();
    //    }
    //}

    private void RayClose()
    {
        transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
        transform.parent = cam.transform;
        transform.localPosition = new Vector3(0, 0, 4.5f);
    }
}