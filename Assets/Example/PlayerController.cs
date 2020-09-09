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

    public IActionMachine actionMachine = new ActionMachine();


    #region  IActionController
    public int ID => id;
    public float gravity { get=> Physics.gravity.y; }

    public Vector3 velocity { get => rigidbody.velocity; set => rigidbody.velocity = value; }
    public Quaternion rotation { get => modelRoot.rotation; set => modelRoot.rotation = value; }

    public bool CheckAir()
    {
        return !CheckGround();
    }

    public bool CheckGround()
    {
        throw new System.NotImplementedException();
    }

    #endregion

    public void Initialize(GameController game)
    {
        actionMachine.Initialize(configName, this, game);
    }

    public void LogicUpdate(float deltaTime)
    {
        actionMachine.LogicUpdate(deltaTime);
    }
}
