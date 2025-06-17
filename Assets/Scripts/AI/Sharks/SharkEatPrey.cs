using System.Collections;
using UnityEngine;

public class SharkEatPrey : AIEatPrey
{
    private SharkController sharkController;
    [SerializeField] private AudioClip[] attackSounds;
    protected override void Start()
    {
        base.Start();

        sharkController = controller as SharkController;
        if (!sharkController) { Debug.LogWarning("Sharkcontroller has not been set!"); }

    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Prey"))
        {
            HandleSharkEating();
        }
        
        if (other.CompareTag("Player"))
        {
            HandleSharkEating();
            preyDetect.SetIsPursuingPrey(false);
            FishEnergy energyComp = other.GetComponent<FishEnergy>();
            if (energyComp) energyComp.AddProgress(-30f);
            GameManager.Instance.ShowHurtEffect();
            //Debug.Log("Shark tryna eat player");
        }
    }

    private void HandleSharkEating()
    {
        if (sharkController)
        {
            SoundFXManager.Instance.PlayRandomSoundFXClip(attackSounds, transform, 1f, 1f, 0.2f, 0.1f);
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
