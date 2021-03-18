using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMLib;
using XMLib.AM;
using XMLib.AM.Ranges;

public class ActionMachineController : MonoBehaviour
{
    [SerializeField]
    private string configName = null;

    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private Rigidbody _rigid = null;

    [SerializeField]
    private Transform modelRoot = null;

    [SerializeField]
    private float rotationSpeed = 1;

    [SerializeField]
    private LayerMask goundMask;

    [SerializeField]
    public bool _isGround = true;

    private IActionMachine actionMachine;
    private float animatorTimer;

    public Rigidbody rigid => _rigid;

    private Quaternion _modelRotation;
    public Quaternion modelRotation => _modelRotation;
    public bool isGround => _isGround && Mathf.Approximately(_rigid.velocity.y, 0);

    private void Start()
    {
        animatorTimer = 0;

        actionMachine = new ActionMachine();
        actionMachine.Initialize(configName, this);

        _modelRotation = modelRoot.rotation;

        InitAnimation();
    }

    private void Update()
    {
        UpdateAnimation();
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        Vector3 velocity = rigid.velocity;
        velocity.y = 0f;
        if (velocity.magnitude > 0.0001f)
        {
            _modelRotation = Quaternion.LookRotation(velocity);
        }

        modelRoot.rotation = Quaternion.Lerp(modelRoot.rotation, _modelRotation, Time.deltaTime * rotationSpeed);
    }

    public void LogicUpdate(float deltaTime)
    {
        //更新状态
        actionMachine.LogicUpdate(deltaTime);

        //更新动画
        UpdateLogicAnimation(deltaTime);

        CheckGround();
    }

    private void CheckGround()
    {
        float length = 0.02f;
        _isGround = rigid.velocity.y > 0 ? false : Physics.Raycast(transform.position + length * Vector3.up, Vector3.down, length * 2, goundMask);
    }

    private void InitAnimation()
    {
        if (animator == null)
        {
            return;
        }

        string animName = actionMachine.GetAnimName();
        animator.Play(animName, 0, 0);
        animator.Update(0);
    }

    private void UpdateAnimation()
    {
        if (animatorTimer <= 0)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        if (deltaTime < animatorTimer)
        {
            animatorTimer -= deltaTime;
        }
        else
        {
            deltaTime = animatorTimer;
            animatorTimer = 0f;
        }

        if (animator != null)
        {
            animator.Update(deltaTime);
        }
    }

    private void UpdateLogicAnimation(float deltaTime)
    {
        ActionMachineEvent eventTypes = actionMachine.eventTypes;

        if ((eventTypes & ActionMachineEvent.FrameChanged) != 0)
        {
            animatorTimer += deltaTime;
        }

        if ((eventTypes & ActionMachineEvent.StateChanged) != 0)
        {
            Debug.Log($"StateChanged：{actionMachine.stateName}");
        }

        if (animator != null && (eventTypes & ActionMachineEvent.AnimChanged) != 0)
        {
            StateConfig config = actionMachine.GetStateConfig();

            float fixedTimeOffset = actionMachine.animStartTime;
            float fadeTime = config.fadeTime;
            string animName = actionMachine.GetAnimName();

            if ((eventTypes & ActionMachineEvent.HoldAnimDuration) != 0)
            {
                fixedTimeOffset = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }

            animator.CrossFadeInFixedTime(animName, fadeTime, 0, fixedTimeOffset);
            animator.Update(0);
        }
    }

    private void OnDrawGizmos()
    {
        if (actionMachine == null)
        {
            return;
        }

        Matrix4x4 mat = Matrix4x4.TRS(transform.position, _modelRotation, Vector3.one);
        var attackRanges = actionMachine.GetAttackRanges();
        var bodyRanges = actionMachine.GetBodyRanges();
        DrawRanges(attackRanges, mat, Color.red);
        DrawRanges(bodyRanges, mat, Color.green);

        return;

        void DrawRanges(List<RangeConfig> ranges, Matrix4x4 matrix, Color color)
        {
            if (ranges == null || ranges.Count == 0)
            {
                return;
            }

            DrawUtility.G.PushColor(color);

            foreach (var range in ranges)
            {
                switch (range.value)
                {
                    case BoxItem v:
                        DrawUtility.G.DrawBox(v.size, matrix * Matrix4x4.TRS((Vector3)v.offset, Quaternion.identity, Vector3.one));
                        break;

                    case SphereItem v:
                        DrawUtility.G.DrawSphere(v.radius, matrix * Matrix4x4.TRS((Vector3)v.offset, Quaternion.identity, Vector3.one));
                        break;
                }
            }
            DrawUtility.G.PopColor();
        }
    }
}