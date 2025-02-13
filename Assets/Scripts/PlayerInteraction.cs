using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float InteractionDistance = 10f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInteraction();
    }

    void CheckForInteraction()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, InteractionDistance, interactableLayer))
        {
            Debug.Log("Hit something!");
        }
    }
}
