using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Ships")]
    public GameObject[] ships;
    public EnemyScript enemyScript;
    private ShipScript shipScript;
    private List<int[]> enemyShips;
    private List<int[]> enemyShipsCpy;
    private int shipIndex = 0;
    public List<TileScript> allTileScripts;

    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public Button menuBtn;
    public Text topText;
    public Text playerShipText;
    public Text enemyShipText;

    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;

    private bool setupComplete = false;
    private bool playerTurn = true;
    private bool enemyShipsShown = false;
    public bool matchPlaying = false;
    public bool playerHasShotMissile = false;

    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires = new List<GameObject>();

    private int enemyShipCount = 5;
    private int playerShipCount = 5;

    public AudioSource audioSource;
    public AudioClip[] audioClipArray;
    private AudioClip missileShotSnd;
    private AudioClip shipHitSnd;
    private AudioClip waterHitSnd;


    // Start is called before the first frame update
    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        replayBtn.onClick.AddListener(() => ReplayClicked());
        menuBtn.onClick.AddListener(() => MenuClicked());
        enemyShips = enemyScript.PlaceEnemyShips();
        enemyShipsCpy = new List<int[]>(enemyShips.Count);
        foreach (int[] s in enemyShips)
        {
            int[] elem = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                elem[i] = s[i];
            }
            enemyShipsCpy.Add(elem);
        }

        audioSource = GetComponent<AudioSource>();
        waterHitSnd = audioClipArray[0];
        shipHitSnd = audioClipArray[1];
        missileShotSnd = audioClipArray[2];
    }

    private void NextShipClicked()
    {
        if (!shipScript.OnGameBoard())
        {
            shipScript.FlashColor(Color.red);
        }
        else
        {
            if (shipIndex <= ships.Length - 2)
            {
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                woodDock.SetActive(false);
                audioSource.Play();
                topText.text = "Guess an enemy tile.";
                setupComplete = true;
                for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
                matchPlaying = true;
            }
        }
    }

    public void TileClicked(GameObject tile)
    {
        if (setupComplete && playerTurn && matchPlaying)
        {
            Vector3 tilePos = tile.transform.position;
            tilePos.y += 15;
            playerTurn = false;
            audioSource.PlayOneShot(missileShotSnd, 1.0f);
            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
            playerHasShotMissile = true;
        }
        else if (!setupComplete)
        {
            PlaceShip(tile);
            shipScript.SetClickedTile(tile);
        }
    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);
        ships[shipIndex].transform.localPosition = newVec;
    }

    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    public void CheckHit(GameObject tile)
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach (int[] tileNumArray in enemyShips)
        {
            if (tileNumArray.Contains(tileNum))
            {
                for (int i = 0; i < tileNumArray.Length; i++)
                {
                    if (tileNumArray[i] == tileNum)
                    {
                        tileNumArray[i] = -5;
                        hitCount++;
                    }
                    else if (tileNumArray[i] == -5)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == tileNumArray.Length)
                {
                    enemyShipCount--;
                    topText.text = "SUNK!!!!!!";
                    Vector3 firePos = new Vector3(tile.transform.position.x, tile.transform.position.y + 0.91f, tile.transform.position.z);
                    enemyFires.Add(Instantiate(firePrefab, firePos, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                    audioSource.PlayOneShot(shipHitSnd, 1.0f);
                }
                else
                {
                    topText.text = "HIT!!";
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255));
                    tile.GetComponent<TileScript>().SwitchColors(1);
                    audioSource.PlayOneShot(shipHitSnd, 1.0f);
                }
                break;
            }

        }
        if (hitCount == 0)
        {
            tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255));
            tile.GetComponent<TileScript>().SwitchColors(1);
            audioSource.PlayOneShot(waterHitSnd, 0.7f);
            topText.text = "Missed.";
        }
        Invoke("EndPlayerTurn", 1.0f);
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {
        enemyScript.MissileHit(tileNum);
        tile.y += 1.5f;
        playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
        if (hitObj.GetComponent<ShipScript>().HitCheckSank())
        {
            playerShipCount--;
            playerShipText.text = playerShipCount.ToString();
            enemyScript.SunkPlayer();
        }
        Invoke("EndEnemyTurn", 2.0f);
    }

    private void EndPlayerTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
        foreach (GameObject fire in playerFires) fire.SetActive(true);
        foreach (GameObject fire in enemyFires) fire.SetActive(false);
        enemyShipText.text = enemyShipCount.ToString();
        topText.text = "Enemy's turn";
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        if (playerShipCount < 1 && matchPlaying) GameOver("ENEMY WINs!!!");
    }

    public void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
        foreach (GameObject fire in playerFires) fire.SetActive(false);
        foreach (GameObject fire in enemyFires) fire.SetActive(true);
        playerShipText.text = playerShipCount.ToString();
        if (matchPlaying)
            topText.text = "Select a tile";
        playerTurn = true;
        ColorAllTiles(1);
        if (enemyShipCount < 1 && matchPlaying) GameOver("YOU WIN!!");
        if (!matchPlaying && !enemyShipsShown)
        {
            ShowEnemyShips();
            enemyShipsShown = true;
        }
    }

    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScript tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    void GameOver(string winner)
    {
        topText.text = "Game Over: " + winner;
        matchPlaying = false;
        replayBtn.gameObject.SetActive(true);
        menuBtn.gameObject.SetActive(true);
        playerTurn = false;
    }

    private void ShowEnemyShips()
    {
        bool isHorizontal;
        bool submarinePlaced = false;
        int shipLen;
        foreach (var ship in enemyShipsCpy)
        {
            isHorizontal = true;
            //Debug.Log("-- ENEMY SHIP POS:");
            //Debug.Log("LENGTH = " + ship.Length.ToString());
            for (int i = 0; i < ship.Length; i++)
            {
                int loc = ship[i];
                //Debug.Log("location[" + i + "] = " + loc);
            }
            if (ship[0] - ship[1] > 1)
                isHorizontal = false;
            if (ship.Length == 3 && submarinePlaced)
                shipLen = 6;
            else
                shipLen = ship.Length;
            allTileScripts.ElementAt(ship[0]-1).SetEnemyLoc(ship[0], shipLen, isHorizontal);
            if (ship.Length == 3 && !submarinePlaced)
            {
                submarinePlaced = true;
            }
        }
    }

    void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void MenuClicked()
    {
        SceneManager.LoadSceneAsync(0);
    }

}
