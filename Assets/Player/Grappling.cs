using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Grappling : MonoBehaviour
{


    private Player player;

    [Header("References")]
    private FirstPersonController fpc;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    //public LayerMask whoIsEnemy;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grapplingDuration;
    public float grappleDelayTime;
    public float startSpeed;
    //public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    //[Header("Input")]
    //public KeyCode grappleKey = KeyCode.Mouse1;

    private bool grappling;
    private bool grapplingEnemy;
    private Transform grappedEnemy;


    private void Start()
    {
        fpc = GetComponent<FirstPersonController>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2)&&player.unlockables[(int)Player.abilities.hook]) StartGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
         if (grappling)
            lr.SetPosition(0, gunTip.position);
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        if (grappling) return;

        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            grapplingEnemy = hit.transform.CompareTag("Enemy");
            grappedEnemy = grapplingEnemy ? hit.transform : null;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }

        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        fpc.SetGrappling(true);

        if (grapplingEnemy)
        {
            StartCoroutine(GrapplingEnemy());
        }
        else
        {
            StartCoroutine(GrapplingStatic());
        }
       
        //Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;

        fpc.SetGrappling(false);
        //fpc.SaveSpeed();
    }

    private IEnumerator GrapplingStatic()
    {
        Vector3 moveDirection = (grapplePoint - transform.position).normalized;
        Vector3 lateralMoveDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);

        float elapsedTime = 0f;

        while (elapsedTime < grapplingDuration)
        {
            fpc.SetVelocity(lateralMoveDirection * startSpeed);
            fpc.SetVerticalVelocity(moveDirection.y * startSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StopGrapple();
    }

    private IEnumerator GrapplingEnemy()
    {
        // TODO : Залочить ии врага

        float elapsedTime = 0f;

        while (elapsedTime < grapplingDuration)
        {
            Vector3 moveDirection = (grappedEnemy.position - transform.position).normalized;
            Vector3 lateralMoveDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            fpc.SetVelocity(lateralMoveDirection * startSpeed);
            fpc.SetVerticalVelocity(moveDirection.y * startSpeed);

            // TODO : притягивание врага

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    //public bool IsGrappling()
    //{
    //    return grappling;
    //}

    //public Vector3 GetGrapplePoint()
    //{
    //    return grapplePoint;
    //}
}