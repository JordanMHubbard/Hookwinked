using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;

public class ProjectileHandler : MonoBehaviour
{
    [SerializeField] private float launchForce = 400f;
    [SerializeField] private Camera mainCamera;
    private List<GameObject> projectiles;
    private bool isUnlocked;
    public void SetIsUnlocked(bool shouldUnlock) { isUnlocked = shouldUnlock;}

    private void Awake()
    {
        projectiles = new List<GameObject>();
        
        if (GameManager.Instance != null && GameManager.Instance.GetIsPerkUnlocked(2)) 
        {
            isUnlocked = true;
            Debug.Log("Added ability to shoot!");
        }
    }
    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.PlayerInput.actions["Attack"].performed += Shoot;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.PlayerInput.actions["Attack"].performed -= Shoot;
        }
    }
    
    public void AddProjectile(GameObject projectile) 
    { 
        projectiles.Add(projectile);
        GameManager.Instance.SetRockCount(projectiles.Count); 
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        if (!isUnlocked) return; 

        if (projectiles.Count > 0)
        {
            Debug.Log("Shot!");
            GameObject lastProjectile = projectiles[projectiles.Count - 1];
            projectiles.RemoveAt(projectiles.Count - 1);

            Rigidbody rb = lastProjectile.GetComponent<Rigidbody>();

            lastProjectile.transform.position = mainCamera.transform.position;
            lastProjectile.SetActive(true);

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(mainCamera.transform.forward * launchForce);
            }

            GameManager.Instance.SetRockCount(projectiles.Count);
        }
    }

}
