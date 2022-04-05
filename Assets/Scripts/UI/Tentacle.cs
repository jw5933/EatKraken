using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    //tentacle
    [SerializeField] int length; //number of points
    LineRenderer lineRend;
    Vector3[] segmentPoses;
    Vector3[] targetPoses;
    Vector3[] segmentV; //smooth damp velocities

    Transform targetDir; //pointer
    [SerializeField] Transform endPos;
    //[SerializeField] float distanceBetweenPoints; //the points keep a distance away from each other
    [SerializeField] float segmentSpeedDifference;
    [SerializeField] float startSpeed;
    float[] segmentSpeeds; //how fast the point will come to a halt
    float segmentFraction;

    
    private void Start(){
        lineRend = GetComponent<LineRenderer>();
        lineRend.sortingLayerName = "Overlay";
        targetDir = transform.parent;
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        targetPoses = new Vector3[length];
        segmentV = new Vector3[length];
        segmentSpeeds = new float[length];

        segmentSpeeds[0] = startSpeed;
        for (int i = 1; i < length - 1; i++){
            segmentSpeeds[i] = segmentSpeeds[i-1] + segmentSpeedDifference;
        }

        segmentPoses[length - 1] = endPos.position;
        segmentFraction = 1f/((float)(length-1));
        //Debug.Log("length: " + length + ", segment fraction: " + segmentFraction);
    }

    public void Update(){
        //wiggleDir.locationRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);
        segmentPoses[0] = targetDir.position; //first position is pointer pos
        Vector3 endToPoint = Vector3.Normalize(segmentPoses[0] - segmentPoses[length-1]);
        float dist = Vector3.Distance(segmentPoses[0], segmentPoses[length-1]);
        //Debug.Log("tentacle end to point: " + endToPoint.ToString());
        for (int i = 1; i <length-1; i++){
            float targetDist = i*segmentFraction*dist;
            targetPoses[i] =  segmentPoses[0] - endToPoint*targetDist;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPoses[i], ref segmentV[i], segmentSpeeds[i]);
        }
        lineRend.SetPositions(segmentPoses);
    }

    /* void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(segmentPoses[segmentPoses.Length-1], segmentPoses[0]);
        Gizmos.color = Color.red;
        // Display the explosion radius when selected
        foreach(Vector3 pos in targetPoses){
            Gizmos.DrawSphere(pos, 0.5f);
        }
        
    } */

}
