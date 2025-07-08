using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissileScript : MonoBehaviour
{
    GameManager gameManager;
    EnemyScript enemyScript;
    public Vector3 targetTileLocation;
    private int targetTile = -1;

    private AudioClip shipHitSnd;
    private AudioClip waterHitSnd;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        enemyScript = GameObject.Find("Enemy").GetComponent<EnemyScript>();
        shipHitSnd = gameManager.audioClipArray[1];
        waterHitSnd = gameManager.audioClipArray[0];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ship"))
        {
            //if (gameManager.playerHasShotMissile)
            //    gameManager.playerHasShotMissile = false;

            //if (collision.gameObject.name == "Submarine") targetTileLocation.y += 0.3f;

            gameManager.EnemyHitPlayer(targetTileLocation, targetTile, collision.gameObject);
            gameManager.audioSource.PlayOneShot(shipHitSnd, 1.0f);

        }
        else
        {
            enemyScript.PauseAndEnd(targetTile);
            gameManager.audioSource.PlayOneShot(waterHitSnd, 0.7f);
        }
        if (gameManager.playerHasShotMissile)
                gameManager.playerHasShotMissile = false;
        
        Destroy(gameObject);
    }

    public void SetTarget(int target)
    {
        targetTile = target;
    }
}
