using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardBalloon : MonoBehaviour {

    [SerializeField]
    private List<BalloonData> balloons;

    [SerializeField]
    private TimerObserver RevealTimer;

    [Tooltip("Prefab du pointeur vers l'objet")]
    [SerializeField]
    private GameObject DirectionnalIndicatorPrefab;

    private bool bonusStarWasPopped = false;
    private bool alreadyRevealed = false;
    
    private GameObject Indicator3D;
    private int currentBalloon = 0;

    [SerializeField]
    private BalloonData bonusStar;

    /// <summary>
    /// Vitesse de la révélation finale
    /// </summary>
    [Range(0.5f,10f)]
    [SerializeField]
    private float revealDuration = 5f;
    private float speedFactor;

    [SerializeField]
    private AudioSource timerAudioSource;
    private AudioSource balloonAudioSource;

    [SerializeField]
    private AudioClip focusSound;

    [SerializeField]
    private List<AudioClip> balloonSound;

    [SerializeField]
    private AudioClip timerSound;

    [SerializeField]
    private AudioClip bonusSound;


    [System.Serializable]
    public class BalloonData
    {
        /// <summary>
        /// Champ du ballon dans le score board
        /// </summary>
        [SerializeField]
        private GameObject balloon;
        /// <summary>
        /// Champ texte du score du ballon
        /// </summary>
        [SerializeField]
        private UnityEngine.UI.Text scoreDisplay;
        /// <summary>
        /// Champ 
        /// </summary>
        [SerializeField]
        private UnityEngine.UI.Text timeDisplay;

        public float Time
        {
            get;set;
        }
        public int Score
        {
            get; set;
        }


        public void filedBalloon()
        {
            if(this.balloon != null)
            {
                this.balloon.SetActive(true);
                //TODO Launch animation
            }
        }

        public void displayScore()
        {
            if(this.scoreDisplay != null)
            {
                this.scoreDisplay.gameObject.SetActive(true);
                this.scoreDisplay.text = this.Score.ToString();
            }
        }

        public void displayTime()
        {
            if (this.timeDisplay != null)
            {
                this.timeDisplay.gameObject.SetActive(true);
                this.timeDisplay.text = this.Time.ToString("0.00")+"s";
            }
        }

        public void cleanBoard()
        {
            if(this.scoreDisplay != null)
            {
                this.scoreDisplay.gameObject.SetActive(false);
            }

            if (this.timeDisplay != null)
            {
                this.timeDisplay.gameObject.SetActive(false);
            }

            if (this.balloon != null)
            {
                this.balloon.SetActive(false);
            }
        }
    }




	// Use this for initialization
	void Start ()
    {
        ScoreBoard.OnBalloonPopped += addBalloonField;
        ScoreBoard.OnBoardStatusChange += handleChange;
        ScoreBoard.OnBoardGaze += startRevealing;
        this.balloonAudioSource = this.GetComponent<AudioSource>();
    }


    private void OnDestroy()
    {
        ScoreManager.onBalloonPopped -= addBalloonField;
        ScoreBoard.OnBoardStatusChange -= handleChange;
        ScoreBoard.OnBoardGaze -= startRevealing;
    }

    private void handleChange(ScoreBoard.boardStatus status)
    {

        if (status == ScoreBoard.boardStatus.END)
        {
            //Starting focus sound
            timerAudioSource.clip = focusSound;
            timerAudioSource.loop = true;
            timerAudioSource.volume = SoundMixManager.getVolume(SoundMixManager.SoundType.SB_FOCUS);
            timerAudioSource.Play();

            //Display Indicator
            if (DirectionnalIndicatorPrefab != null)
                Indicator3D = Instantiate(DirectionnalIndicatorPrefab, this.transform);
            
            //startRevealing();
            if (bonusStar != null)
                bonusStar.cleanBoard();
            foreach (BalloonData bd in balloons)
            {
                bd.cleanBoard();
            }

            alreadyRevealed = false;
        }
        else
        {
            if (Indicator3D != null)
                Destroy(Indicator3D.gameObject);
        }
          
        if (status == ScoreBoard.boardStatus.INITIAL || status == ScoreBoard.boardStatus.FINAL || status == ScoreBoard.boardStatus.LAST_LEVEL_FINAL)
        {
            //On stop le reveal.
            StopAllCoroutines();
            //On libère le timer (au cas où)
            
            overrideRevealTimer(false);
            currentBalloon = 0;
            bonusStarWasPopped = false;
            if (bonusStar != null)
                bonusStar.cleanBoard();

            foreach (BalloonData bd in balloons)
            {
                bd.cleanBoard();
            }

            if(timerAudioSource != null)
            {
                timerAudioSource.Stop();
            }
            if (balloonAudioSource != null)
            {
                balloonAudioSource.Stop();
            }
        }
    }

    private void addBalloonField(float time, int scoreGain, bool isBonus)
    {
        if(currentBalloon < balloons.Count && !isBonus)
        {
            //May have ordering issue to fix.
            BalloonData b = balloons[currentBalloon];
            b.Score = scoreGain;
            b.Time = time;
            b.filedBalloon();
            currentBalloon++;
        }

        if (isBonus && bonusStar != null)
        {
            bonusStarWasPopped = true;
            bonusStar.filedBalloon();
            bonusStar.Score = scoreGain;
        }
    }

    private void overrideRevealTimer(bool b)
    {
        if (RevealTimer != null)
        {
            RevealTimer.TimeOverride = b;
        }
    }


    private void updateRevealTimer(float f)
    {
        if (RevealTimer != null && RevealTimer.TimeOverride)
        {
            RevealTimer.setTime(f);
        }
    }

    public void startRevealing()
    {
        if (Indicator3D != null)
            Destroy(Indicator3D.gameObject);
        if (!alreadyRevealed)
        {

            alreadyRevealed = true;
            StartCoroutine(results());
        }
    }

    private IEnumerator results()
    {
        //  we stop focus sound
        timerAudioSource.Stop(); 
        timerAudioSource.loop = false;


        float finalTime = TimerManager.getTimeStamp();
        int counter = 0;
        float timeReveal = 0;
        float balloonFrequency = finalTime /balloons.Count;
        overrideRevealTimer(true);
        updateRevealTimer(timeReveal);

        speedFactor = finalTime / revealDuration;

        timerAudioSource.PlayOneShot(timerSound, SoundMixManager.getVolume(SoundMixManager.SoundType.SB_TIMER));

        while(timeReveal < finalTime)
        {
            timeReveal += Time.deltaTime * speedFactor;
            updateRevealTimer(timeReveal);
            //while (balloons.Count > counter && timeReveal > balloons[counter].Time)
            if (balloons.Count > counter && timeReveal > counter * balloonFrequency)
            {
                balloons[counter].filedBalloon();
                balloons[counter].displayScore();
                balloons[counter].displayTime();
                balloonAudioSource.PlayOneShot(balloonSound[counter], SoundMixManager.getVolume(SoundMixManager.SoundType.SB_BALLOONS));
                counter++;
                //yield return new WaitForSeconds(0.75f);
            }
            yield return null;
        }
        //yield return new WaitForSeconds(0.75f);
        if (bonusStar != null && bonusStarWasPopped)
        {
            balloonAudioSource.PlayOneShot(bonusSound, SoundMixManager.getVolume(SoundMixManager.SoundType.SB_BONUS));
            bonusStar.filedBalloon();
            bonusStar.displayScore();
            bonusStarWasPopped = false;
        }
        overrideRevealTimer(false);
        yield return null;
    }

}
