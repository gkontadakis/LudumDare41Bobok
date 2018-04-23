using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject _player, _playerFemale, _playerFemale2, _playerBad;

    private Vector3 _startPlayerPos, _startPlayerFemale2Pos;
    private Quaternion _startPlayerRot, _startPlayerFemale2Rot;

    private GameObject _infoImage;
    private Text _infoText;

    private GameObject _controlsPanel;

    private readonly Vector3 _firstCameraCheckPoint = new Vector3(7.77f, 8.11f, -16.69619f);
    private Vector3 _lastCameraCheckPoint = new Vector3(108.47f, 45, -16.69619f);

    private float _msgDt;

    private bool MessageEnded
    {
        get { return _msgDt <= 0; }
    }

    public Vector3 PlayerSpawnLocation;

    private bool _introSkipped;

    private bool _controlsShown;

    private enum GameTransition
    {
        GtNone,
        GtCameraDown,
        GtCameraDownDone,
        GtCuGoodLoveLine,
        GtCuGoodLoveLineDone,
        GtCutieLoveLine,
        GtCutieLoveLineDone,
        GtCubadComing,
        GtCubadComingDone,
        GtCubadTaking,
        GtCubadTakingDone,
        GtCubadGoing,
        GtCubadGoingDone,
        GtCugoodCry,
        GtCugoodCryDone,
        GtMainGame,
        GtMainGameDone,
        GtCugoodFindsCutie,
        GtCugoodFindsCutieDone,
        GtCutierIntro,
        GtCutierIntroDone,
        GtCugoodOops,
        GtCugoodOopsDone,
        GtCutierFindHer,
        GtCutierFindHerDone,
        GtCugoodCompliment,
        GtCugoodComplimentDone,
        GtCutierOh,
        GtCutierOhDone,
        GtCameraUp,
        GtCameraUpDone,
        GtEnd,
        GtEndDone,
    }

    private GameTransition _gameTransition;

    // Use this for initialization
    void Start ()
	{
	    _player = GameObject.Find("Player");
	    _player.tag = "Untagged";
        _playerFemale = GameObject.Find("PlayerFemale");
	    _playerFemale2 = GameObject.Find("PlayerFemale2");
        _startPlayerPos = _player.transform.position;
	    _startPlayerRot = _player.transform.rotation;
	    _startPlayerFemale2Pos = _playerFemale2.transform.position;
        _startPlayerFemale2Rot = _playerFemale2.transform.rotation;
	    _playerBad = GameObject.Find("PlayerBad");
	    _playerBad.SetActive(false);
        _infoImage = GameObject.Find("InfoImage");
	    _infoImage.SetActive(false);
	    
	    _infoText = _infoImage.transform.GetChild(0).GetComponent<Text>();
	    _introSkipped = false;
	    _controlsPanel = GameObject.Find("ControlsPanel");
        _controlsPanel.SetActive(false);
	    _controlsShown = false;

	    ShowMessage("If you cannot stand the intro press Space to skip", Color.black, 3.0f);
	    _player.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
	    _playerFemale.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

	    _gameTransition = GameTransition.GtCameraDown;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    _msgDt -= Time.smoothDeltaTime;

        //_player.transform.position = _startPlayerPos;
        switch (_gameTransition)
	    {
	        case GameTransition.GtCameraDown:
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _firstCameraCheckPoint, 0.5f * Time.smoothDeltaTime);
	            _player.transform.position = _startPlayerPos + new Vector3(Mathf.PingPong(Time.time, 0.5f), 0, 0);
	            _player.transform.rotation = _startPlayerRot;
	            if (MessageEnded)
	            {
	                HideMessage();
	            }

                if (Vector3.Distance(Camera.main.transform.position, _firstCameraCheckPoint) < 0.1f)
	            {
	                _gameTransition = GameTransition.GtCameraDownDone;
	            }
	            break;
	        case GameTransition.GtCameraDownDone:
	            Camera.main.transform.position = _firstCameraCheckPoint;
	            _gameTransition = GameTransition.GtCuGoodLoveLine;
	            ShowMessage("Cugood: Ahhh... Pssstfffs mouts mouts", Color.blue, 2.5f);
                break;
	        case GameTransition.GtCuGoodLoveLine:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCuGoodLoveLineDone;
                }
	            break;
	        case GameTransition.GtCuGoodLoveLineDone:
	            Camera.main.transform.position = _firstCameraCheckPoint;
	            _gameTransition = GameTransition.GtCutieLoveLine;
	            ShowMessage("Cutie: Ahhh... Pssstfffs mouts mouts", Color.magenta, 2.5f);
                break;
	        case GameTransition.GtCutieLoveLine:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCutieLoveLineDone;
	            }
                break;
	        case GameTransition.GtCutieLoveLineDone:
	            Camera.main.transform.position = _firstCameraCheckPoint;
	            DestroyHeartParticles();
	            _gameTransition = GameTransition.GtCubadComing;
	            EnableCubad();
	            _player.transform.GetChild(2).GetComponent<Rigidbody>().AddForce(10 * Vector3.left, ForceMode.Impulse);
	            _playerFemale.transform.GetChild(3).GetComponent<Rigidbody>().AddForce(10 * Vector3.right, ForceMode.Impulse);
                break;
            case GameTransition.GtCubadComing:
                _playerBad.transform.GetChild(0).GetComponent<Rigidbody>().velocity = Vector3.up;

                if (MessageEnded)
                {
                    _gameTransition = GameTransition.GtCubadComingDone;
                }
                break;
	        case GameTransition.GtCubadComingDone:
                ShowMessage("Cubad: I will take this beautiful lady just because I can", Color.black, 4.0f);
	            _gameTransition = GameTransition.GtCubadTaking;
                break;
	        case GameTransition.GtCubadTaking:
	            _playerBad.transform.GetChild(0).GetComponent<Rigidbody>().velocity = Vector3.up * 1.0f;
	            _playerFemale.transform.GetChild(0).GetComponent<Rigidbody>().velocity = Vector3.up * 2.5f;
                if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCubadTakingDone;
	            }
	            break;
	        case GameTransition.GtCubadTakingDone:
	            ShowMessage("Cutie: Ahhh! Help me Cugood!", Color.magenta, 2.5f);
	            _gameTransition = GameTransition.GtCubadGoing;
                break;
	        case GameTransition.GtCubadGoing:
	            _playerBad.transform.GetChild(0).GetComponent<Rigidbody>().velocity = (Vector3.up + 2.5f * Vector3.right) * 2.5f;
	            _playerFemale.transform.GetChild(0).GetComponent<Rigidbody>().velocity = (Vector3.up + 2.5f * Vector3.right) * 2.5f;
                if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCubadGoingDone;
	            }
	            break;
	        case GameTransition.GtCubadGoingDone:
	            _playerBad.SetActive(false);
	            _playerFemale.SetActive(false);
	            ShowMessage("Cugood: Nooooo !!! In my tetra honor as Cugood I' ll find you even if you somehow can fly and I can't!", Color.blue, 6.0f);
	            _player.transform.GetChild(0).GetComponent<Rigidbody>().AddForce(50 * Vector3.up, ForceMode.Impulse);
                _gameTransition = GameTransition.GtCugoodCry;
	            break;  
	        case GameTransition.GtCugoodCry:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCugoodCryDone;
	            }
                break;
	        case GameTransition.GtCugoodCryDone:
	            HideMessage();
	            _player.tag = "Player";
	            _gameTransition = GameTransition.GtMainGame;
                SkipIntro();
                //Game sound starts
                break;
            case GameTransition.GtMainGame:
                if (_player == null)    // When deleted then find new player again
                    _player = GameObject.Find("Player");
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(_player.transform.GetChild(0).position.x, Camera.main.transform.position.y, Camera.main.transform.position.z), Time.smoothDeltaTime);
                break;
	        case GameTransition.GtMainGameDone:
	            ShowMessage("Cugood: My beloved Cutie! Once again we are together!", Color.blue, 4.0f);
	            _gameTransition = GameTransition.GtCugoodFindsCutie;
                break;
	        case GameTransition.GtCugoodFindsCutie:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCugoodFindsCutieDone;
                }
	            break;
	        case GameTransition.GtCugoodFindsCutieDone:
	            ShowMessage("Cutier: Gugood I am not Cutie I am Cutier. Cutie is in another box cage.", Color.magenta, 6.0f);
                _gameTransition = GameTransition.GtCutierIntro;
	            break;
	        case GameTransition.GtCutierIntro:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCutierIntroDone;
	            }
	            break;
	        case GameTransition.GtCutierIntroDone:
	            ShowMessage("Cugood: Oh Sh... Ehm Oops!", Color.blue, 3.0f);
	            _gameTransition = GameTransition.GtCugoodOops;
	            break;
	        case GameTransition.GtCugoodOops:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCugoodOopsDone;
	            }
                break;
	        case GameTransition.GtCugoodOopsDone:
	            ShowMessage("Cutier: Find her and defeat Cubad once and for all !!! ", Color.magenta, 6.0f);
	            _gameTransition = GameTransition.GtCutierFindHer;
                break;
	        case GameTransition.GtCutierFindHer:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCutierFindHerDone;
	            }
	            break;
	        case GameTransition.GtCutierFindHerDone:
	            ShowMessage("Cugood: She will be fine... You know you are very cute also...", Color.blue, 6.0f);
	            _player.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                _gameTransition = GameTransition.GtCugoodCompliment;
	            break;
	        case GameTransition.GtCugoodCompliment:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCugoodComplimentDone;
	            }
	            break;
	        case GameTransition.GtCugoodComplimentDone:
	            ShowMessage("Cutier: Oh Cugood... ", Color.magenta, 4.0f);
	            _playerFemale2.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                _gameTransition = GameTransition.GtCutierOh;
	            break;
	        case GameTransition.GtCutierOh:
	            if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtCutierOhDone;
	            }
	            break;
	        case GameTransition.GtCutierOhDone:
	            _gameTransition = GameTransition.GtCameraUp;
                _playerBad.SetActive(true);
	            HideMessage();
                break;
	        case GameTransition.GtCameraUp:
	            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, _lastCameraCheckPoint, 0.5f * Time.smoothDeltaTime);
	            _playerBad.transform.GetChild(0).GetComponent<Rigidbody>().velocity = Vector3.up * 1.0f;

	            if (Vector3.Distance(Camera.main.transform.position, _lastCameraCheckPoint) < 1f)
	            {
	                _gameTransition = GameTransition.GtCameraUpDone;
	            }
	            break;
            case GameTransition.GtCameraUpDone:
                ShowMessage("Cubad: The end ...", Color.black, 4.0f);
                _gameTransition = GameTransition.GtEnd;
                break;
	        case GameTransition.GtEnd:
	            _playerBad.transform.GetChild(0).GetComponent<Rigidbody>().velocity = Vector3.up * 1.0f;
                if (MessageEnded)
	            {
	                _gameTransition = GameTransition.GtEndDone;
                }
                break;
	        case GameTransition.GtEndDone:
	            if (MessageEnded)
	            {
	                Application.Quit();
	            }
	            break;
            default:
	            break;
        }

	    if (_gameTransition != GameTransition.GtMainGame && Input.GetKeyDown(KeyCode.Space))
	    {
	        SkipIntro();
	    }
	    _playerFemale2.transform.position = _startPlayerFemale2Pos;
	    _playerFemale2.transform.rotation = _startPlayerFemale2Rot;
	}

    void SkipIntro()
    {
        if (_introSkipped) return;

        Camera.main.transform.position = _firstCameraCheckPoint;
        DestroyHeartParticles();
        _playerBad.SetActive(false);
        _playerFemale.SetActive(false);
        _player.tag = "Player";
        HideMessage();
        _gameTransition = GameTransition.GtMainGame;

        _playerBad.GetComponent<BoxController>().ResetToPosition(new Vector3(_lastCameraCheckPoint.x, _lastCameraCheckPoint.y - 10, 0));

        _introSkipped = true;
    }

    void HideMessage()
    {
        if (!_infoImage.activeInHierarchy) return;
        _infoImage.SetActive(false);
        _infoText.text = "";
        _msgDt = 0;
    }

    void ShowMessage(string message, Color color, float time = 1.0f)
    {
        _infoImage.SetActive(true);
        _infoText.text = message;
        _infoText.color = color;
        _msgDt = time;
    }

    void DestroyHeartParticles()
    {
        if (_player.transform.GetChild(0).GetComponent<ParticleSystem>() == null)
            return;
        _player.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        //var psMain = _player.GetComponent<ParticleSystem>().main;
        //psMain.startDelay = 0.0f;
        _playerFemale.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        //Destroy(_player.GetComponent<ParticleSystem>());
        //Destroy(_playerFemale.GetComponent<ParticleSystem>());
    }

    void EnableCubad()
    {
        _playerBad.SetActive(true);
        ShowMessage("Cubad: Sorry for interupting", Color.black, 4.0f);
    }

    public void ToggleControlPanel()    // Called by Buttons in Editor
    {
        _controlsShown = !_controlsShown;
        _controlsPanel.SetActive(_controlsShown);
    }

    public void StartEndingSequence()
    {
        _gameTransition = GameTransition.GtMainGameDone;
    }
}
