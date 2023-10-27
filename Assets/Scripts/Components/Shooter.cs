using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Shooter : MonoBehaviour
{
	public Transform _transform;
	public Transform _transform_StartBall;
	public Renderer _renderer_StartBall;
	public Renderer _renderer_StartBall_Shadow;
	public Transform _transform_EndBall;
	public Renderer _renderer_EndBall;
	public Renderer _renderer_EndBall_Shadow;
	public TextMesh _textMesh_BallAdd;
	public TextMesh _textMesh_BallCount;
	public TextMesh _textMesh_Skip;

    //public TextMesh _textMesh_Turn;
    public Transform _tr_WallUp;

	/////////////////////////////////////////////////////////////
	//const int PurchaseSpeedTurn = -1;
	//const int AdRewardSpeedTurn = 100;
	//int _adRewardSpeedTurn = 0;
	/////////////////////////////////////////////////////////////
	///
	public static int SkipGameTurn = 100;

	// powerMode
	[HideInInspector]
	public int AdRewardPowerModeTurn;
	[HideInInspector]
	public bool IsHandleUserEarnedRewardPowerModeCallback = false;

	// speedMode
	[HideInInspector]
	public int AdRewardSpeedModeTurn;
	[HideInInspector]
	public bool IsHandleUserEarnedRewardSpeedCallback = false;
	[HideInInspector]
	public int _ballSpeed = 1;

	float _speedPerFrame;
	float _bottomLine;

	public List<Ball> _ball_List = new List<Ball>();

	public List<Ball> _ball_ReadyToShoot_List = new List<Ball> ();
	public List<Ball> _ball_OnMove_List = new List<Ball>();
	public List<Ball> _ball_OnGather_List = new List<Ball> ();
	public List<Ball> _ball_TurnEnded_List = new List<Ball> ();
	public List<Ball> _ball_Spare_List = new List<Ball>();

	List<IEnumerator> _thread_Move_List = new List<IEnumerator>();
	List<IEnumerator> _thread_Move_Expired_List = new List<IEnumerator>();
	List<IEnumerator> _thread_GatherBall_List = new List<IEnumerator>();
	List<IEnumerator> _thread_GatherBall_Expired_List = new List<IEnumerator>();
	IEnumerator _thread_Skip;
	IEnumerator _thread_CheckTurnEnd;
	System.Action _callback_TurnEnd;

	public List<Ball> _pickUpBall_List = new List<Ball>();
	List<IEnumerator> _thread_PickUpDrop_List = new List<IEnumerator>();
	List<IEnumerator> _thread_PickUpDrop_Expired_List= new List<IEnumerator>();
	IEnumerator _thread_AddBall;

	List<BrickBreakEffect> _breakEffect_List = new List<BrickBreakEffect>();
	IEnumerator _thread_GameOver;

	public int _returnBallCount;
	public bool _returnedEmptyBall;
	public bool _existHitBall;
//	public bool _isEveryBallUnderBrick;
	public bool _ableToSkip;
    public float _skipButtonTime = 0.0f;

	public ParticleSystem PowerModeParticleSystem;

	public Vector3 _turnStartPosition;
	Vector3 _turnEndPosition;

	public int OriginBallCount { get; set; }

	System.Text.StringBuilder _strBuilder = new System.Text.StringBuilder();

	MenuButton_Multiple _speedModeComponent = null;
	MenuButton_PowerMode _powerModeComponent = null;

	public void Initialize()
	{
		_transform = transform;

		_renderer_StartBall.transform.localScale = Vector3.one * InGameController._BallRadius * 2;
		_renderer_StartBall.material = MaterialHolder._Material_PlayerBall_Position;

		_renderer_EndBall.transform.localScale = Vector3.one * InGameController._BallRadius * 2;
		_renderer_EndBall.material = MaterialHolder._Material_PlayerBall_Position;

		_renderer_StartBall_Shadow.transform.localScale = Vector3.one * InGameController._BallRadius * 2;
		_renderer_StartBall_Shadow.transform.localPosition = InGameController._PositionModifier_Shadow + Vector3.forward * 0.1f;
		_renderer_StartBall_Shadow.material = MaterialHolder._Material_BallShadow_Position;
		_renderer_StartBall_Shadow.enabled = false;

		_renderer_EndBall_Shadow.transform.localScale = Vector3.one * InGameController._BallRadius * 2;
		_renderer_EndBall_Shadow.transform.localPosition = InGameController._PositionModifier_Shadow + Vector3.forward * 0.1f;
		_renderer_EndBall_Shadow.material = MaterialHolder._Material_BallShadow_Position;
		_renderer_EndBall_Shadow.enabled = false;

		_textMesh_BallAdd.font = FontHolder.GetFont ();
		_textMesh_BallAdd.GetComponent<Renderer>().material = _textMesh_BallAdd.font.material;
		_textMesh_BallAdd.color = Static_ColorConfigs._Color_Ball;
		_textMesh_BallAdd.text = "+1";

		_textMesh_BallCount.font = FontHolder.GetFont ();
		_textMesh_BallCount.GetComponent<Renderer>().material = _textMesh_BallAdd.font.material;
		_textMesh_BallCount.color = Static_ColorConfigs._Color_Ball;
		_textMesh_BallCount.text = "+1";

		_textMesh_Skip.font = FontHolder.GetFont ();
		_textMesh_Skip.GetComponent<Renderer>().material = _textMesh_Skip.font.material;
		_textMesh_Skip.color = Static_ColorConfigs._Color_TextOrange;
		_textMesh_Skip.text = Static_TextConfigs._Skip;

		_textMesh_Skip.transform.localScale = Vector3.one * 0.15f;

		if((float)Screen.height / (float)Screen.width < 3.5f/2.5f)
			_textMesh_Skip.transform.localPosition = Vector3.up * InGameController._BrickHeight * 1.5f + Vector3.back * 0.6f;
		else
			_textMesh_Skip.transform.localPosition = Vector3.down * 0.5f + Vector3.back * 0.6f;

		if ((float)Screen.height / (float)Screen.width < 3.5f / 2.5f)
			_textMesh_BallCount.transform.localScale = Vector3.one * 0.075f;
		else
			_textMesh_BallCount.transform.localScale = Vector3.one * 0.15f;

		_speedModeComponent = FindObjectOfType<MenuButton_Multiple>();
		_powerModeComponent = FindObjectOfType<MenuButton_PowerMode>();
	}

    public void Reset()
    {
        Reset_Common();

        _turnStartPosition = Vector3.up * _bottomLine;
        _turnEndPosition = _turnStartPosition;

        _transform_StartBall.localPosition = _turnStartPosition;

        Ball ball = ObjectPoolHolder.GetObject<Ball>(_transform);
        ball.Initialize(InGameController._BallRadius);
        ball._transform.localPosition = _turnStartPosition;
        _ball_List.Add(ball);
        _ball_ReadyToShoot_List.Add(ball);

		OriginBallCount = _ball_List.Count;

		_textMesh_BallCount.gameObject.SetActive(false);
	}

    public void ResetSkip()
    {
        Reset_Common();

        _turnStartPosition = Vector3.up * _bottomLine;
        _turnEndPosition = _turnStartPosition;

        _transform_StartBall.localPosition = _turnStartPosition;


		SetBallList((UserData._Score_Best / SkipGameTurn) * SkipGameTurn);
		//OriginBallCount = (UserData._Score_Best / SkipGameTurn) * SkipGameTurn;
		//int defaultBall = 100;
		//float originHP = 500;
		//float defaultEXP = defaultBall - originHP / 100;
		//
		//float exp = Mathf.Max(OriginBallCount - defaultEXP, 0);
		//float reduceRatio = 1 - originHP / (originHP + exp);
		//int additionalPower = (int)(OriginBallCount * reduceRatio);
		//int shootCount = OriginBallCount - additionalPower;
		//int defaultPower = (int)(additionalPower / shootCount) + 1;
		//int powerUpCount = additionalPower % shootCount;
		//int frameGap = _ballSpeed == 1 ? 4 : 3;
		//
		//for (int i=0; i<shootCount; i++)
		//{
		//	Ball ball = ObjectPoolHolder.GetObject<Ball>(_transform);
		//	ball.name = "ball " + i;
		//	ball.Initialize(InGameController._BallRadius);
		//	ball._transform.localPosition = _turnStartPosition;
		//	_ball_List.Add(ball);
		//	_ball_ReadyToShoot_List.Add(ball);
		//}

////////////////////////////////////////////////////////////////////////////////////////////////

		//for (int i = 0; i < (UserData._Score_Best / SkipGameTurn) * SkipGameTurn; i++)
        //{
        //    Ball ball = ObjectPoolHolder.GetObject<Ball>(_transform);
        //    ball.name = "ball " + i;
        //    ball.Initialize(InGameController._BallRadius);
        //    ball._transform.localPosition = _turnStartPosition;
        //    _ball_List.Add(ball);
        //    _ball_ReadyToShoot_List.Add(ball);
        //}

        UpdateBallCountLabel();
        _textMesh_BallCount.gameObject.SetActive(true);
        UpdateBallCountPosition();
    }

	private void SetBallList(int originBallCount)
	{
		OriginBallCount = originBallCount;

		int defaultBall = Ball_Config.BallDefault;
		float originHP = Ball_Config.BallMaximum;
		float defaultEXP = defaultBall - originHP / 100;

		float exp = Mathf.Max(OriginBallCount - defaultEXP, 0);
		float reduceRatio = 1 - originHP / (originHP + exp);
		int additionalPower = (int)(OriginBallCount * reduceRatio);
		int shootCount = (OriginBallCount - additionalPower) >0 ? (OriginBallCount - additionalPower) : 1;
		int defaultPower = (int)(additionalPower / shootCount) + 1;
		int powerUpCount = additionalPower % shootCount;

		for (int i = 0; i < shootCount; i++)
		{
			Ball ball = ObjectPoolHolder.GetObject<Ball>(_transform);
			ball.name = "ball " + i;
			ball.Initialize(InGameController._BallRadius);
			ball._transform.localPosition = _turnStartPosition;
			_ball_List.Add(ball);
			_ball_ReadyToShoot_List.Add(ball);
		}
	}

    public void Continue()
	{
		Reset_Common ();

		_turnStartPosition = InGameController._BallPosition;
		_turnEndPosition = _turnStartPosition;

		_transform_StartBall.localPosition = _turnStartPosition;

		SetBallList(InGameController._BallCount);
		//for(int i = 0; i < InGameController._BallCount; i++)
		//{
		//	Ball ball = ObjectPoolHolder.GetObject<Ball> (_transform);
		//	ball.Initialize (InGameController._BallRadius);
		//	ball._transform.localPosition = _turnStartPosition;
		//	_ball_List.Add (ball);
		//	_ball_ReadyToShoot_List.Add(ball);
		//}

		NewTurnUpdateBallCountLabel();
		//UpdateBallCountLabel ();

		_textMesh_BallCount.gameObject.SetActive (true);
		UpdateBallCountPosition ();
	}

	void Reset_Common()
	{
        //Invoke("DelayReset", 0.1f);
        _skipButtonTime = 0.0f;
        _speedPerFrame = InGameController._BallRadius * 2f * 0.4f;
//		_frameGap = 4;
		_bottomLine = InGameController._BallRadius;

		ObjectPoolHolder.PutObjectList<Ball> (_ball_List);
		_ball_List.Clear ();
		
		ObjectPoolHolder.PutObjectList<Ball> (_pickUpBall_List);
		_pickUpBall_List.Clear ();
		
		ObjectPoolHolder.PutObjectList<BrickBreakEffect> (_breakEffect_List);
		_breakEffect_List.Clear ();
		
		_ball_ReadyToShoot_List.Clear ();
		_ball_OnMove_List.Clear ();
		_ball_OnGather_List.Clear ();
		_ball_TurnEnded_List.Clear ();
		_ball_Spare_List.Clear ();

		_transform_StartBall.gameObject.SetActive (true);
		_transform_EndBall.gameObject.SetActive (false);
		_textMesh_BallAdd.gameObject.SetActive (false);
		_textMesh_Skip.gameObject.SetActive (false);

		_thread_AddBall = null;
		_thread_CheckTurnEnd = null;
		_thread_Skip = null;
		
		_thread_Move_List.Clear ();
		_thread_GatherBall_List.Clear ();
		_thread_PickUpDrop_List.Clear ();
		_thread_GameOver = null;

		_callback_TurnEnd = null;

		OriginBallCount = 0;
	}

	public void InitSpeedAndPowerMode()
	{
		bool isActiveSpeedMode = PlayerPrefs.GetInt(PlayerPrefs_Config.ActiveSpeedMode, 1) != 0;
		bool isActivePowerMode = PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 1;

		AdRewardSpeedModeTurn = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, -1);
		AdRewardPowerModeTurn = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, -1);

		// 구매 유저에 대한 처리.
		if (StaticMethod.IsPurchase_Speed_Total_SmartShopReward())
		{
			SpeedMode(isActiveSpeedMode);
			PowerMode(isActivePowerMode);

			if(isActivePowerMode)
			{
				StopCoroutine("SetBallColor");
				StartCoroutine(SetBallColor(true));
				PowerModeParticleSystem.Play();
			}
		}
		else
		{
			if (isActiveSpeedMode && AdRewardSpeedModeTurn > 0)
			{
				SpeedMode();
			}
			else
			{
				SpeedMode(false);
			}

			if (isActivePowerMode && AdRewardPowerModeTurn > 0 && OriginBallCount >= PowerMode_Config.PowerModeLimitCount)
			{
				PowerMode();

				StopCoroutine("SetBallColor");
				StartCoroutine(SetBallColor(true));
				PowerModeParticleSystem.Play();
			}
			else
			{
				PowerMode(false);
			}
		}
	}

	void NewTurnUpdateBallCountLabel()
	{
		_strBuilder.Remove(0, _strBuilder.Length);
		_strBuilder.AppendFormat("x{0}", OriginBallCount);
		_textMesh_BallCount.text = _strBuilder.ToString();
	}

	void UpdateBallCountLabel()
	{
		_strBuilder.Remove(0, _strBuilder.Length);
		var ttt = _ball_ReadyToShoot_List.Sum(v => v._power);
		_strBuilder.AppendFormat("x{0}", ttt);
		_textMesh_BallCount.text = _strBuilder.ToString();

		//_strBuilder.Remove (0, _strBuilder.Length);
		//_strBuilder.AppendFormat ("x{0}", (_ball_ReadyToShoot_List.Count + _ball_Spare_List.Count));
		//_textMesh_BallCount.text = _strBuilder.ToString ();
	}

	void UpdateBallCountPosition()
	{
		float x = _turnStartPosition.x;
		float leftMax = -0.5f * InGameController._CurrentStageWidth + _textMesh_BallCount.GetComponent<Renderer>().bounds.size.x * 0.5f;
		float rightMax = 0.5f * InGameController._CurrentStageWidth - _textMesh_BallCount.GetComponent<Renderer>().bounds.size.x * 0.5f;
		x = Mathf.Clamp (x, leftMax, rightMax);

		if((float)Screen.height / (float)Screen.width < 3.5f/2.5f)
			_textMesh_BallCount.transform.localPosition = Vector3.right * x + Vector3.down * 0.25f;
		else
			_textMesh_BallCount.transform.localPosition = Vector3.right * x + Vector3.down * 0.5f;
	}

	public void SetStageWidth(float stageWidth)
	{
		//float x = -(_tr_WallUp.localScale.x * 0.25f);
		//float y = (stageWidth + _tr_WallUp.transform.localScale.y) * 0.5f;
		//float z = 0f;
		//_textMesh_Turn.transform.localPosition = new Vector3(x, y, z);
		//x = (_tr_WallUp.localScale.x * 0.25f);
		//TextMesh_PowerModeTurn.transform.localPosition = new Vector3(x, y, z);

		// 기존 로직
		//_textMesh_Turn.transform.localPosition = Vector3.up * (stageWidth + _tr_WallUp.transform.localScale.y) * 0.5f;
	}

	void SetSkipable(bool b)
	{
		_ableToSkip = b;
		if(_textMesh_Skip.gameObject.activeSelf != b)
			_textMesh_Skip.gameObject.SetActive(b);
	}

	public void TryToSkip(Vector3 position)
	{
		if(_ableToSkip == false)
			return;

		//if((float)Screen.height / (float)Screen.width < 3.5f/2.5f)
		//{
		//	if (InGameController._BrickHeight <= position.y && position.y <= InGameController._BrickHeight * 2)
		//		Skip ();
		//}
		//else
		//{
		//	if (position.y <= 0)
		//		Skip ();
		//}
	}
    //1231 5분 스킵 기능체크
    //buttonSkip 버튼 활성화 비활성화시 박스 컬라이더도 같이 활성/비활성 하게끔 바꿔주게 해둠

    //public void SkipGamePlay()
    //{
    //    if(_activeSkipButton)
    //        Skip();
    //}

