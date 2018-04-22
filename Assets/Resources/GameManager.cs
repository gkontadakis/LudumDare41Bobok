using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject _player, _playerFemale, _playerBad;

    private Vector3 _startPlayerPos, _startPlayerFemalePos;
    private Quaternion _startPlayerRot, _startPlayerFemaleRot;

    private GameObject _infoImage;
    private Text _infoText;

    private GameObject _controlsPanel;

    private readonly Vector3 _firstCameraCheckPoint = new Vector3(7.77f, 8.11f, -16.69619f);

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
        GtMainGame
    }

    private GameTransition _gameTransition;

    // Use this for initialization
    void Start ()
	{
	    _player = GameObject.Find("Player");
	    _player.tag = "Untagged";
        _playerFemale = GameObject.Find("PlayerFemale");
	    _startPlayerPos = _player.transform.position;
	    _startPlayerFemaleRot = _playerFemale.transform.rotation;
	    _startPlayerRot = _player.transform.rotation;
	    _startPlayerFemalePos = _playerFemale.transform.position;
	    _playerBad = GameObject.Find("PlayerBad");
	    _playerBad.SetActive(false);
        _infoImage = GameObject.Find("InfoImage");
	    _infoImage.SetActive(false);
	    _gameTransition = GameTransition.GtCameraDown;
	    _infoText = _infoImage.transform.GetChild(0).GetComponent<Text>();
	    _introSkipped = false;
	    _controlsPanel = GameObject.Find("ControlsPanel");
        _controlsPanel.SetActive(false);
	    _controlsShown = false;

	    ShowMessage("If you cannot stand the intro press Space to skip", Color.black, 3.0f);
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
	            ShowMessage("Cugood: Nooooo!!! In my tetra honor as Cugood I' ll find you!", Color.blue, 4.0f);
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
            default:
	            break;
        }

	    if (_gameTransition != GameTransition.GtMainGame && Input.GetKeyDown(KeyCode.Space))
	    {
	        SkipIntro();
	    }
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
        if (_player.GetComponent<ParticleSystem>() == null)
            return;
        _player.GetComponent<ParticleSystem>().Stop();
        _playerFemale.GetComponent<ParticleSystem>().Stop();
        Destroy(_player.GetComponent<ParticleSystem>());
        Destroy(_playerFemale.GetComponent<ParticleSystem>());
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
}
