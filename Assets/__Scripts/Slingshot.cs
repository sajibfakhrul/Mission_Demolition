using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;

    public GameObject projLinePrefab;  // new edit a



    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    private void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter()
    {
        //print("Slingshot: OnMouseEnter()");
        launchPoint.SetActive(true);
    }


     void OnMouseExit()
    {
        //print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);

    }

    private void OnMouseDown()
    {
        aimingMode = true;
        projectile= Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        if (!aimingMode) return;

        Vector3 mousePos2D= Input.mousePosition;
        mousePos2D.z= -Camera.main.transform.position.z; 
        Vector3 mousePos3D= Camera.main.ScreenToWorldPoint(mousePos2D);


        Vector3 mouseDelta = mousePos3D - launchPos;

        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        
        Vector3 projPros = launchPos +mouseDelta;
        projectile.transform.position=projPros;

        if ( Input.GetMouseButtonUp(0) )
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot); // Switch to slingshot view immediately before setting POI

            FollowCam.POI = projectile; // Set the _MainCamera POI

            Instantiate<GameObject>(projLinePrefab,projectile.transform); // new edit b
            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }

    }    
    
}
