using NaughtyAttributes;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	public int score = 0;
	public float health = 100f;

	[SerializeField] private float speed = 5f;
	[SerializeField] private bool canMove = true;
	[SerializeField] private bool canAttack = true;
	[Space]
	[SerializeField] private TextMeshPro usernameText;
	[SerializeField] private Image healthBarImage;
	[SerializeField] private TextMeshProUGUI healthBarText;
	[SerializeField] private float minX, maxX, minY, maxY;
	[Space]
	[SerializeField] private AudioClip[] movementAudio;

	[ReadOnly] [SerializeField] private Vector2 movementInput;
	[ReadOnly] [SerializeField] private Vector2 moveAmount;
	[ReadOnly] [SerializeField] private float attackInput;

	private Animator anim;
	private PhotonView view;
	private BoxCollider2D boxCollider2D;
	private GameManager gameManager;

	public TextMeshPro UsernameText { get => usernameText; set => usernameText = value; }

	private void Start()
	{
		anim = GetComponent<Animator>();
		view = GetComponent<PhotonView>();
		boxCollider2D = GetComponent<BoxCollider2D>();
		gameManager = FindObjectOfType<GameManager>();

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
			if( collision.CompareTag( "Enemy" ) )
			{
				TakeDamage( 10 );
			}
		}
	}

	private void AnimateSprite()
	{
		if( canMove && movementInput != Vector2.zero )
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

	private void SetUsername()
	{
		if( view.IsMine )
		{
			usernameText.text = PhotonNetwork.NickName;
		}
		else
		{
			usernameText.text = view.Owner.NickName;
		}
	}

	private void Wrap()
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

	private void PlayMovementAudio()
	{
		AudioSource.PlayClipAtPoint( movementAudio[Random.Range( 0, movementAudio.Length )], transform.position, 0.2f );
	}

	public void AddScore()
	{
		view.RPC( "AddScoreRPC", RpcTarget.All );
	}

	[PunRPC]
	private void AddScoreRPC()
	{
		score++;
	}

	public void TakeDamage( int damage )
	{
		view.RPC( "TakeDamageRPC", RpcTarget.All, damage );
	}

	[PunRPC]
	private void TakeDamageRPC( int damage )
	{
		if( health > 0 )
		{
			health -= damage;
			healthBarImage.fillAmount = health / 100f;
			healthBarText.text = health.ToString();
		}

		if( health <= 0 )
		{
			anim.SetBool( "Dead", true );

			canMove = false;
			canAttack = false;

			healthBarText.text = "DEAD";

			boxCollider2D.enabled = false;

			gameManager.GameOver();
		}
	}
}
