
using System.Collections.Generic;

enum RotatingSwitchStates
{
    None,
    IndoorLightLevel,

}

//@TODO
public class RotatingSwitch : SwitchBase
{
    RotatingSwitchStates modifiedProperty;
    public List<float> States;
    public List<float> PerStateRotation;

    public AK.Wwise.Event SwitchAudio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnValidate()
    {
        if (States.Count > PerStateRotation.Count)
            PerStateRotation.Add(0);
        if (PerStateRotation.Count > States.Count)
            States.Add(0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (modifiedProperty)
        {
            case RotatingSwitchStates.None:
                break;
            case RotatingSwitchStates.IndoorLightLevel:
                break;
        }
    }

    public override void Switch()
    {
        SwitchAudio.Post(gameObject);

        switch (modifiedProperty)
        {
            case RotatingSwitchStates.None:
                break;
            case RotatingSwitchStates.IndoorLightLevel:
                break;
        }

    }
}