//	void Skip()
//	{
//        //		Debug.LogWarning ("Skip");
//        buttonSkip.SetActive(false);
//        buttonSkip.GetComponent<BoxCollider>().enabled = false;
//        _activeSkipButton = false;
//        _skipButtonTime = 0.0f;

//        SetSkipable(false);
//		_thread_Skip = Thread_SkipBall(0, 0.4f);
////		_thread_Skip = Thread_SkipBall(0, 2.0f);
//	}

	void Skip_AllClear()
	{
		SetSkipable(false);
		_thread_Skip = Thread_SkipBall(1f, 1f);
	}

	IEnumerator Thread_SkipBall(float startDelay, float duration)
	{
		bool isPowermode = PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 1;
		Color startBallColor = isPowermode ? Static_ColorConfigs._Color_PowerBall : Static_ColorConfigs._Color_Ball;
		Color endBallColor = startBallColor;
		endBallColor.a = 0;

		Color startShadowColor = Static_ColorConfigs._Color_Ball_Shadow;
		Color endShadowColor = startShadowColor;
		endShadowColor.a = 0;

		float elapsedTime = 0f;
		float lerpRatio = 0;

		while(elapsedTime < duration + startDelay)
		{
			lerpRatio = (elapsedTime-startDelay) / duration;

			//MaterialHolder._Material_PlayerBall.color = Color.Lerp(startBallColor, endBallColor, lerpRatio);
			//MaterialHolder._Material_BallShadow.color = Color.Lerp(startShadowColor, endShadowColor, lerpRatio);

			//for(int i = 0; i < _ball_List.Count; i++)
			//{
			//	_ball_List[i]._trail.material.color = Color.Lerp(startBallColor, endBallColor, lerpRatio);
			//	_ball_List[i]._trailShadow.material.color = Color.Lerp(startBallColor, endBallColor, lerpRatio);
			//}
			for(int i=0; i < _ball_OnMove_List.Count; i++)
			{
				_ball_OnMove_List[i]._renderer_Model.material.color = Color.Lerp(startBallColor, endBallColor, lerpRatio);
				_ball_OnMove_List[i]._trail.material.color = Color.Lerp(startBallColor, endBallColor, lerpRatio);
				_ball_OnMove_List[i]._trailShadow.material.color = Color.Lerp(startBallColor, endBallColor, lerpRatio);
			}

			yield return 0;
			elapsedTime += Time.deltaTime;

			if (_ball_OnMove_List.Count <= 0)
				break;
		}

		//MaterialHolder._Material_PlayerBall.color = ColorConfigs._Color_Ball;
		//MaterialHolder._Material_BallShadow.color = ColorConfigs._Color_Ball_Shadow;

		_transform_StartBall.gameObject.SetActive (false);

		_thread_Move_List.Clear ();
		_thread_GatherBall_List.Clear ();

		_ball_ReadyToShoot_List.Clear ();
		_ball_OnMove_List.Clear ();
		_ball_OnGather_List.Clear ();
		_ball_Spare_List.Clear ();

		for (int i = 0; i < _ball_List.Count; i++)
		{
			_ball_List [i].Sleep ();
			_ball_List [i].gameObject.SetActive(false);
			//_ball_List [i]._trail.material = MaterialHolder._Material_BallTrail;
			//_ball_List [i]._trailShadow.material = MaterialHolder._Material_BallTrailShadow;
		}
	}

	///////////////////////////////////////////////////////////////////////
	//  public void SetBallSpeed(int multiple, bool isAdMob = false)
	//  {
	//      _ballSpeed = multiple;
	//      PlayerPrefs.SetInt("LastBallSpeed", _ballSpeed);
	//var isSpeedPurchase = PlayerPrefs.GetInt("SpeedPurchase", 0) == 0 ? false : true;
	//var isAdSpeedPurchase = PlayerPrefs.GetInt("AdSpeedPurchase", 0) == 0 ? false : true;

	//if (isSpeedPurchase || isAdSpeedPurchase || SmartShopController.reward_Activated)
	//          _adRewardSpeedTurn = PurchaseSpeedTurn;
	//      else
	//      {
	//	if (AdController.AdReward && _adRewardSpeedTurn == 0)
	//          {
	//              _adRewardSpeedTurn = AdRewardSpeedTurn;
	//		PlayerPrefs.SetInt("AdSpeedTurn", _adRewardSpeedTurn);
	//	}
	//      }

	//      if (isAdMob)
	//          Invoke("SetAdBallSpeed", 0.1f);
	//      else
	//      {
	//          var menuMultiple = FindObjectOfType<MenuButton_Multiple>();
	//          if (menuMultiple != null)
	//          {
	//              menuMultiple.SetTextMultiple(_ballSpeed == 1 ? TextConfigs._1x : TextConfigs._2x);

	//		if (AdController.AdReward)
	//              {
	//                  _textMesh_Turn.text = _adRewardSpeedTurn.ToString();
	//                  _textMesh_Turn.gameObject.SetActive(true);
	//              }
	//              else
	//              {
	//                  _textMesh_Turn.text = _adRewardSpeedTurn.ToString();
	//                  _textMesh_Turn.gameObject.SetActive(false);
	//              }

	//              if (_adRewardSpeedTurn == PurchaseSpeedTurn)
	//              {
	//                  _textMesh_Turn.text = _adRewardSpeedTurn.ToString();
	//                  _textMesh_Turn.gameObject.SetActive(false);
	//              }
	//          }
	//      }
	//  }
	///////////////////////////////////////////////////////////////////////

	///////////////////////////////////////////////////////////////////////
	//public void SetAdBallSpeed()
	//   {
	//       _adRewardSpeedTurn = AdRewardSpeedTurn;
	//       PlayerPrefs.SetInt("AdReward", 1);
	//       PlayerPrefs.SetInt("AdSpeedTurn", _adRewardSpeedTurn);
	//       var menuMultiple = FindObjectOfType<MenuButton_Multiple>();
	//       if (menuMultiple != null)
	//       {
	//           menuMultiple.SetTextMultiple(_ballSpeed == 1 ? TextConfigs._1x : TextConfigs._2x);
	//           _textMesh_Turn.text = _adRewardSpeedTurn.ToString();
	//           _textMesh_Turn.gameObject.SetActive(true);
	//       }
	//   }
	///////////////////////////////////////////////////////////////////////

	public void SetNewTurn()
	{
		_returnBallCount = 0;
		_returnedEmptyBall = false;
		_existHitBall = false;
//		_isEveryBallUnderBrick = false;
		_ableToSkip = false;

		_ball_ReadyToShoot_List.Clear ();
		_ball_OnMove_List.Clear ();
		_ball_OnGather_List.Clear ();
		_ball_TurnEnded_List.Clear ();
		_ball_Spare_List.Clear ();

		_turnStartPosition = _turnEndPosition;

		for(int i = 0; i < _ball_List.Count; i++)
		{
			_ball_List[i].OffTrail();
			_ball_List[i]._collider.enabled = false;
			_ball_List[i]._transform.position = _turnStartPosition;
			_ball_ReadyToShoot_List.Add(_ball_List[i]);
			_ball_List[i].gameObject.SetActive(false);
		}

		SetSkipable(false);

		NewTurnUpdateBallCountLabel();
		//UpdateBallCountLabel ();

		_textMesh_BallCount.gameObject.SetActive (true);
		UpdateBallCountPosition ();

		_transform_StartBall.position = _turnStartPosition;
		_transform_StartBall.gameObject.SetActive (true);
		_transform_EndBall.gameObject.SetActive (false);

		_thread_GatherBall_List.Clear ();
		_thread_AddBall = null;
		_thread_CheckTurnEnd = null;

		AdRewardSpeedModeTurn = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, 0);
		AdRewardPowerModeTurn = PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, 0);

		// 비구매 유저만 카운팅 한다.
		if (StaticMethod.IsPurchase_Speed_Total_SmartShopReward() == false)
		{
            // speedMode
            if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActiveSpeedMode, 1) == 1)
            {
                if (AdRewardSpeedModeTurn <= 0)
                {
                    SpeedMode(false);
                    PopUpController.AddAlert(AlertPopUpType.SpeedMode_Guide);
                }
                else
                {
                    AdRewardSpeedModeTurn--;
                    SpeedMode(true);
                }
            }
            else
            {
                SpeedMode(false);
            }

            // powerMode
            if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 1)
			{
				if (AdRewardPowerModeTurn <= 0)
				{
					PowerMode(false);
					PopUpController.AddAlert(AlertPopUpType.PowerMode_Guide);
				}
				else
				{
					AdRewardPowerModeTurn--;
					PowerMode(true);
				}
			}
			else
			{
				PowerMode(false);
			}

		}

		//////////////////////////////////////////////////////////////////////////
		//var menuMultiple = GameObject.FindObjectOfType<MenuButton_Multiple>();
		//
		//if (_adRewardSpeedTurn == PurchaseSpeedTurn)
		//{
		//    _textMesh_Turn.text = _adRewardSpeedTurn.ToString();
		//    _textMesh_Turn.gameObject.SetActive(false);
		//}
		//
		//if (AdController.AdReward)
		//{
		//	if (_adRewardSpeedTurn == PurchaseSpeedTurn)
		//		return;

		//	if (_ballSpeed == 2)
		//	{
		//		_adRewardSpeedTurn--;
		//		PlayerPrefs.SetInt("AdSpeedTurn", _adRewardSpeedTurn);
		//		PlayerPrefs.SetInt("AdReward", 1);
		//	}

		//	if (menuMultiple != null)
		//	{
		//		_textMesh_Turn.text = _adRewardSpeedTurn.ToString();
		//		_textMesh_Turn.gameObject.SetActive(true);
		//	}

		//	if (_adRewardSpeedTurn == 0)
		//	{
		//		AdController.AdReward = false;
		//		_ballSpeed = 1;
		//		PlayerPrefs.SetInt("LastBallSpeed", _ballSpeed);
		//		PlayerPrefs.SetInt("AdSpeedTurn", _adRewardSpeedTurn);
		//		PlayerPrefs.SetInt("AdReward", 0);

		//		if (menuMultiple != null)
		//		{
		//			menuMultiple.SetTextMultiple(TextConfigs._1x);
		//			_textMesh_Turn.text = _adRewardSpeedTurn.ToString();
		//			_textMesh_Turn.gameObject.SetActive(false);
		//		}
		//	}
		//}

		//////////////////////////////////////////////////////////////////////////
	}

	public void SpeedMode(bool isActive = true)
	{
		if (_speedModeComponent == null)
			_speedModeComponent = FindObjectOfType<MenuButton_Multiple>();

		_ballSpeed = isActive ? SpeedMode_Config.ActiveValue : SpeedMode_Config.InactiveValue;
		//if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActiveSpeedMode, 1) == 0)
		//	_ballSpeed = SpeedMode_Config.InactiveValue;
		//else if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActiveSpeedMode, 1) == 1)
		//	_ballSpeed = SpeedMode_Config.ActiveValue;
		//else
		//	_ballSpeed = SpeedMode_Config.ActiveValue_4;

		//if (_ballSpeed != 1)
		if (isActive)
		{
			PlayerPrefs.SetInt(PlayerPrefs_Config.ActiveSpeedMode, 1);
			PlayerPrefs.SetInt(PlayerPrefs_Config.AdRewardSpeedModeTurn, AdRewardSpeedModeTurn);

			if (_speedModeComponent)
				_speedModeComponent.SetMode(true);
		}
		else
		{
			PlayerPrefs.SetInt(PlayerPrefs_Config.ActiveSpeedMode, 0);

			if (_speedModeComponent)
			{
				_speedModeComponent.SetMode(false);
			}
		}
	}

	public void PowerMode(bool isActive = true)
	{
		if (_powerModeComponent == null)
			_powerModeComponent = FindObjectOfType<MenuButton_PowerMode>();

		if (isActive)
		{
			if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 0)
			{
				StopCoroutine("SetBallColor");
				StartCoroutine(SetBallColor(true));
				PowerModeParticleSystem.Play();
			}

			PlayerPrefs.SetInt(PlayerPrefs_Config.ActivePowerMode, 1);
			PlayerPrefs.SetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, AdRewardPowerModeTurn);

			if (_powerModeComponent)
				_powerModeComponent.SetMode(true);
		}
		else
		{
			PlayerPrefs.SetInt(PlayerPrefs_Config.ActivePowerMode, 0);

			if (_powerModeComponent)
				_powerModeComponent.SetMode(false);
		}
	}

	IEnumerator SetBallColor(bool isPowermode)
	{
		float durationTime = 1.6f;
		float elapsedTime = 0f;
		float lerpRatio = 0f;

		Color start = Color.white;
		Color end = isPowermode ? Static_ColorConfigs._Color_PowerBall : Static_ColorConfigs._Color_Ball;

		while(elapsedTime < durationTime)
		{
			lerpRatio = elapsedTime / durationTime;

			_renderer_StartBall.material.color = Color.Lerp(start, end, lerpRatio);
			_renderer_EndBall.material.color = Color.Lerp(start, end, lerpRatio);
			_textMesh_BallAdd.color = Color.Lerp(start, end, lerpRatio);
			_textMesh_BallCount.color = Color.Lerp(start, end, lerpRatio);

			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		_renderer_StartBall.material.color = end;
		_renderer_EndBall.material.color = end;
		_textMesh_BallAdd.color = end;
		_textMesh_BallCount.color = end;
	}

	public void AddBall(System.Action endCallback)
	{
		_thread_PickUpDrop_List.Clear ();
		_thread_AddBall = Thread_AddBall (endCallback);
	}

	IEnumerator Thread_AddBall(System.Action endCallback)
	{
		bool isPowermode = PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 1;

		if(_pickUpBall_List.Count > 0)
		{
			Vector3[] startPosition_List = new Vector3[_pickUpBall_List.Count];

			for(int i = 0; i < _pickUpBall_List.Count; i++)
				startPosition_List[i] = _pickUpBall_List[i]._transform.localPosition;

			float elapsedTime = 0f;
			float duration = 0.3f;
			float lerpRatio = 0f;

			Color startColor = Static_ColorConfigs._Color_PickUp;
			Color endColor = isPowermode ? Static_ColorConfigs._Color_PowerBall : Static_ColorConfigs._Color_Ball;
			Color startShadowColor = Static_ColorConfigs._Color_PickUp_Shadow;
			Color endShadowColor = Static_ColorConfigs._Color_Ball_Shadow;

			while(elapsedTime < duration)
			{
				lerpRatio = elapsedTime / duration;
				for(int i = 0; i < _pickUpBall_List.Count; i++)
				{
					_pickUpBall_List[i]._transform.localPosition = Vector3.Lerp(startPosition_List[i], _turnStartPosition, lerpRatio);
					_pickUpBall_List[i]._renderer_Model.material.color = Color.Lerp(startColor, endColor, lerpRatio);
					_pickUpBall_List[i]._renderer_Shadow.material.color = Color.Lerp(startShadowColor, endShadowColor, lerpRatio);
				}

				yield return 0;
				elapsedTime += Time.deltaTime;
			}

			for(int i = 0; i < _pickUpBall_List.Count; i++)
			{
				_pickUpBall_List[i]._transform.localPosition = _turnStartPosition;
				_pickUpBall_List[i]._transform_Model.localPosition = Vector3.back * 0.1f;
				_pickUpBall_List[i]._renderer_Model.material = MaterialHolder._Material_PlayerBall;
				_pickUpBall_List[i]._renderer_Shadow.material = MaterialHolder._Material_BallShadow;
				_ball_List.Add(_pickUpBall_List[i]);
				_ball_ReadyToShoot_List.Add(_pickUpBall_List[i]);
				OriginBallCount++;
			}

			NewTurnUpdateBallCountLabel();
			//UpdateBallCountLabel ();

			SoundController.Play_Success(1);

			_strBuilder.Remove (0, _strBuilder.Length);
			_strBuilder.Append ("+");
			_strBuilder.Append (_pickUpBall_List.Count);
			_textMesh_BallAdd.text = _strBuilder.ToString ();



			AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;
			AnimationCurve curveAccel = CurveHolder._AcceleratedRising;

			Vector3 startPosition = _turnStartPosition + Vector3.up * InGameController._BallRadius*2 + Vector3.back * 0.6f;
			Vector3 endPosition = startPosition + Vector3.up * 3;

			startColor = isPowermode ? Static_ColorConfigs._Color_PowerBall : Static_ColorConfigs._Color_Ball;
			endColor = startColor;
			endColor.a = 0;

			_transform_StartBall.localPosition = _turnStartPosition;
			_textMesh_BallAdd.transform.localPosition = startPosition;
			_textMesh_BallAdd.color = startColor;

			_textMesh_BallAdd.gameObject.SetActive (true);

			elapsedTime = 0f;
			duration = 0.5f;

			while(elapsedTime < duration)
			{
				lerpRatio = elapsedTime / duration;
				_textMesh_BallAdd.transform.localPosition = Vector3.Lerp(startPosition, endPosition, curveDeAccel.Evaluate(lerpRatio));
				_textMesh_BallAdd.color = Color.Lerp(startColor, endColor, curveAccel.Evaluate(lerpRatio));

				yield return 0;
				elapsedTime += Time.deltaTime;
			}

			_textMesh_BallAdd.transform.localPosition = endPosition;
			_textMesh_BallAdd.color = endColor;

			_textMesh_BallAdd.gameObject.SetActive (false);
		}

		for (int i = 0; i < _pickUpBall_List.Count; i++)
			_pickUpBall_List [i].gameObject.SetActive (false);

		_pickUpBall_List.Clear ();

		while(_thread_Move_List.Count > 0 || _thread_GatherBall_List.Count > 0)
			yield return 0;

		if(endCallback != null)
			endCallback();
	}

	public void Shoot(Vector3 dir, System.Action turnEndCallback)
	{
		_callback_TurnEnd = turnEndCallback;

		int shootCount = 0;
		int powerUpCount = 0;
		int defaultPower = 1;

		int frameGap = _ballSpeed == SpeedMode_Config.ActiveValue ? SpeedMode_Config.ActiveFrameGap : SpeedMode_Config.InactiveFrameGap;
		//int frameGap = SpeedMode_Config.ActiveValue;
		//if (_ballSpeed == SpeedMode_Config.ActiveValue)
		//	frameGap = SpeedMode_Config.ActiveFrameGap;
		//else if (_ballSpeed == SpeedMode_Config.ActiveValue_4)
		//	frameGap = SpeedMode_Config.ActiveFrameGap_4;
		//else
		//	frameGap = SpeedMode_Config.InactiveFrameGap;

		int totalCount = OriginBallCount;

		// powerMode
		if (PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) != 0 &&
			(StaticMethod.IsPurchase_Speed_Total_SmartShopReward() || PlayerPrefs.GetInt(PlayerPrefs_Config.AdRewardPowerModeTurn, 0) >= 0))
		{
			shootCount = totalCount;

			if (totalCount >= PowerMode_Config.PowerModeMinPowerBallCount)
			{
				if (totalCount <= Ball_Config.BallMaximum)
				{
					shootCount /= 2;
				}
				else
				{
					shootCount = PowerMode_Config.PowerModeMaxPowerBallCount;
				}

				//var percentage = Mathf.Min(totalCount / MAX_ADDITIONAL_POWERBALL_COUNT, 1f);
				//var addBallCount = (int)((MAX_ADDITIONAL_POWERBALL_COUNT - MIN_ADDITIONAL_POWERBALL_COUNT) * percentage);
				//shootCount = Mathf.Max(MIN_ADDITIONAL_POWERBALL_COUNT + addBallCount, 0);
				//if(totalCount <= originHP)
				//{
				//	shootCount /= 2;
				//}
				defaultPower = totalCount / shootCount;
				powerUpCount = totalCount % shootCount;
			}
		}
		else
		{
			int defaultBall = Ball_Config.BallDefault;
			float originHP = Ball_Config.BallMaximum;
			float defaultEXP = defaultBall - originHP / 100;
		
			float exp = Mathf.Max(totalCount - defaultEXP, 0);
			float reduceRatio = 1 - originHP / (originHP + exp);
			int additionalPower = (int)(totalCount * reduceRatio);
			shootCount = totalCount - additionalPower;
			defaultPower = (int)(additionalPower / shootCount) + 1;
			powerUpCount = additionalPower % shootCount;
		}

		bool isPowermode = PlayerPrefs.GetInt(PlayerPrefs_Config.ActivePowerMode, 0) == 1;
		for (int i = 0; i < _ball_List.Count; i++)
		{
			_ball_List[i]._renderer_Model.material.color = isPowermode ? Static_ColorConfigs._Color_PowerBall : Static_ColorConfigs._Color_Ball;
			_ball_List[i]._trail.material.color = isPowermode ? Static_ColorConfigs._Color_PowerBall : Static_ColorConfigs._Color_Ball;

			_ball_List[i]._collider.enabled = true;

			if(i < shootCount)
			{
				_ball_List[i]._frameDelay = frameGap * i;
				
				if(i < powerUpCount)
					_ball_List[i]._power = defaultPower+1;
				else
					_ball_List[i]._power = defaultPower;
		
				_thread_Move_List.Add(Thread_Move (dir, _ball_List[i], null));
			}
			else
			{
				_ball_ReadyToShoot_List.Remove(_ball_List[i]);
				_ball_Spare_List.Add(_ball_List[i]);
				_ball_List[i].gameObject.SetActive(false);
			}
		}

		_thread_AddBall = null;
		_thread_CheckTurnEnd = Thread_CheckTurnEnd ();
	}

