using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    [SerializeField] private Animator anim;

    float maxComboDelay = 0.8f;
    public static int noOfClicks = 0;
    float lastClickedTime = 0f;

    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime > 0.25f && stateInfo.IsName("LightAttack1"))
            anim.SetBool("LightAttack1", false);

        if (stateInfo.normalizedTime > 0.25f && stateInfo.IsName("LightAttack2"))
            anim.SetBool("LightAttack2", false);

        if (stateInfo.normalizedTime > 0.25f && stateInfo.IsName("LightAttack3"))
            anim.SetBool("LightAttack3", false);

        if (stateInfo.normalizedTime > 0.25f && stateInfo.IsName("LightAttack4"))
        {
            anim.SetBool("LightAttack4", false);
            noOfClicks = 0;
        }

        if (Time.time - lastClickedTime > maxComboDelay)
            noOfClicks = 0;

        if (Input.GetMouseButtonDown(0))
            OnClick();
    }

    void OnClick()
    {
        lastClickedTime = Time.time;
        noOfClicks = Mathf.Clamp(noOfClicks + 1, 0, 4);

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        if (noOfClicks == 1)
            anim.SetBool("LightAttack1", true);

        else if (noOfClicks == 2 && stateInfo.normalizedTime > 0.25f && stateInfo.IsName("LightAttack1"))
        {
            anim.SetBool("LightAttack1", false);
            anim.SetBool("LightAttack2", true);
        }
        else if (noOfClicks == 3 && stateInfo.normalizedTime > 0.25f && stateInfo.IsName("LightAttack2"))
        {
            anim.SetBool("LightAttack2", false);
            anim.SetBool("LightAttack3", true);
        }
        else if (noOfClicks == 4 && stateInfo.normalizedTime > 0.25f && stateInfo.IsName("LightAttack3"))
        {
            anim.SetBool("LightAttack3", false);
            anim.SetBool("LightAttack4", true);
        }
    }
}
