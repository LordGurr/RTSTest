using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitRTS : MonoBehaviour
{
    private GameObject selected;
    [SerializeField] private NavMeshAgent navComponent;
    private bool CarryingSomething = false;
    private GameObject rock;

    // Start is called before the first frame update
    private void Awake()
    {
        rock = this.gameObject.transform.GetChild(1).gameObject;
        rock.SetActive(false);
        selected = transform.Find("Selected").gameObject;
        selected.SetActive(false);
        navComponent.speed = Random.Range(navComponent.speed - 1, navComponent.speed + 1);
        navComponent.acceleration = Random.Range(navComponent.acceleration - 1, navComponent.acceleration + 1);
    }

    public void SetSelectedVisible(bool visible)
    {
        selected.SetActive(visible);
    }

    public void GoToPos(Vector3 pos)
    {
        navComponent.SetDestination(pos);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (navComponent.isStopped)
        //{
        Debug.Log(other.gameObject.name);
        if (other.gameObject.layer == 10)
        {
            if (!CarryingSomething)
            {
                CarryingSomething = true;
                rock.SetActive(true);
            }
        }

        //}
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name + " Collided");
        if (collision.gameObject.layer == 10)
        {
            if (!CarryingSomething)
            {
                CarryingSomething = true;
                rock.SetActive(true);
            }
        }
    }

    public Vector3 unitPos()

    {
        return transform.position;
    }
}