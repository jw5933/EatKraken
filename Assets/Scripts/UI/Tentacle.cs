using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    //tentacle
    [SerializeField] bool anchored;
    [SerializeField] int length; //number of points
    LineRenderer lineRend;
    Vector3[] segmentPoses;
    Vector3[] segmentV; //smooth damp velocities

    Transform targetDir; //pointer
    [SerializeField] Transform [] endPoses;
    [SerializeField] float distanceBetweenPoints; //the points keep a distance away from each other
    [SerializeField] float smoothSpeed; //how fast the point will come to a halt
    Vector3 frac;
    //float trailSpeed;

   /*  float wiggleSpeed; //how fast the wave moves
    float wiggleMagnitude; //pronouced waveline motion (snake movement)
    Transform wiggleDir; */

    //additional sprites
    /* Transform tailEnd;
    Transform[] bodyParts; */


    //start
    private void Start(){
        lineRend = GetComponent<LineRenderer>();
        lineRend.sortingLayerName = "Overlay";
        targetDir = transform.parent;
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
        for (int i = 0; i < endPoses.Length; i++){
            segmentPoses[length- 1 - i] = endPoses[i].position;
        }
        frac = new Vector3 (1/length, 1, 1);
        ResetPos();
    }

    //update
    public void Update(){
        //wiggleDir.locationRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);
        segmentPoses[0] = targetDir.position; //first position is pointer pos
        for (int i = 1; i < (anchored ? segmentPoses.Length - endPoses.Length: segmentPoses.Length); i++){
            //interpolate each point to the next point's position
            //segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i-1] + targetDir.right*distanceBetweenPoints, ref segmentV[i], smoothSpeed + i/trailSpeed);

            Vector3 targetPos = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * distanceBetweenPoints;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], smoothSpeed);

            //additional body parts
            //bodyParts[i - 1].transform.position = segmentPoses[i];
        }
        if (anchored){
            for (int i = 0; i < endPoses.Length; i++){
                Vector3 newV = new Vector3(targetDir.position.x, targetDir.position.y, endPoses[i].position.z);
                endPoses[i].position = newV;
                segmentPoses[length- 1 - i] = newV;
            }
        }
        lineRend.SetPositions(segmentPoses);

        //tailEnd.position = segmentPoses[segmentPoses.Length - 1];

    }

    private void ResetPos(){
        segmentPoses[0] = targetDir.position;
        for(int i = 1; i < length - endPoses.Length; i++){
            segmentPoses[i] = segmentPoses[i-1] + targetDir.right * distanceBetweenPoints;
        }
        lineRend.SetPositions(segmentPoses);
    }

}
