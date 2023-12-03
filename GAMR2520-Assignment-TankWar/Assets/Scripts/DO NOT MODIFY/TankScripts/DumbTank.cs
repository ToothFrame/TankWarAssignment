using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DumbTank : AITank
{
    //store ALL currently visible 
    public Dictionary<GameObject, float> enemyTanksFound = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> consumablesFound = new Dictionary<GameObject, float>();
    public Dictionary<GameObject, float> enemyBasesFound = new Dictionary<GameObject, float>();

    //store ONE from ALL currently visible
    public GameObject enemyTankPosition;
    public GameObject consumablePosition;
    public GameObject enemyBasePosition;

    //timer
    float t;

    /*******************************************************************************************************      
    WARNING, do not include void Start(), use AITankStart() instead if you want to use Start method from Monobehaviour.
    *******************************************************************************************************/
    public override void AITankStart()
    {
    }

    /*******************************************************************************************************       
    WARNING, do not include void Update(), use AITankUpdate() instead if you want to use Update method from Monobehaviour.
    *******************************************************************************************************/
    public override void AITankUpdate()
    {
        //Update all currently visible.
        enemyTanksFound = VisibleEnemyTanks;
        consumablesFound = VisibleConsumables;
        enemyBasesFound = VisibleEnemyBases;


        //if low health or ammo, go searching
        if (TankCurrentHealth < 50 || TankCurrentAmmo < 5)
        {
            //if there is more than 0 consumables visible
            if (consumablesFound.Count > 0)
            {
                //get the first consumable from the list.
                consumablePosition = consumablesFound.First().Key;
                FollowPathToWorldPoint(consumablePosition, 1f);
                t += Time.deltaTime;
                if (t > 10)
                {
                    GenerateRandomPoint();
                    t = 0;
                }
            }
            else
            {
                enemyTankPosition = null;
                consumablePosition = null;
                enemyBasePosition = null;
                FollowPathToRandomWorldPoint(1f);
            }
        }
        else
        {
            //if there is a enemy tank found
            if (enemyTanksFound.Count > 0 && enemyTanksFound.First().Key != null)
            {
                enemyTankPosition = enemyTanksFound.First().Key;
                if (enemyTankPosition != null)
                {
                    //get closer to target, and fire
                    if (Vector3.Distance(transform.position, enemyTankPosition.transform.position) < 25f)
                    {
                        TurretFireAtPoint(enemyTankPosition);
                    }
                    //else follow
                    else
                    {
                        FollowPathToWorldPoint(enemyTankPosition, 1f);
                    }
                }
            }
            else if (consumablesFound.Count > 0)
            {
                //if consumables are found, go to it.
                consumablePosition = consumablesFound.First().Key;
                FollowPathToWorldPoint(consumablePosition, 1f);
            }
            else if (enemyBasesFound.Count > 0)
            {
                //if base if found
                enemyBasePosition = enemyBasesFound.First().Key;
                if (enemyBasePosition != null)
                {
                    //go close to it and fire
                    if (Vector3.Distance(transform.position, enemyBasePosition.transform.position) < 25f)
                    {
                        TurretFireAtPoint(enemyBasePosition);
                    }
                    else
                    {
                        FollowPathToWorldPoint(enemyBasePosition, 1f);

                    }
                }
            }
            else
            {
                //searching
                enemyTankPosition = null;
                consumablePosition = null;
                enemyBasePosition = null;
                FollowPathToRandomWorldPoint(1f);
                t += Time.deltaTime;
                if (t > 10)
                {
                    GenerateNewRandomWorldPoint();
                    t = 0;
                }
            }
            
        }
    }

    /*******************************************************************************************************       
    WARNING, do not include void OnCollisionEnter(), use AIOnCollisionEnter() instead if you want to use Update method from Monobehaviour.
    *******************************************************************************************************/
    public override void AIOnCollisionEnter(Collision collision)
    {
    }



    /*******************************************************************************************************       
    Below are a set of functions you can use. These reference the functions in the AITank Abstract class
    and are protected. These are simply to make access easier if you an not familiar with inheritance and modifiers
    when dealing with reference to this class. This does mean you will have two similar function names, one in this
    class and one from the AIClass. 
    *******************************************************************************************************/


    /// <summary>
    /// Generate a path from current position to pointInWorld (GameObject)
    /// </summary>
    public void GeneratePathToWorldPoint(GameObject pointInWorld)
    {
        FindPathToPoint(pointInWorld);
    }

    /// <summary>
    ///Generate and Follow path to pointInWorld (GameObject) at normalizedSpeed (0-1)
    /// </summary>
    public void FollowPathToWorldPoint(GameObject pointInWorld, float normalizedSpeed)
    {
        FollowPathToPoint(pointInWorld, normalizedSpeed);
    }

    /// <summary>
    ///Generate and Follow path to a randome point at normalizedSpeed (0-1)
    /// </summary>
    public void FollowPathToRandomWorldPoint(float normalizedSpeed)
    {
        FollowPathToRandomPoint(normalizedSpeed);
    }

    /// <summary>
    ///Generate new random point
    /// </summary>
    public void GenerateNewRandomWorldPoint()
    {
        GenerateRandomPoint();
    }

    /// <summary>
    /// Stop Tank at current position.
    /// </summary>
    public void TankStop()
    {
        StopTank();
    }

    /// <summary>
    /// Continue Tank movement at last know speed and pointInWorld path.
    /// </summary>
    public void TankGo()
    {
        StartTank();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject)
    /// </summary>
    public void TurretFaceWorldPoint(GameObject pointInWorld)
    {
        FaceTurretToPoint(pointInWorld);
    }

    /// <summary>
    /// Reset turret to forward facing position
    /// </summary>
    public void TurretReset()
    {
        ResetTurret();
    }

    /// <summary>
    /// Face turret to pointInWorld (GameObject) and fire (has delay).
    /// </summary>
    public void TurretFireAtPoint(GameObject pointInWorld)
    {
        FireAtPoint(pointInWorld);
    }

    /// <summary>
    /// Returns true if the tank is currently in the process of firing.
    /// </summary>
    public bool TankIsFiring()
    {
        return IsFiring;
    }

    /// <summary>
    /// Returns float value of remaining health.
    /// </summary>
    public float TankCurrentHealth
    {
        get
        {
            return GetHealthLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining ammo.
    /// </summary>
    public float TankCurrentAmmo
    {
        get
        {
            return GetAmmoLevel;
        }
    }

    /// <summary>
    /// Returns float value of remaining fuel.
    /// </summary>
    public float TankCurrentFuel
    {
        get
        {
            return GetFuelLevel;
        }
    }

    /// <summary>
    /// Returns list of friendly bases.
    /// </summary>
    protected List<GameObject> MyBases
    {
        get
        {
            return GetMyBases;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject target, float distance) of visible targets (tanks in TankMain LayerMask).
    /// </summary>
    protected Dictionary<GameObject, float> VisibleEnemyTanks
    {
        get
        {
            return TanksFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject consumable, float distance) of visible consumables (consumables in Consumable LayerMask).
    /// </summary>
    protected Dictionary<GameObject, float> VisibleConsumables
    {
        get
        {
            return ConsumablesFound;
        }
    }

    /// <summary>
    /// Returns Dictionary(GameObject base, float distance) of visible enemy bases (bases in Base LayerMask).
    /// </summary>
    protected Dictionary<GameObject, float> VisibleEnemyBases
    {
        get
        {
            return BasesFound;
        }
    }

}
