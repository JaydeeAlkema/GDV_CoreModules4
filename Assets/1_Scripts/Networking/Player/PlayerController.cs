using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float health = 10f;
	[SerializeField] private float speed = 5f;
	[SerializeField] private bool canMove = true;
	[SerializeField] private bool canAttack = true;
	[Space]
	[SerializeField] private TextMeshPro usernameText;
	[SerializeField] private Image healthBar;
	[SerializeField] private float minX, maxX, minY, maxY;

	[ReadOnly] [SerializeField] private Vector2 movementInput;
	[ReadOnly] [SerializeField] private Vector2 moveAmount;
	[ReadOnly] [SerializeField] private float attackInput;

	private Animator anim;
	private PhotonView view;

	private void Start()
	{
		anim = GetComponent<Animator>();
		view = GetComponent<PhotonView>();

		SetUsername();
	}

	private void Update()
	{
		if( view.IsMine )
		{
			movementInput = new Vector2( Input.GetAxisRaw( "Horizontal" ), Input.GetAxisRaw( "Vertical" ) );
			attackInput = Input.GetAxisRaw( "Fire1" );
			moveAmount = speed * Time.deltaTime * movementInput.normalized;

			Wrap();

			if( canMove && movementInput != Vector2.zero )
			{
				transform.position += ( Vector3 )moveAmount;
			}

			if( canAttack && attackInput == 1 )
			{
				Attack();
			}


			if( Input.GetKeyDown( KeyCode.H ) )
			{
				view.RPC( "TakeDamageRPC", RpcTarget.All );
			}

			AnimateSprite();
		}
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		if( view.IsMine )
		{
			TakeDamage();
		}
	}

	private void AnimateSprite()
	{
		if( canMove )
		{
			anim.SetFloat( "Horizontal", movementInput.x );
			anim.SetFloat( "Vertical", movementInput.y );
		}
	}

	private void Attack()
	{
		view.RPC( "AttackRPC", RpcTarget.All );
	}

	[PunRPC]
	private IEnumerator AttackRPC()
	{
		anim.SetTrigger( "Attack" );

		canMove = false;
		canAttack = false;
		yield return new WaitForSeconds( 0.3f );
		canMove = true;
		yield return new WaitForSeconds( 0.2f );
		canAttack = true;
	}

	public void TakeDamage()
	{
		view.RPC( "TakeDamageRPC", RpcTarget.All );
	}

	[PunRPC]
	public void TakeDamageRPC()
	{
		health -= 10f;
		healthBar.fillAmount = health / 100f;
	}

	public void SetUsername()
	{
		view.RPC( "SetUsernameRPC", RpcTarget.All );
	}

	[PunRPC]
	private void SetUsernameRPC()
	{
		usernameText.text = PhotonNetwork.NickName;
	}

	void Wrap()
	{
		if( transform.position.x < minX )
		{
			transform.position = new Vector2( maxX, transform.position.y );
		}

		if( transform.position.x > maxX )
		{
			transform.position = new Vector2( minX, transform.position.y );
		}

		if( transform.position.y < minY )
		{
			transform.position = new Vector2( transform.position.x, maxY );
		}

		if( transform.position.y > maxY )
		{
			transform.position = new Vector2( transform.position.x, minY );
		}
	}
}
