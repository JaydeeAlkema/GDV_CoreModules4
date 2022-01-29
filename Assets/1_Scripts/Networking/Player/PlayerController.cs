using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Photon.Pun;

public enum MovementState
{
	idle,
	up,
	down,
	left,
	right
};

public class PlayerController : MonoBehaviour
{
	[SerializeField] private MovementState movementState = MovementState.idle;
	[SerializeField] private float speed = 5f;

	[ReadOnly] [SerializeField] private Vector2 movementInput = Vector2.zero;
	[ReadOnly] [SerializeField] private Rigidbody2D rb2d;
	[ReadOnly] [SerializeField] private Animator anim;
	[ReadOnly] [SerializeField] private PhotonView view;

	private void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		view = GetComponent<PhotonView>();
	}

	private void Update()
	{
		if( view.IsMine )
		{
			movementInput = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );

			UpdateMoveDirection();
			AnimateSprite();
		}
	}

	private void FixedUpdate()
	{
		Move();
	}

	private void Move()
	{
		rb2d.velocity = new Vector2( movementInput.normalized.x * speed, movementInput.normalized.y * speed );
	}

	private void UpdateMoveDirection()
	{
		if( movementInput.x == 1 ) { movementState = MovementState.right; }
		if( movementInput.x == -1 ) { movementState = MovementState.left; }
		if( movementInput.y == 1 ) { movementState = MovementState.up; }
		if( movementInput.y == -1 ) { movementState = MovementState.down; }
	}

	private void AnimateSprite()
	{
		anim.SetFloat( "Horizontal", movementInput.x );
		anim.SetFloat( "Vertical", movementInput.y );
	}
}
