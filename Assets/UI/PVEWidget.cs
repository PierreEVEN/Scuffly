using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVEWidget : MonoBehaviour
{
    public GameObject KillText;

    public GameObject EndContainer;
    public GameObject EndText;
    GamemodePVE owner;
    public void Setup(GamemodePVE owner)
    {
        this.owner = owner;
        owner.OnLost.AddListener(OnLost);
        owner.OnKill.AddListener(OnKill);
        OnKill();
        EndContainer.SetActive(false);
    }

    void OnLost()
    {
        EndContainer.SetActive(true);
        EndText.GetComponent<Text>().text = "You survived " + (int)(owner.timeSurvived / 60) + " minutes...\n\n... and destroyed " + owner.killCount / 2 + " enemies!";
    }
    void OnKill()
    {
        KillText.GetComponent<Text>().text = "Enemy destroyed : " + owner.killCount / 2;
    }

}