#if UNITY_EDITOR
	System.Diagnostics.Stopwatch StopWatch = new System.Diagnostics.Stopwatch();
#endif

	IEnumerator Thread_CheckTurnEnd()
	{
#if UNITY_EDITOR
		StopWatch.Reset();
		StopWatch.Start();
#endif

		while (_ball_ReadyToShoot_List.Count > 0)
		{
			if(_thread_Skip == null && _returnedEmptyBall)
				Skip_AllClear();

			yield return 0;
		}

		_transform_StartBall.gameObject.SetActive(false);
		_textMesh_BallCount.gameObject.SetActive(false);

		while (_ball_OnMove_List.Count > 0)
		{
			if(_thread_Skip == null && _returnedEmptyBall)
				Skip_AllClear();

			yield return 0;
		}

		while (_ball_OnGather_List.Count > 0 || _thread_PickUpDrop_List.Count > 0)
			yield return 0;

		List<BallPickUp> pickUp_List = BrickGenerator.GetBottomPickUp_List ();
		for(int i = 0; i < pickUp_List.Count; i++)
			AddPickUpBall(pickUp_List[i], pickUp_List[i]._transform.position);

		while (_thread_Skip != null)
			yield return 0;

        _skipButtonTime = 0.0f;

        if (_callback_TurnEnd != null)
			_callback_TurnEnd();

#if UNITY_EDITOR
		StopWatch.Stop();
		Debug.LogFormat("StopWatch: {0}", StopWatch.ElapsedMilliseconds / 1000f);
#endif
	}

	IEnumerator Thread_Move(Vector3 shootDir, Ball ball, System.Action shootStartCallback = null)
	{
		int currentMoveFrame = 0;

		while(currentMoveFrame < ball._frameDelay)//1212 체크
		{
			yield return 0;
			currentMoveFrame++;
		}

		if(shootStartCallback != null)
			shootStartCallback();

		ball.gameObject.SetActive (true);

		ball.Shoot (shootDir.normalized * (_speedPerFrame * _ballSpeed) * 60f, Callback_Trigger);
		yield return 0;
		currentMoveFrame++;
		_ball_ReadyToShoot_List.Remove (ball);
		_ball_OnMove_List.Add (ball);

		_ball_Spare_List.Remove(ball);

		//for (int i = 1; i < ball._power; i++)
		//	_ball_Spare_List.RemoveAt (0);

		UpdateBallCountLabel ();

		while(_ball_OnMove_List.Contains(ball))
		{
			Vector2 velocity = ball._rigidbody.velocity;

			float angleLimit = 10f;
			float angle = (Static_Calculator.XYMeter2Angle(velocity) + 360f) % 360f;
			if(0f <= angle && angle < angleLimit)
				angle = angleLimit;
			else if(180f - angleLimit < angle && angle <= 180f)
				angle = 180f - angleLimit;
			else if(180f < angle && angle < 180f + angleLimit)
				angle = 180f + angleLimit;
			else if(360f - angleLimit <= angle && angle < 360f)
				angle = 360f - angleLimit;

			//Vector3 newDir = Calculator.Vector2XYForce(angle, 1);
			//Vector3 nextVelocity = newDir * (_speedPerFrame * _ballSpeed);
			//Vector3 currentPosition = ball._transform.localPosition;
			//Vector3 nextPosition = currentPosition + newDir * (_speedPerFrame * _ballSpeed);

			Vector3 newDir = Static_Calculator.Vector2XYForce(angle, _ballSpeed);
			Vector3 nextVelocity = newDir * _speedPerFrame;
			Vector3 currentPosition = ball._transform.localPosition;
			Vector3 nextPosition = currentPosition + newDir * _speedPerFrame;
			
			if(nextPosition.y <= _bottomLine)
			{
				if(ball._isHitted == false)
				{
					if(_returnedEmptyBall == false && _ball_OnMove_List.Any(v => v._isHitted) == false)
						_returnedEmptyBall = true;
				}

				ball.Sleep();

				float yError = nextPosition.y - _bottomLine;
				float xError = (nextVelocity.x / nextVelocity.y) * yError;
				ball._transform.localPosition = Vector3.right * (nextPosition.x - xError) + Vector3.up * (nextPosition.y - yError);

				if(_returnBallCount == 0)
				{
					_turnEndPosition = ball._transform.localPosition;
					_turnEndPosition.x = Mathf.Clamp(_turnEndPosition.x, -0.5f * InGameController._CurrentStageWidth + InGameController._BallRadius, 0.5f * InGameController._CurrentStageWidth - InGameController._BallRadius);
					_transform_EndBall.localPosition = _turnEndPosition;

					_transform_EndBall.gameObject.SetActive(true);
				}

				_returnBallCount += ball._power;

				ball._onDirect = true;
				_ball_OnMove_List.Remove(ball);
				_ball_OnGather_List.Add(ball);

				_thread_GatherBall_List.Add(Thread_MoveToEndPosition(ball));
			}
			else
			{
				ball._velocity_Previous = ball._rigidbody.velocity;
				ball._rigidbody.velocity = nextVelocity * 60f;
				if(ball._isHitted)
				{
					if(_existHitBall == false)
						_existHitBall = true;
				}
			}

			yield return 0;
			currentMoveFrame++;
		}
	}

	IEnumerator Thread_MoveToEndPosition(Ball ball)
	{
		while(ball._onDirect)
		{
			Vector3 dir = _turnEndPosition - ball._transform.localPosition;
			float distance = dir.magnitude;
			float speedPerFrame = _speedPerFrame * 2;
			
			if(distance < speedPerFrame)
			{
				ball._transform.localPosition = _turnEndPosition;
				ball._onDirect = false;
			}
			else
			{
				ball._transform.localPosition += dir.normalized * speedPerFrame;
			}
			
			yield return 0;
		}

		_ball_OnGather_List.Remove(ball);
		_ball_TurnEnded_List.Add(ball);

		float elapsedTime = 0;
		float duration = ball._trail.time;
		while(elapsedTime < duration)
		{
			yield return 0;
			elapsedTime += Time.fixedDeltaTime;
		}
		
		ball.gameObject.SetActive (false);
	}

	void Callback_Trigger(Ball ball, Collider2D col)
	{
		BallPickUp pickUp = col.GetComponent<BallPickUp> ();
		if(pickUp != null && pickUp._isExpired == false)
			AddPickUpBall(pickUp, ball._transform.position);
	}

	void AddPickUpBall(BallPickUp pickUp, Vector3 impactPosition)
	{
		pickUp.GetPickUp(impactPosition);
		
		Ball newBall = ObjectPoolHolder.GetObject<Ball>(_transform);
		newBall.Initialize(InGameController._BallRadius);
		newBall._transform.position = pickUp._transform.position;
		newBall._renderer_Model.material = MaterialHolder._Material_BallPickUp;
		newBall._renderer_Shadow.material = MaterialHolder._Material_BallPickUp_Shadow;
		newBall.gameObject.SetActive(true);
		
		_thread_PickUpDrop_List.Add(Director_PickUpBallDrop(newBall));
		_pickUpBall_List.Add(newBall);
	}

	IEnumerator Director_PickUpBallDrop(Ball ball)
	{
		float speed = (_speedPerFrame * _ballSpeed) * 60f;
		float distanceToDrop = ball._transform.localPosition.y - _bottomLine;

		Vector3 startPosition = ball._transform.localPosition;
		Vector3 endPosition = startPosition;
		endPosition.y = _bottomLine;

		float elapsedTime = 0;
		float duration = distanceToDrop / speed;
		float lerpRatio = 0;

		ball._transform_Model.localPosition = Vector3.back * 0.5f;

		while(elapsedTime < duration)
		{
			lerpRatio = elapsedTime / duration;
			ball._transform.localPosition = Vector3.Lerp(startPosition, endPosition, lerpRatio);

			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		ball._transform.localPosition = endPosition;
	}

	public void Direct_GameOver()
	{
        _transform_StartBall.gameObject.SetActive (false);
		_textMesh_BallCount.gameObject.SetActive (false);

		/////////////////////////////////////////////////////////////////
		//PlayerPrefs.SetInt("LastBallSpeed", _ballSpeed);
		//var menuMultiple = GameObject.FindObjectOfType<MenuButton_Multiple>();
		//if (menuMultiple != null)
		//{
		//    menuMultiple.SetTextMultiple(_ballSpeed == 1 ? TextConfigs._1x : TextConfigs._2x);
		//    _textMesh_Turn.text = _adRewardSpeedTurn.ToString();
		//    _textMesh_Turn.gameObject.SetActive(false);
		//}
		/////////////////////////////////////////////////////////////////

		int effectCount = 24;
		float angleGap = 360f / effectCount;

		Vector3 effectScale = Vector3.one * InGameController._BallRadius * 0.4f;

		Vector3 impactPosition = Vector3.right * _turnStartPosition.x;

		Material modelMat = new Material (MaterialHolder._Material_BreakEffect);
		Material shadowMat = new Material (MaterialHolder._Material_BreakEffect_Shadow);
	
		modelMat.color = Static_ColorConfigs._Color_Ball;
		shadowMat.color = Static_ColorConfigs._Color_Ball_Shadow;

		List<BrickBreakEffect> list = new List<BrickBreakEffect> ();
		for(int i = 0; i < effectCount; i++)
		{
			BrickBreakEffect effect = ObjectPoolHolder.GetObject<BrickBreakEffect>(_transform);
			effect.Initialize();
			effect.SetScale(effectScale);
			effect.SetMaterial(modelMat, shadowMat);
			effect._transform.position = _turnStartPosition + Static_Calculator.Vector2To3(Static_Calculator.Vector2XYForce(angleGap*i, InGameController._BallRadius * 0.5f));
			effect.gameObject.SetActive(true);
			
			_breakEffect_List.Add(effect);
			list.Add(effect);

			float power = 120f;
			power += Random.Range(-1f, 1f) * 12f;
			float angle = Static_Calculator.XYMeter2Angle(effect._transform.position - impactPosition);// - Vector3.up * InGameController._CurrentStageWidth));
			angle += Random.Range(-1f, 1f) * 15f;
			Vector2 force = Static_Calculator.Vector2XYForce(angle, power);

			effect.AddForce(force, impactPosition);
		}

		_thread_GameOver = Thread_GameOver(list, modelMat, shadowMat);
	}

	IEnumerator Thread_GameOver(List<BrickBreakEffect> list, Material modelMat, Material shadowMat)
	{
		AnimationCurve curveAccel = CurveHolder._AcceleratedRising;
		AnimationCurve curveDeAccel = CurveHolder._DeAcceleratedRising;
		
		float elapsedTime = 0;
		float pickTime = 0.75f;
		float duration = 1.5f;
		float lerpRatio = 0;

		Color startColor = modelMat.color;
		Color pickColor = Static_ColorConfigs.GetColor_From_HCL (Static_ColorConfigs._Hue_Ball, Static_ColorConfigs._Chroma_Brick_Max, Static_ColorConfigs._Luma_Brick_Max);
		Color endColor = Static_ColorConfigs.GetColor_From_HCL (Static_ColorConfigs._Hue_Ball, Static_ColorConfigs._Chroma_Brick_Min, Static_ColorConfigs._Luma_Brick_Min);
		endColor.a = 0;

		Color pickShadowColor = shadowMat.color;
		Color endShadowColor = shadowMat.color;
		endShadowColor.a = 0;

		Vector3 startScale = Vector3.one;
		Vector3 pickScale = Vector3.one * 2f;
		Vector3 endScale = Vector3.one * 0.5f;
		
		while(elapsedTime < duration)
		{
			if(elapsedTime < pickTime)
			{
				lerpRatio = elapsedTime / pickTime;
				modelMat.color = Color.Lerp(startColor, pickColor, curveDeAccel.Evaluate(lerpRatio));
				
				for(int i = 0; i < list.Count; i++)
					list[i]._transform.localScale = Vector3.Lerp(startScale, pickScale, curveDeAccel.Evaluate(lerpRatio));
			}
			else
			{
				lerpRatio = (elapsedTime - pickTime) / (duration - pickTime);
				modelMat.color = Color.Lerp(pickColor, endColor, curveAccel.Evaluate(lerpRatio));
				shadowMat.color = Color.Lerp(pickShadowColor, endShadowColor, curveAccel.Evaluate(lerpRatio));

				for(int i = 0; i < list.Count; i++)
					list[i]._transform.localScale = Vector3.Lerp(pickScale, endScale, curveAccel.Evaluate(lerpRatio));
			}
			
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		
		for(int i = 0; i < list.Count; i++)
		{
			_breakEffect_List.Remove(list[i]);
			ObjectPoolHolder.PutObject<BrickBreakEffect>(list[i].gameObject);
		}
	}

	public void FixedMoveNext()
	{
		_existHitBall = false;
		for(int i = 0; i < _thread_Move_List.Count; i++)
		{
			if(_thread_Move_List[i].MoveNext() == false)
				_thread_Move_Expired_List.Add(_thread_Move_List[i]);
		}

		for(int i = 0; i < _thread_Move_Expired_List.Count; i++)
			_thread_Move_List.Remove(_thread_Move_Expired_List[i]);
		
		_thread_Move_Expired_List.Clear ();

		for(int i = 0; i < _thread_GatherBall_List.Count; i++)
		{
			if(_thread_GatherBall_List[i].MoveNext() == false)
				_thread_GatherBall_Expired_List.Add(_thread_GatherBall_List[i]);
		}
		
		for(int i = 0; i < _thread_GatherBall_Expired_List.Count; i++)
			_thread_GatherBall_List.Remove(_thread_GatherBall_Expired_List[i]);
		
		_thread_GatherBall_Expired_List.Clear ();

		if(_thread_CheckTurnEnd != null)
			_thread_CheckTurnEnd.MoveNext();

		if(IsHandleUserEarnedRewardSpeedCallback)
		{
			IsHandleUserEarnedRewardSpeedCallback = false;
			SpeedMode();
			//////////////////////////////////////////////////
			//SetBallSpeed(2, true);
			//////////////////////////////////////////////////
		}

		if (IsHandleUserEarnedRewardPowerModeCallback)
		{
			IsHandleUserEarnedRewardPowerModeCallback = false;
			PowerMode();
		}
	}

	public void MoveNext()
	{
		if(_thread_Skip != null)
		{
			if(_thread_Skip.MoveNext() == false)
				_thread_Skip = null;
		}

		if(_thread_AddBall != null)
			_thread_AddBall.MoveNext();

		for(int i = 0; i < _thread_PickUpDrop_List.Count; i++)
		{
			if(_thread_PickUpDrop_List[i].MoveNext() == false)
				_thread_PickUpDrop_Expired_List.Add(_thread_PickUpDrop_List[i]);
		}

		for(int i = 0; i < _thread_PickUpDrop_Expired_List.Count; i++)
			_thread_PickUpDrop_List.Remove(_thread_PickUpDrop_Expired_List[i]);

		_thread_PickUpDrop_Expired_List.Clear ();

		if(_thread_GameOver != null)
		{
			if(_thread_GameOver.MoveNext() == false)
				_thread_GameOver = null;
		}
	}
}
