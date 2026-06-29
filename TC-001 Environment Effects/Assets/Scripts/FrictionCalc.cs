using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrictionCalc : MonoBehaviour
{
    [Header("Ground Check")]
    public float PlayerHeight;

    [Header("Dust")]
    public LayerMask whatIsDust;
    bool groundedDust;
    public float dustFriction;

    [Header("Plains")]
    public LayerMask whatIsPlains;
    bool groundedPlains;
    public float plainsFriction;

    [Header("Ice")]
    public LayerMask whatIsIce;
    bool groundedIce;
    public float iceFriction;

    [Header("Clean")]
    public LayerMask whatIsClean;
    bool groundedClean;
    public float cleanFriction;

    StatCheck stat;
    float maxSlopeAngle;
    RaycastHit slopeHit;

    //calculates the drag/friction of different surfaces
    public float DragCalc()
    {
        float currentFrict;
        groundedDust = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsDust);
        groundedPlains = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsPlains);
        groundedIce = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsIce);
        groundedClean = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsClean);
        if (groundedDust)
        { currentFrict = dustFriction; }
        else if (groundedPlains)
        { currentFrict = plainsFriction; }
        else if (groundedIce)
        { currentFrict = iceFriction; }
        else if (groundedClean)
        { currentFrict = cleanFriction; }
        else { currentFrict = 0; }

        return currentFrict;
    }

    public bool GroundedCheck()
    {
        bool grounded;
        groundedDust = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsDust);
        groundedPlains = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsPlains);
        groundedIce = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsIce);
        groundedClean = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f + 0.2f, whatIsClean);
        if (groundedDust || groundedPlains || groundedIce || groundedClean)
        { grounded = true; }
        else { grounded = false; }

        return grounded;
    }

    public bool SlopedCheck()
    {
        stat = gameObject.GetComponent<StatCheck>();
        maxSlopeAngle = stat.SlopeCheck(gameObject.tag);
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, PlayerHeight * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
}
