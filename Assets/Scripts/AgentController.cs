using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AgentController : MonoBehaviour {

    private NavMeshAgent agent;
    public GameObject[] CitizensAC;
    public GameObject[] houses;
    public int targetCitizen;
    private bool targetIsCitizen = false;
    public Transform targetLocation;
    public float targetDistAC;
    public GameObject waterWell;
    private bool waterWellReached;
    private bool houseReached;

    public Vector3 randomVector3;
    public int walkRadius;
    private NavMeshHit navHit;

    private bool isClicked; //not needed yet

    private void Start()
    {
        targetIsCitizen = false;
        isClicked = false;
        waterWellReached = false;
        houseReached = false;

        agent = this.GetComponent<NavMeshAgent>();
        UpdateCitizenArray();
        UpdateHouseArray();

        //Debug.Log(CitizensAC.Length);
       
        FindTarget();
    }

    public void UpdateCitizenArray()
    {
        CitizensAC = GameObject.FindGameObjectsWithTag("Citizen");
        this.GetComponent<infectionController>().CitizensIC = CitizensAC;
    }
    public void UpdateHouseArray()
    {
        houses = GameObject.FindGameObjectsWithTag("House");
    }

    void Update ()
    {
        //if target is a citizen needs dynamic target positioning to update the setdestination with the targets new location, hence the function being in update
        if(targetIsCitizen == true)
        {
            //print(targetCitizen);
            //print(CitizensAC.Length);
            targetLocation = CitizensAC[targetCitizen].transform;
            agent.SetDestination(targetLocation.position);
            //Debug.Log("moving target");

            //The script below became obselete when the script checked all citizens distance rather than just the targets distance
            //if the target is a citizen we need to be able to use the distance between the target and this.gameobject so when they are close enough, the target will be infected if this.gameobject is also infected
            //targetDistAC = Vector3.Distance(this.transform.position, targetLocation.position);
        }
	}

    /*
     for findTarget:
     select target from all citizen
     when in x range of target and they are infected, re-select target 
     if in x range of target and not infected, target is now infected, reselect target
     */
    void FindTarget()
    {
        StartCoroutine(FindTargetPoint());
        Debug.Log("target changing");
    }

    void StopInvoke()
    {
        CancelInvoke();
    }

    private IEnumerator FindTargetPoint()
    {
        //value below is the random number which determines what the agent does
        var randomNumber = Random.value;

        //to find a random point on the navigation mesh
        walkRadius = Random.Range(20, 150);
        randomVector3 = Random.insideUnitSphere * walkRadius;
        randomVector3 += transform.position;
        NavMesh.SamplePosition(randomVector3, out navHit, walkRadius, 1);

        if (targetCitizen <= CitizensAC.Length)
        {
            // if randomNumber is more than 0.4 this agents target is a random citizen in the world
            if (randomNumber >= 0.4f && CitizensAC.Length >= 0)
            {
                //Debug.Log("changing target");
                targetCitizen = Random.Range(1, CitizensAC.Length);
                targetIsCitizen = true;
                //Debug.Log(targetCitizen);
                yield return new WaitForSeconds(Random.Range(10f, 15f)); // waits between 5 to 10 seconds each time

            }
            else if (0.6f >= randomNumber && randomNumber >= 0.4f) // when the random.value is less than 0.4 the setdestination(target) is a random point on in the world
            {
                //when searhcing for a well the agent will go to the well, wait 3 seconds and walk to a random house, as if taking water from the well to the house
                agent.SetDestination(waterWell.transform.position);
                yield return new WaitUntil(() => waterWellReached == true);
                yield return new WaitForSeconds(3f);
                agent.SetDestination(houses[Random.Range(1, houses.Length)].transform.position);
                yield return new WaitUntil(() => houseReached == true);
                yield return new WaitForSeconds(3f);
            }
            else
            {
                //Debug.Log("finding random point on navmesh");
                agent.SetDestination(navHit.position);
                targetIsCitizen = false;
                yield return new WaitForSeconds(Random.Range(10f, 15f)); // waits between 5 to 10 seconds each time

            }
        }
        else
        {
            UpdateCitizenArray();
        }
        
        FindTarget();
    }
}
