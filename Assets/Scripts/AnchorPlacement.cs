using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.SceneManagement;

public class AnchorSystem : MonoBehaviour
{
    [Header("Anchor Settings")]
    public GameObject anchorPrefab;
    public GameObject arrow3DPrefab;
    public float arrowHeight = 0.5f;

    [Header("Model Settings")]
    public GameObject[] modelPrefabs;
    public float modelSpawnDistance = 1.5f;

    [Header("UI Settings")]
    public Canvas floatingMenu;
    public Button[] placeModelButtons;

    [Header("XR References")]
    public XRRayInteractor rayInteractor;  

    private Camera xrCamera;
    private GameObject currentModel;

    private void Start()
    {
        xrCamera = Camera.main;

        if (rayInteractor == null) {
            rayInteractor = GetComponent<XRRayInteractor>();
            if (rayInteractor == null) {
                Debug.LogWarning("XRRayInteractor not assigned and not found on this GameObject.");
            }
        }

            placeModelButtons[0].onClick.AddListener(PlaceModelInFront);
            placeModelButtons[1].onClick.AddListener(ActiveScene);

   

        SetupFloatingMenu();
    }

    private void SetupFloatingMenu()
    {
        if (floatingMenu != null && xrCamera != null) {
            floatingMenu.worldCamera = xrCamera;
            UpdateMenuPosition();
        }
    }

    private void Update()
    {
        UpdateMenuPosition();

        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame) {
            PlaceAnchorAtRaycast();
        }
    }

    private void UpdateMenuPosition()
    {
        if (floatingMenu != null && xrCamera != null) {
            floatingMenu.transform.position = xrCamera.transform.position + xrCamera.transform.forward * 1.5f;
            floatingMenu.transform.LookAt(xrCamera.transform);
            floatingMenu.transform.Rotate(0, 180, 0);
        }
    }

    private void PlaceAnchorAtRaycast()
    {
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit)) {
            Debug.Log("Arrow spawned");
            GameObject anchor = Instantiate(anchorPrefab, hit.point, Quaternion.identity);

            GameObject arrow = Instantiate(
                arrow3DPrefab,
                hit.point + Vector3.up * arrowHeight,
                Quaternion.Euler(0f, 0f, 90f));

            arrow.transform.SetParent(anchor.transform);
        }
        else {
            Debug.LogWarning("RayInteractor is null or did not hit a valid surface.");
        }
    }

    private void PlaceModelInFront()
    {
        if (modelPrefabs == null || modelPrefabs.Length == 0) {
            Debug.LogWarning("No model prefabs assigned.");
            return;
        }

        if (xrCamera == null) {
            Debug.LogWarning("Main XR Camera is null.");
            return;
        }

        if (currentModel != null) Destroy(currentModel);

        currentModel = Instantiate(
            modelPrefabs[Random.Range(0, modelPrefabs.Length)],
            xrCamera.transform.position + xrCamera.transform.forward * modelSpawnDistance,
            Quaternion.identity);

        SetupModelInteractions(currentModel);
    }

    private void SetupModelInteractions(GameObject model)
    {
        var grab = model.GetComponent<XRGrabInteractable>();
        if (grab != null)
            grab.movementType = XRBaseInteractable.MovementType.Instantaneous;
    }

    public void ActiveScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
}
