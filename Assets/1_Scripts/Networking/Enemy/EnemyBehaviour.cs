using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
	[SerializeField] private float health = 100f;
	[Space]
	[SerializeField] PlayerController[] players;
	[SerializeField] PlayerController nearestPlayer;
	[SerializeField] private float speed;
	[Space]
	[SerializeField] private GameObject healthbarParent;
	[SerializeField] private Image healthBarImage;
	[Space]
	[SerializeField] private GameObject deathFX;
	[SerializeField] private AudioClip[] damageAudio;

	private PhotonView view;
	private ScoreUI score;

	private void Start()
	{
		view = GetComponent<PhotonView>();
		players = FindObjectsOfType<PlayerController>();
		score = FindObjectOfType<ScoreUI>();
		healthbarParent.SetActive( false );
	}

	private void Update()
	{
		if( nearestPlayer != null )
		{
			transform.position = Vector2.MoveTowards( transform.position, nearestPlayer.transform.position, speed * Time.deltaTime );
		}

		GetNearestPlayer();
	}

	private void GetNearestPlayer()
	{
		float distance = Mathf.Infinity;

		for( int i = 0; i < players.Length; i++ )
		{
			float distanceToPlayer = Vector2.Distance( transform.position, players[i].transform.position );
			if( distanceToPlayer < distance )
			{
				distance = distanceToPlayer;
				nearestPlayer = players[i];
			}
		}

	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		if( collision.CompareTag( "Player" ) )
		{
			TakeDamage();

			if( PhotonNetwork.IsMasterClient )
			{
				if( health <= 0 )
				{
					collision.GetComponent<PlayerController>().AddScore();
					score.UpdateScoreCounter();
					view.RPC( "SpawnParticle", RpcTarget.All );
					PhotonNetwork.Destroy( this.gameObject );
				}
			}
		}
	}

	private void TakeDamage()
	{
		health -= 25f;
		if( !healthbarParent.activeInHierarchy ) healthbarParent.SetActive( true );
		healthBarImage.fillAmount = health / 100f;
		AudioSource.PlayClipAtPoint( damageAudio[Random.Range( 0, damageAudio.Length )], transform.position );
	}

	[PunRPC]
	private void SpawnParticle()
	{
		Instantiate( deathFX, transform.position, Quaternion.identity );
	}
}
