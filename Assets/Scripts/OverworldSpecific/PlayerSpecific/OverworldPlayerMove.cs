using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeriUtils.Core;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))]
public class OverworldPlayerMove : MonoBehaviour
{
    public PrimaryControls controls;
    [SerializeField] [Range(0.1f, 25.0f)] float _movementSpeed = 5.0f;
    [SerializeField] LayerMask _validLayers;
    [SerializeField] CircleCollider2D _cirCol;
    Vector2 _internalPosition;
    Vector2 _internalVelocity;
    Collider2D[] _colliderBuffer = new Collider2D[3];
    Animator _animator;


    // Start is called before the first frame update
    void OnEnable()
    {
        controls = new PrimaryControls();
        controls.Enable();
        _cirCol = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
        _animator.Play("standSW");
    }

    void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        GetState();

        //
        _internalPosition += _internalVelocity;
        //
        CheckCollisions();
        SetState();
    }
    void GetState()
    {
        _internalPosition = transform.position;
        _internalVelocity = controls.Overworld.Move.ReadValue<Vector2>() * _movementSpeed * Time.deltaTime;
        _internalVelocity.y *= 0.5f;
        AnimationUpdate();
    }
    void CheckCollisions()
    {

        if (Physics2D.OverlapCircleNonAlloc(transform.position, _cirCol.radius, _colliderBuffer, _validLayers) > 0)
        {
            OverworldManager om = (OverworldManager)CentralManager.GetStateManager();
            for (int i = 0; i < _colliderBuffer.Length; i++)
            {
                if (_colliderBuffer[i])
                {
                    om.currentEnemyEncounter = _colliderBuffer[i].GetComponent<CharacterGameOverworldEntity>();
                }
            }
            om.BattleStart();
        }
    }
    void SetState()
    {
        transform.position = _internalPosition;
    }

    void AnimationUpdate()
    {
        float sin = Mathf.Sin((Vector2.SignedAngle(Vector2.right, controls.Overworld.Move.ReadValue<Vector2>())) * Mathf.PI / 180f);
        float cos = Mathf.Cos((Vector2.SignedAngle(Vector2.right, controls.Overworld.Move.ReadValue<Vector2>())) * Mathf.PI / 180f);

        if (sin > -0.5f && sin <= 0.5f && cos >= 0 && _internalVelocity.magnitude != 0f)
        {
            _animator.Play("standE");
        }
        else if (sin > 0.5f && sin <= 0.86f && cos >= 0)
        { _animator.Play("standNE"); }
        else if (sin > 0.86f && sin <= 1)
        { _animator.Play("standN"); }
        else if (sin > 0.5f && sin <= 0.86f && cos <= 0)
        { _animator.Play("standNW"); }
        if (sin > -0.5f && sin <= 0.5f && cos <= 0)
        { _animator.Play("standW"); }
        else if (sin <= -0.5f && sin > -0.86f && cos <= 0)
        { _animator.Play("standSW"); }
        else if (sin <= -0.86f && sin <= -1f)
        { _animator.Play("standS"); }
        else if (sin <= -0.5f && sin > -0.86f && cos >= 0)
        { _animator.Play("standSE"); }


        if (sin != 0)
        {
            if (!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioManager>().PlayTrack(AUDIOCLIPS.WALK);
        }
        else
        {
            GetComponent<AudioSource>().Stop();
        }
    }
}
