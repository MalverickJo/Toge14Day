using UnityEngine;
using System.Collections.Generic;

public class PartyFollow : MonoBehaviour
{
    private Transform target;
    private float followSpeed = 5f;
    private float stopDistance = 0.5f;
    private Character character;
    private int myIndex = 1;

    private List<Vector3> positionHistory = new List<Vector3>();
    private float recordTimer = 0f;
    private float recordInterval = 0.05f;
    private int historyOffset = 10;

    private Vector3 currentVelocity = Vector3.zero;
    private float smoothTime = 0.15f;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void SetTarget(Transform t, int index = 1)
    {
        target = t;
        myIndex = index;
    }

    private void Update()
    {
        if (target == null) return;

        recordTimer += Time.deltaTime;
        if (recordTimer >= recordInterval)
        {
            recordTimer = 0f;
            positionHistory.Insert(0, target.position);
            if (positionHistory.Count > 150)
                positionHistory.RemoveAt(positionHistory.Count - 1);
        }

        int targetIndex = myIndex * historyOffset;
        if (positionHistory.Count <= targetIndex) return;

        Vector3 targetPos = positionHistory[targetIndex];
        targetPos.z = transform.position.z;

        float dist = Vector3.Distance(transform.position, targetPos);
        bool isMoving = dist > stopDistance;

        character?.SetMoveAnimation(isMoving);

        if (isMoving)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref currentVelocity,
                smoothTime
            );
            float dirX = targetPos.x - transform.position.x;
            if (Mathf.Abs(dirX) > 0.05f)
                character?.SetFacingDirection(dirX);
        }
        else
        {
            currentVelocity = Vector3.zero;
        }
    }
}