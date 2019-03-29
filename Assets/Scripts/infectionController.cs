using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class infectionController : MonoBehaviour {

    #region Variables

    public bool infected = false;
    public Material nonInfectedMat;
    public Material infectedMat;
    public float targetDistIC;
    public GameObject[] CitizensIC;

    #endregion

    private void Awake()
    {
        if (Random.value >= 0.33)
        {
            IsNotInfected();
        }
        else
        {
            IsInfected();
        }
    }

    public void IsNotInfected()
    {
        //this.GetComponent<AgentController>().UpdateCitizenArray();
        this.tag = "Citizen";
        this.GetComponent<Renderer>().material = nonInfectedMat;
        infected = false;
       // Debug.Log("not infected");
    }
    public void IsInfected()
    {
        //this.GetComponent<AgentController>().UpdateCitizenArray();
        this.tag = "Infected";
        this.GetComponent<Renderer>().material = infectedMat;
        infected = true;
        //Debug.Log("infected");
        //invoke("infector every 2 seconds");
        InvokeRepeating("Infector", 5f, 5f);
    }

    private void Infector()
    {
        foreach (GameObject C in CitizensIC)
        {
            if (Vector3.Distance(this.transform.position, C.transform.position) <= 5f && C != this.gameObject && Random.value > 0.6f)
            {
                C.GetComponent<infectionController>().SendMessage("IsInfected");
            }
        }
    }
}
