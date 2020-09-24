using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib.AM;

public interface IActionController
{
    int ID { get; }
    Vector3 velocity { get; set; }
    Quaternion rotation { get; set; }
    float gravity { get; }

    bool CheckGround();
    bool CheckAir();
}

public class PlayerController : MonoBehaviour, IActionController
{
    public int id;
    public string configName;
    public Animator animator;
    public new Rigidbody rigidbody;
    public Transform modelRoot;

    public IActionMachine machine = new ActionMachine();


    #region  IActionController
    public int ID => id;
    public float gravity { get => Physics.gravity.y; }

    public Vector3 velocity { get => rigidbody.velocity; set => rigidbody.velocity = value; }

    public Quaternion rotation { get => transform.rotation; set => transform.rotation = value; }

    protected float _animatorTimer = 0f;
    public float softRotationSpeed = 10f;
    private Quaternion softRotationValue;

    private void OnEnable()
    {
        softRotationValue = transform.rotation;
    }

    public bool CheckAir()
    {
        return !CheckGround();
    }

    public bool CheckGround()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        UpdateRotation();
        UpdateAnimation();
    }

    private void UpdateRotation()
    {
        softRotationValue = Quaternion.Lerp(softRotationValue, rotation, Time.deltaTime * softRotationSpeed);
        modelRoot.rotation = softRotationValue;
    }

    #endregion

    public void Initialize(GameController game)
    {
        machine.Initialize(configName, this, game);
    }

    private void UpdateAnimation()
    {
        //更新动画
        if (_animatorTimer > 0)
        {
            _animatorTimer -= Time.deltaTime;
            if (_animatorTimer > 0)
            {
                if (animator != null)
                {
                    animator.Update(Time.deltaTime);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.Update(Time.deltaTime + _animatorTimer);
                }
                _animatorTimer = 0;
            }
        }
    }

    public void LogicUpdate(float deltaTime)
    {
        machine.LogicUpdate(deltaTime);

        var eventTypes = machine.eventTypes;

        if ((eventTypes & ActionMachineEvent.FrameChanged) != 0)
        {
            _animatorTimer += deltaTime;
        }

        if ((eventTypes & ActionMachineEvent.AnimChanged) != 0)
        {//动画切换
            StateConfig config = machine.GetStateConfig();

            float fixedTimeOffset = 0;
            float fadeTime = config.fadeTime;
            string animName = machine.GetAnimName();

            if ((eventTypes & ActionMachineEvent.HoldAnimDuration) != 0)
            {
                fixedTimeOffset = animator.GetCurrentAnimatorStateInfo(0).length;
            }

            animator.CrossFadeInFixedTime(animName, fadeTime, 0, fixedTimeOffset);
            animator.Update(0);
        }
    }
}
