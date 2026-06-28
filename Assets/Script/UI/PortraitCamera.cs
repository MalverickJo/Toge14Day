using UnityEngine;
using UnityEngine.UI;

public class PortraitCamera : MonoBehaviour
{
    private Camera portraitCam;
    private RenderTexture renderTexture;
    private GameObject spawnedPrefab;

    private static float xOffset = -50f;
    private static int slotIndex = 0;
    private Vector3 spawnPosition;

    public RawImage targetImage;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetSlotIndex()
    {
        slotIndex = 0;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Setup(GameObject prefab, RawImage rawImage, bool faceRight = true)
    {
        targetImage = rawImage;

        int mySlot = slotIndex++;
        spawnPosition = new Vector3(xOffset + (mySlot * 5f), -50f, 0f);

        spawnedPrefab = Instantiate(prefab, spawnPosition, Quaternion.identity);

        SpriteRenderer[] renderers = spawnedPrefab.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in renderers)
        {
            Vector3 scale = sr.transform.localScale;
            scale.x = faceRight ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            sr.transform.localScale = scale;
        }

        foreach (var rb in spawnedPrefab.GetComponentsInChildren<Rigidbody2D>())
            rb.simulated = false;
        foreach (var col in spawnedPrefab.GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        var anim = spawnedPrefab.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetBool("1_Move", false);

        renderTexture = new RenderTexture(128, 128, 16);
        renderTexture.name = $"PortraitRT_{mySlot}_{Time.frameCount}";
        renderTexture.antiAliasing = 1;
        renderTexture.Create(); 

        GameObject camObj = new GameObject($"PortraitCam_{mySlot}");
        camObj.transform.position = spawnPosition + new Vector3(0f, 0.5f, -10f);
        camObj.transform.SetParent(transform);

        portraitCam = camObj.AddComponent<Camera>();
        portraitCam.orthographic = true;
        portraitCam.orthographicSize = 0.4f;
        portraitCam.targetTexture = renderTexture;
        portraitCam.clearFlags = CameraClearFlags.SolidColor;
        portraitCam.backgroundColor = new Color(0, 0, 0, 0);
        portraitCam.cullingMask = LayerMask.GetMask("Default");
        portraitCam.depth = -10 - mySlot; 

        targetImage.texture = renderTexture;
        targetImage.color = Color.white;
    }

    private void OnDestroy()
    {
        if (spawnedPrefab != null)
        {
            Destroy(spawnedPrefab);
            spawnedPrefab = null;
        }
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
            renderTexture = null;
        }
    }

}