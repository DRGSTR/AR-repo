using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown navigationTargetDropDown;
    [SerializeField]
    private List<Target> navigationTargetObjects = new List<Target>();

    //[SerializeField]
    //private Camera topDownCamera;
    //[SerializeField]
    //private GameObject navTargetObject;

    private NavMeshPath path;
    private LineRenderer line;
    private Vector3 targetPosition = Vector3.zero;
    
    private bool lineToggle = false;

    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        line.enabled = lineToggle;
    }

    private void Update()
    {
        //if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        //{
        //    lineToggle = !lineToggle;           
        //}

        //if(lineToggle)
        //{
        //    NavMesh.CalculatePath(transform.position, navTargetObject.transform.position, NavMesh.AllAreas, path);
        //    line.positionCount = path.corners.Length;
        //    line.enabled = true;
        //}

        if(lineToggle && targetPosition != Vector3.zero)
        {
            NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, path);
            line.positionCount = path.corners.Length;
            line.SetPositions(path.corners);
        }
    }

    public void SetCurrentNavigationTarget(int selectedValue)
    {
        targetPosition = Vector3.zero;
        string selectedText = navigationTargetDropDown.options[selectedValue].text;
        Target currentTarget = navigationTargetObjects.Find(x =>  x.Name.Equals(selectedText));
        if(currentTarget != null )
        {
            targetPosition = currentTarget.positionObject.transform.position;
        }
    }

    public void ToggleVisibility()
    {
        lineToggle = !lineToggle;
        line.enabled = !lineToggle;
    }

} // class
