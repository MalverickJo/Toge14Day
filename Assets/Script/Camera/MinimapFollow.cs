using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform target;
    public float zOffset = -50f;

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = new Vector3(target.position.x, target.position.y, zOffset);
    }
}
