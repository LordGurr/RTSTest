using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//using UnityEngine.UIElements;
using UnityEditor.AI;

public class GameRTSController : MonoBehaviour
{
    private Vector3 startPosition = new Vector3(0, 0, 0);
    private Vector3 endPosition = new Vector3(0, 0, 0);
    private List<UnitRTS> selectedUnitRtsList;
    private List<UnitRTS> allMyUnits;
    private GameObject mainCam;
    private Camera actualCam;
    [SerializeField] private RectTransform selectionBox;
    private Vector2 boxStartPos;
    private Vector2 boxEndPos;

    //private Rigidbody cam;
    [SerializeField] private float speed;

    private float camOriginalPos;
    private bool keypressed = false;
    [SerializeField] private GameObject troop;
    [SerializeField] private GameObject fakeBush;
    [SerializeField] private GameObject rock;
    [SerializeField] private GameObject spiderMans;
    [SerializeField] private float panBorderThickness = 5;
    private float xDivider = 120;
    private float yDivider = 100;
    [SerializeField] private float[] panLimit = new float[4];
    private float scrollspeed = 20f;
    [SerializeField] private Vector2 scrollLimit;

    // Start is called before the first frame update
    private void Awake()
    {
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = false;
        int amountOfSomething = Random.Range(2, 4);
        for (int i = 0; i < amountOfSomething; i++)
        {
            for (int a = 0; a < 50; a++)
            {
                float xpos = Random.Range(-35, 35);
                float zpos = Random.Range(-35, 35);
                if (!Physics.CheckCapsule(new Vector3(xpos, 7, zpos), new Vector3(xpos, 10f, zpos), 4))
                {
                    Instantiate(spiderMans, new Vector3(xpos, 1f, zpos), Quaternion.Euler(Quaternion.identity.x, Quaternion.identity.y, Random.Range(-5f, 5f)));
                    break;
                }
            }
        }
        amountOfSomething = Random.Range(15, 30);
        for (int i = 0; i < amountOfSomething; i++)
        {
            for (int a = 0; a < 50; a++)
            {
                float xpos = Random.Range(-45f, 45f);
                float zpos = Random.Range(-45f, 45f);
                if (!Physics.CheckCapsule(new Vector3(xpos, 7, zpos), new Vector3(xpos, 10f, zpos), 4))
                {
                    Instantiate(fakeBush, new Vector3(xpos, 1f, zpos), Quaternion.Euler(Quaternion.identity.x, Random.Range(0f, 360), Quaternion.identity.z));
                    break;
                }
            }
        }
        amountOfSomething = Random.Range(3, 5);
        for (int i = 0; i < amountOfSomething; i++)
        {
            for (int a = 0; a < 50; a++)
            {
                float xpos = Random.Range(-40f, 40);
                float zpos = Random.Range(-40, 40);
                if (!Physics.CheckCapsule(new Vector3(xpos, 7, zpos), new Vector3(xpos, 10f, zpos), 5))
                {
                    Instantiate(rock, new Vector3(xpos, 1f, zpos), Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
                    break;
                }
            }
        }
        amountOfSomething = Random.Range(5, 15);
        for (int i = 0; i < amountOfSomething; i++)
        {
            for (int a = 0; a < 50; a++)
            {
                float xpos = Random.Range(-45f, 45f);
                float zpos = Random.Range(-45f, 45f);
                if (!Physics.CheckCapsule(new Vector3(xpos, 7, zpos), new Vector3(xpos, 10f, zpos), 3))
                {
                    Instantiate(troop, new Vector3(xpos, 1, zpos), Quaternion.identity);
                    break;
                }
            }
        }
        selectedUnitRtsList = new List<UnitRTS>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera")/*.GetComponent<Rigidbody>()*/;
        camOriginalPos = mainCam.transform.position.y;
        //cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Rigidbody>();
        actualCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        NavMeshBuilder.BuildNavMesh();
        allMyUnits = new List<UnitRTS>();
        GameObject[] tempUnit = GameObject.FindGameObjectsWithTag("Troop");
        for (int i = 0; i < tempUnit.Length; i++)
        {
            UnitRTS unitRTS = tempUnit[i].GetComponent<UnitRTS>();
            if (unitRTS != null)
            {
                allMyUnits.Add(unitRTS);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            pos.z += speed * pos.y / scrollLimit[0] * Time.deltaTime;
        }
        else if (Input.mousePosition.y >= Screen.height - Screen.height / panBorderThickness)
        {
            pos.z += (speed + ((Input.mousePosition.y * pos.y / scrollLimit[0]) / yDivider)) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            pos.z -= speed * pos.y / scrollLimit[0] * Time.deltaTime;
        }
        else if (Input.mousePosition.y <= Screen.height / panBorderThickness)
        {
            pos.z -= (speed + (((Screen.height - Input.mousePosition.y) * pos.y / scrollLimit[0]) / yDivider)) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            pos.x += speed * pos.y / scrollLimit[0] * Time.deltaTime;
        }
        else if (Input.mousePosition.x >= Screen.width - Screen.width / panBorderThickness)
        {
            pos.x += (speed + (((Input.mousePosition.x) * pos.y / scrollLimit[0])) / xDivider) * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            pos.x -= speed * pos.y / scrollLimit[0] * Time.deltaTime;
        }
        else if (Input.mousePosition.x <= Screen.width / panBorderThickness)
        {
            pos.x -= (speed + (((Screen.width - Input.mousePosition.y) * pos.y / scrollLimit[0])) / xDivider) * Time.deltaTime;
        }
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollspeed * 100f * Time.deltaTime;

        pos.x = Mathf.Clamp(pos.x, panLimit[0], panLimit[1]);
        pos.z = Mathf.Clamp(pos.z, panLimit[2], panLimit[3]);
        pos.y = Mathf.Clamp(pos.y, scrollLimit.x, scrollLimit.y);

        //Debug.Log(speed);
        //float xDirection = Input.GetAxisRaw("Horizontal");
        //float yDirection = Input.GetAxisRaw("Vertical");
        //int layer_mask = LayerMask.GetMask("Ground", "Enemy");
        //new Vector2(xDirection * speed, cam.velocity.y);
        //cam.AddForce(transform.right * xDirection);
        //cam.AddForce(transform.up * yDirection);
        //keypressed = false;
        //if (Input.GetKey(KeyCode.W) == true && Input.GetKey(KeyCode.D) == true)
        //{
        //    //cam.velocity = transform.up * speed;
        //    cam.velocity = Vector3.Normalize(transform.up + transform.right) * speed;
        //    keypressed = true;
        //}
        //else if (Input.GetKey(KeyCode.W) == true && Input.GetKey(KeyCode.A) == true)
        //{
        //    //cam.velocity = -transform.right * speed;
        //    cam.velocity = Vector3.Normalize(transform.up - transform.right) * speed;
        //    keypressed = true;
        //}
        //else if (Input.GetKey(KeyCode.S) == true && Input.GetKey(KeyCode.A) == true)
        //{
        //    //cam.velocity = -transform.up * speed;
        //    cam.velocity = Vector3.Normalize(-transform.up - transform.right) * speed;
        //    keypressed = true;
        //    keypressed = true;
        //}
        //else if (Input.GetKey(KeyCode.S) == true && Input.GetKey(KeyCode.D) == true)
        //{
        //    //cam.velocity = transform.right * speed;
        //    cam.velocity = Vector3.Normalize(-transform.up + transform.right) * speed;
        //    keypressed = true;
        //    keypressed = true;
        //}
        //else if (Input.GetKey(KeyCode.W) == true)
        //{
        //    //cam.velocity = Vector3.Normalize(transform.up + -transform.right) * speed;
        //    cam.velocity = transform.up * speed;
        //    keypressed = true;
        //}
        //else if (Input.GetKey(KeyCode.D) == true)
        //{
        //    //cam.velocity = Vector3.Normalize(transform.up + transform.right) * speed;
        //    cam.velocity = transform.right * speed;
        //    keypressed = true;
        //}
        //else if (Input.GetKey(KeyCode.A) == true)
        //{
        //    //cam.velocity = Vector3.Normalize((-transform.up + -transform.right) * speed);
        //    cam.velocity = -transform.right * speed;
        //    keypressed = true;
        //}
        //else if (Input.GetKey(KeyCode.S) == true)
        //{
        //    //cam.velocity = Vector3.Normalize((-transform.up + transform.right) * speed);
        //    cam.velocity = -transform.up * speed;
        //    keypressed = true;
        //}

        //if (!keypressed)
        //{
        //    cam.velocity *= 0.99f;// * Time.deltaTime;
        //}

        //mainCam.transform.position = new Vector3(mainCam.transform.position.x, camOriginalPos, mainCam.transform.position.z);
        transform.position = pos;
        if (Input.GetMouseButtonDown(1))
        {
            foreach (UnitRTS unitRTS1 in selectedUnitRtsList)
            {
                unitRTS1.SetSelectedVisible(false);
            }
            selectedUnitRtsList.Clear();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = actualCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (selectedUnitRtsList.Count > 0)
                {
                    for (int i = 0; i < selectedUnitRtsList.Count; i++)
                    {
                        selectedUnitRtsList[i].GoToPos(hit.point);
                    }
                }
                startPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            boxStartPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0)) //new Vector3(startPosition.x, startPosition.y - 5, startPosition.z)
        {
            Ray ray = actualCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
                Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);
                foreach (UnitRTS unit in allMyUnits)
                {
                    Vector3 screenPos = actualCam.WorldToScreenPoint(unit.unitPos());
                    if (screenPos.x > min.x && screenPos.x < max.x)
                    {
                        if (screenPos.y > min.y && screenPos.y < max.y)
                        {
                            selectedUnitRtsList.Add(unit);
                            unit.SetSelectedVisible(true);
                        }
                    }
                }

                //endPosition = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                //Collider[] colliderArray = Physics.OverlapBox(Vector3.Lerp(startPosition, endPosition, 0.5f), new Vector3(Mathf.Abs(startPosition.x - endPosition.x), Mathf.Abs(startPosition.y - startPosition.y), Mathf.Abs(startPosition.z - endPosition.z)), transform.rotation/*, endPosition*/);
                ////foreach (UnitRTS unitRTS1 in selectedUnitRtsList)
                ////{
                ////    unitRTS1.SetSelectedVisible(false);
                ////}
                ////selectedUnitRtsList.Clear();

                //foreach (Collider collider in colliderArray)
                //{
                //    UnitRTS unitRTS = collider.GetComponent<UnitRTS>();
                //    if (unitRTS != null)
                //    {
                //        selectedUnitRtsList.Add(unitRTS);
                //    }
                //}
                //foreach (UnitRTS unitRTS1 in selectedUnitRtsList)
                //{
                //    Debug.Log(unitRTS1);
                //    unitRTS1.SetSelectedVisible(true);
                //}
                ////Debug.Log("Mouse clicked");
            }
            Debug.Log("StartPos:" + startPosition + " EndPos" + hit.point);

            selectionBox.gameObject.SetActive(false);
        }
    }

    private void UpdateSelectionBox(Vector2 mousePos)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
        {
            selectionBox.gameObject.SetActive(true);
        }

        float width = mousePos.x - boxStartPos.x;
        float heigth = mousePos.y - boxStartPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(heigth));
        selectionBox.anchoredPosition = boxStartPos + new Vector2(width / 2, heigth / 2);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.Lerp(startPosition, endPosition, 0.5f), new Vector3(Mathf.Abs(startPosition.x - endPosition.x), Mathf.Abs(startPosition.y - startPosition.y), Mathf.Abs(startPosition.z - endPosition.z))/*, Quaternion.Euler(0.0f, 45f, 0f), endPosition*/);
    }
}