using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject HintText;

    public GameObject LabelsContainer;
    public GameObject LabelUI;

    public List<PlaneLabel> LabelList = new List<PlaneLabel>();
    
    void UpdateLabels()
    {
        if (!LabelsContainer)
            return;

        // Desactive les labels en mode realiste
        if (GameplayManager.Singleton.CurrentSettings.Difficulty == Difficulty.Realistic)
        {
            foreach (var Label in LabelList)
                Destroy(Label.gameObject);
            LabelList.Clear();
            return;
        }

        // recupere la liste des labels a afficher
        List<GameObject> desiredTargets = new List<GameObject>();
        foreach (var target in PlaneActor.PlaneList)
        {
            if (Vector3.Distance(Camera.main.transform.position, target.transform.position) < 200000 && target != PlayerManager.LocalPlayer.controlledPlane)
            {
                desiredTargets.Add(target.gameObject);
            }
        }

        // Supprime les labels obsoletes
        for (int i = 0; i < LabelList.Count; ++i)
        {
            if (desiredTargets.Contains(LabelList[i].target))
            {
                desiredTargets.Remove(LabelList[i].target);
            }
            else
            {
                Destroy(LabelList[i].gameObject);
                LabelList.RemoveAt(i);
            }
        }

        // Ajoute les labels manquants
        foreach (var target in desiredTargets)
        {
            GameObject newLabel = Instantiate(LabelUI, LabelsContainer.transform);
            PlaneLabel labelScript = newLabel.GetComponent<PlaneLabel>();
            labelScript.target = target;
            LabelList.Add(labelScript);
        }

    }

    void Update()
    {
        var selectedSwitch = PlayerManager.LocalPlayer.GetComponent<CameraManager>().selectedSwitch;
        HintText.GetComponent<Text>().text = selectedSwitch ? selectedSwitch.Desc : "";

        UpdateLabels();
    }
}
