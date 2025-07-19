using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    GameManager gameManager;
    Ray ray;
    RaycastHit hit;

    private bool missileHit = false;
    Color32[] hitColor = new Color32[2];

    private AudioClip waterHit;

    [Header("Objects")]
    public GameObject carrierShipPrefab;
    public GameObject battleShipPrefab;
    public GameObject submarinePrefab;
    public GameObject cruiserPrefab;
    public GameObject destroyerPrefab;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hitColor[0] = gameObject.GetComponent<MeshRenderer>().material.color;
        hitColor[1] = gameObject.GetComponent<MeshRenderer>().material.color;
        waterHit = gameManager.audioClipArray[0];
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            if(Input.GetMouseButtonDown(0) && hit.collider.gameObject.name == gameObject.name)
            {
                if (missileHit == false)
                {
                    if (!gameManager.matchPlaying)
                        gameManager.TileClicked(hit.collider.gameObject);
                    else
                    {
                        if (!gameManager.playerHasShotMissile)
                        {
                            gameManager.TileClicked(hit.collider.gameObject);
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Missile"))
        {
            missileHit = true;
        }
        else if (collision.gameObject.CompareTag("EnemyMissile"))
        {
            hitColor[0] = new Color32(38, 57, 76, 255);
            GetComponent<Renderer>().material.color = hitColor[0];
        }
    }

    public void SetTileColor(int index, Color32 color)
    {
        hitColor[index] = color;
    }

    public void SwitchColors(int colorIndex)
    {
        GetComponent<Renderer>().material.color = hitColor[colorIndex];
    }

    public void SetEnemyLoc(int tilePos, int shipLength, bool isHorizontal  /*Color32 color*/ )
    {
        Vector3 shipPos;
        switch (shipLength)
        {
            case 6:
                shipPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.7f, gameObject.transform.position.z);
                if (!isHorizontal)
                {
                    cruiserPrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 180f);
                    shipPos.z -= 2.25f;
                }
                else
                {
                    cruiserPrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 270f);
                    shipPos.x -= 2.25f;
                }
                Instantiate(cruiserPrefab, shipPos, cruiserPrefab.transform.rotation);
                break;
            case 3:
                shipPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.7f, gameObject.transform.position.z);
                if (!isHorizontal)
                {
                    submarinePrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 0);
                    shipPos.z -= 2.85f;
                }
                else
                {
                    submarinePrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 90f);
                    shipPos.x -= 2.85f;
                }
                Instantiate(submarinePrefab, shipPos, submarinePrefab.transform.rotation);
                break;
            case 5:
                shipPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.7f, gameObject.transform.position.z);
                if (!isHorizontal)
                {
                    carrierShipPrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 0);
                    shipPos.z -= 4.35f;
                }
                else
                {
                    carrierShipPrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 90f);
                    shipPos.x -= 4.35f;
                }
                Instantiate(carrierShipPrefab, shipPos, carrierShipPrefab.transform.rotation);
                break;
            case 4:
                shipPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.7f, gameObject.transform.position.z);
                if (!isHorizontal)
                {
                    battleShipPrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 0);
                    shipPos.z -= 3.35f;
                }
                else
                {
                    battleShipPrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 90f);
                    shipPos.x -= 3.35f;
                }
                Instantiate(battleShipPrefab, shipPos, battleShipPrefab.transform.rotation);
                break;
            case 2:
                shipPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z);
                if (!isHorizontal)
                {
                    destroyerPrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 180f);
                    shipPos.z -= 1.35f;
                }
                else
                {
                    destroyerPrefab.transform.rotation = Quaternion.Euler(-90f, 180f, 270f);
                    shipPos.x -= 1.35f;
                }
                Instantiate(destroyerPrefab, shipPos, destroyerPrefab.transform.rotation);
                break;
            default:
                break;
        }
    }
}
