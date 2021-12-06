
/// <summary>
/// A simple switch interface. handle all the required behaviour to implement many kind of button
/// </summary>
public abstract class SwitchBase : PlaneComponent
{
    /// <summary>
    /// Description of the button
    /// </summary>
    public string Desc = "";

    /// <summary>
    /// Event called when the button is pressed
    /// </summary>
    public abstract void Switch();

    /// <summary>
    /// Event called when the button is released
    /// </summary>
    public abstract void Release();

    /// <summary>
    /// Event called when the button is Overred by the mouse
    /// </summary>
    public void StartOver()
    {
        // set the object layer to 3 to enable outline post process effect
        gameObject.layer = 3;
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.layer = 3;
    }

    /// <summary>
    /// Event called when the mouse stop over this button
    /// </summary>
    public void StopOver()
    {
        // Set the object layer back to 0
        gameObject.layer = 0;
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.layer = 0;
    }
}
