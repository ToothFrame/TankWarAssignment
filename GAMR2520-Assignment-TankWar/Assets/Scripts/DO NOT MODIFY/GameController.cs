using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    List<AITank> aiTanks = new List<AITank>();

    private float gameStartTime = 3f;

    [HideInInspector]
    public bool gameStarted = false;


    private AStar aStar;

    private GameObject healthGameObject;
    private GameObject ammoGameObject;
    private GameObject fuelGameObject;

    List<GameObject> consumable = new List<GameObject>();

    public UIControllerScript uiControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        healthGameObject = transform.Find("Health").gameObject;
        ammoGameObject = transform.Find("Ammo").gameObject;
        fuelGameObject = transform.Find("Fuel").gameObject;
        aStar = GameObject.Find("AStarPlane").GetComponent<AStar>();

        uiControllerScript.gameObject.SetActive(true);
        uiControllerScript.winsText.text = "";

        consumable.Add(healthGameObject);
        consumable.Add(ammoGameObject);
        consumable.Add(ammoGameObject);
        consumable.Add(fuelGameObject);
        consumable.Add(fuelGameObject);
        consumable.Add(fuelGameObject);

        foreach (GameObject cons in consumable)
        {
            cons.SetActive(false);
        }

        StartCoroutine(GameStart(gameStartTime));
        StartCoroutine("GenerateRandomConsumable");

        GameObject[] aiTanksTemp = GameObject.FindGameObjectsWithTag("Tank");
        for (int i = 0; i < aiTanksTemp.Length; i++)
        {
            aiTanks.Add(aiTanksTemp[i].GetComponent<AITank>());
        }

        if(aiTanks.Count > 2)
        {
            uiControllerScript.DisableStatsUi();
        }

    }
    IEnumerator GameStart(float gameStartTime)
    {
        yield return new WaitForSeconds(gameStartTime);
        gameStarted = true;
    }

    IEnumerator GenerateRandomConsumable()
    {
        foreach (GameObject cons in consumable)
        {
            cons.SetActive(false);
        }

        yield return new WaitForSeconds(Random.Range(1f, 5f));
        Node randomNode = aStar.NodePositionInGrid(new Vector3(Random.Range(-92, 92), 0, Random.Range(-92, 92)));
        Vector3 consPos = Vector3.zero;


        while (!randomNode.traversable)
        {
            randomNode = aStar.NodePositionInGrid(new Vector3(Random.Range(-95, 95), 0, Random.Range(-95, 95)));

            yield return new WaitForEndOfFrame();
        }


        consPos = randomNode.nodePos;

        int randCons = Random.Range(0, consumable.Count);

        consumable[randCons].transform.position = new Vector3(consPos.x, 5, consPos.z) ;

        yield return new WaitForSeconds(1f);

        consumable[randCons].SetActive(true);

        yield return new WaitForSeconds(Random.Range(12f, 25f));

        StartCoroutine("GenerateRandomConsumable");
    }

    public void ConsumableCollection()
    {
        StopCoroutine("GenerateRandomConsumable");
        StartCoroutine("GenerateRandomConsumable");
    }

    private void Update()
    {
        List<AITank> aiTanksTemp = aiTanks;

        for (int i = 0; i < aiTanksTemp.Count; i++)
        {
            if(aiTanksTemp[i] == null)
            {
                aiTanksTemp.RemoveAt(i);
            }
            if(aiTanksTemp.Count == 1)
            {
                break;
            }
        }

        if(aiTanksTemp.Count == 1)
        {
            print(aiTanksTemp[0].transform.parent.name + " Wins!");
            gameStarted = false;
            uiControllerScript.winsText.text = aiTanksTemp[0].transform.parent.name + " Wins!";
        }
    }

    public void SetUIStats(float health, float fuel, float ammo, AITank tank)
    {
        if(aiTanks.Count == 2)
        {
            if(aiTanks[0] == tank)
            {
                uiControllerScript.tankOneName.text = tank.transform.parent.name;
                uiControllerScript.tankOneHealth.text = "Health: " + health.ToString();
                uiControllerScript.tankOneFuel.text = "Fuel: " + Mathf.Clamp(fuel, 0, fuel).ToString("00.0");
                uiControllerScript.tankOneAmmo.text = "Ammo: " + ammo.ToString();

            }

            if (aiTanks[1] == tank)
            {
                uiControllerScript.tankTwoName.text = tank.transform.parent.name;
                uiControllerScript.tankTwoHealth.text = "Health: " + health.ToString();
                uiControllerScript.tankTwoFuel.text = "Fuel: " + Mathf.Clamp(fuel, 0, fuel).ToString("00.0");
                uiControllerScript.tankTwoAmmo.text = "Ammo: " + ammo.ToString();
            }
        }
    }

}
