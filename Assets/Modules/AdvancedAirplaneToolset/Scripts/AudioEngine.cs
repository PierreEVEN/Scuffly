
using UnityEngine;

// Gestion de l'audio d'un moteur (en 3 phases : mise en route / activé / arret). //@TODO find a better way to handle engine audio with Wwise
public class AudioEngine : MonoBehaviour
{
    // Events start et stop pour chaque phase
    bool isPlayingStart = false;
    public AK.Wwise.Event PlayStart;
    public AK.Wwise.Event StopStart;
    bool isPlayingStop = false;
    public AK.Wwise.Event PlayStop;
    public AK.Wwise.Event StopStop;
    bool isPlayingIdle = false;
    public AK.Wwise.Event PlayIdle;
    public AK.Wwise.Event StopIdle;

    // volume de chaque pahse (transition progressive d'une phase a l'autre au niveau du volume)
    public AK.Wwise.RTPC StartVolumeProperty;
    public AK.Wwise.RTPC StopVolumeProperty;
    public AK.Wwise.RTPC IdleVolumeProperty;

    // Vulume de chaque phase
    float StartVolume = 0;
    float StopVolume = 0;
    float IdleVolume = 0;

    // Etat du moteur (1 = activé, 0 = eteint)
    float startPercent = 0;
    bool isMovingUp = true;
    bool isStartOrStop = false;

    private void OnDisable()
    {
        StopIdle.Post(gameObject);
        StopStop.Post(gameObject);
        StopStart.Post(gameObject);
    }

    // Met a jour l'etat du moteur
    public void SetStartPercent(float percent)
    {
        if (startPercent == percent)
            return;
        isStartOrStop = percent > 0.01 && percent < 0.99;
        if (percent > startPercent)
            isMovingUp = true;
        else
            isMovingUp = false;
        startPercent = percent;
    }

    void Update()
    {
        // Met a jour les differents niveaux de volume pour chaque etat
        StartVolume = Mathf.Clamp01(StartVolume + Mathf.Clamp(isStartOrStop && isMovingUp ? 1 : -1, -Time.deltaTime, Time.deltaTime));
        StopVolume = Mathf.Clamp01(StopVolume + Mathf.Clamp(isStartOrStop && !isMovingUp ? 1 : -1, -Time.deltaTime, Time.deltaTime));
        IdleVolume = Mathf.Clamp01(IdleVolume + Mathf.Clamp(!isStartOrStop && startPercent > 0.5f ? 1 : -1, -Time.deltaTime, Time.deltaTime));


        // Pour chaque etat : joue le son ou le coupe en fonction de si son niveau de volume > 0.01
        if (StartVolume > 0.01 && !isPlayingStart)
        {
            PlayStart.Post(gameObject);
            isPlayingStart = true;
        }
        if (StartVolume < 0.01 && isPlayingStart)
        {
            StopStart.Post(gameObject);
            isPlayingStart = false;
        }

        if (StopVolume > 0.01 && !isPlayingStop)
        {
            PlayStop.Post(gameObject);
            isPlayingStop = true;
        }
        if (StopVolume < 0.01 && isPlayingStop)
        {
            StopStop.Post(gameObject);
            isPlayingStop = false;
        }

        if (IdleVolume > 0.01 && !isPlayingIdle)
        {
            PlayIdle.Post(gameObject);
            isPlayingIdle = true;
        }
        if (IdleVolume < 0.01 && isPlayingIdle)
        {
            StopIdle.Post(gameObject);
            isPlayingIdle = false;
        }

        // Transmet le volume de chaque phase à wwise
        StartVolumeProperty.SetValue(gameObject, StartVolume * 100);
        StopVolumeProperty.SetValue(gameObject, StopVolume * 100);
        IdleVolumeProperty.SetValue(gameObject, IdleVolume * 100);
    }
}
