using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AIEatPrey : MonoBehaviour
{
    private AIFishController controller;
    private PreyDetection preyDetect;
    [SerializeField] private AudioClip[] eatSounds;

    private void Start()
    {
        controller = GetComponentInParent<AIFishController>();
        preyDetect = controller.GetComponentInChildren<PreyDetection>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prey"))
        {
            HandleSharkEating();

            SoundFXManager.Instance.PlayRandomSoundFXClip(eatSounds, transform, 1f, 1f, 0.2f, 0.1f);
            preyDetect.SetIsPursuingPrey(false);
            preyDetect.RemovePrey(other.transform.parent.gameObject);
            GameManager.Instance.PreyConsumed(other.transform.parent.gameObject);
        }
    }

    private void HandleSharkEating()
    {
        SharkController sharkController = controller as SharkController;
        if (sharkController)
        {
            sharkController.animator.Play("Attack");
            StartCoroutine(ReturnSharkToSwim(sharkController));
        }
    }

    private IEnumerator ReturnSharkToSwim(SharkController controller)
    {
        yield return new WaitForSeconds(controller.animator.GetCurrentAnimatorStateInfo(0).length);
        controller.animator.Play("Swim");
    }
}
