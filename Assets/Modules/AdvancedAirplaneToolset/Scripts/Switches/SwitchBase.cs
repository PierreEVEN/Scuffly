
// Interface d'un bouton simple sur lequel on peut faire une action au clic
public abstract class SwitchBase : PlaneComponent
{
    // Texte de description du bouton
    public string Desc = "";

    // Appuis sur le bouton
    public abstract void Switch();

    // Highligh du bouton quand on passe la souris dessus
    public void StartOver()
    {
        gameObject.layer = 3;
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.layer = 3;
    }

    public void StopOver()
    {
        gameObject.layer = 0;
        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).gameObject.layer = 0;
    }
}
