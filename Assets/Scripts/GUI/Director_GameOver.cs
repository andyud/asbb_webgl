using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine.Networking;
using System;

public class Director_GameOver : MonoBehaviour
{
	public UILabel _label_Title;

	public UILabel _label_FinalScore;
	public UILabel _label_IsNewRecord;
	public UITweener _tweener_IsNewRecord;

	public UILabel _label_RankingScore;

	public UILabel _label_Leaderboard;
	public UIBasicSprite _texture_Leaderboard;
	public UIButton _button_Leaderboard;
	public Collider _collider_Leaderboard;

	public UILabel _label_ShareScore;
	public UIBasicSprite _texture_ShareScore_BG;
	public UIButton _button_ShareScore;
	public Collider _collider_ShareScore;

	public UILabel _label_BackToHome;
	public UIBasicSprite _texture_Icon_Home;
	public UIBasicSprite _texture_BackToHome;
	public UIButton _button_BackToHome;
	public Collider _collider_BackToHome;
	
	public UILabel _label_Restart;
	public UIBasicSprite _texture_Icon_Restart;
	public UIBasicSprite _texture_Restart;
	public UIButton _button_Restart;
	public Collider _collider_Rastart;

    public UILabel _label_RestartSkip;
    public UIBasicSprite _texture_AD;
    public UIBasicSprite _texture_RestartSkip;
    public UIButton _button_RestartSkip;
    public Collider _collider_RestartSkip;

	public UIButton _button_Backup;
	public UIBasicSprite _texture_Backup;
	public UILabel _label_Backup;
	public UILabel _label_Backup_UpdateTime;

    public UIBasicSprite _texture_Screen;

	int _finalScore;

	System.Text.StringBuilder _strBuilder = new System.Text.StringBuilder();
	
	public bool _OnDirect { get { return _thread != null ? true : false; } }
	IEnumerator _thread;

    public void Initialize()
	{
		_label_Title.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_FinalScore.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_IsNewRecord.color = Static_ColorConfigs._Color_TextStrongGreen;

		_label_RankingScore.color = Static_ColorConfigs._Color_ButtonFrame;

		_label_ShareScore.color = Static_ColorConfigs._Color_TextWhite;
		_texture_ShareScore_BG.color = Static_ColorConfigs._Color_TextOrange;

		_label_Leaderboard.color = Static_ColorConfigs._Color_TextWhite;
		_texture_Leaderboard.color = Static_ColorConfigs._Color_TextOrange;
		//		_label_BackToHome.color = ColorConfigs._Color_TextWhite;
		_texture_BackToHome.color = Static_ColorConfigs._Color_TextOrange;
		//		_label_Restart.color = ColorConfigs._Color_TextWhite;
		_texture_Restart.color = Static_ColorConfigs._Color_TextOrange;
        _label_Restart.color = Static_ColorConfigs._Color_TextWhite;
        _texture_RestartSkip.color = Static_ColorConfigs._Color_TextOrange;

        _texture_Icon_Home.color = Static_ColorConfigs._Color_TextWhite;
		_texture_Icon_Restart.color = Static_ColorConfigs._Color_TextWhite;		
		
		_label_BackToHome.gameObject.SetActive (false);
		_label_Restart.gameObject.SetActive (false);
		_texture_Icon_Home.gameObject.SetActive (true);
		_texture_Icon_Restart.gameObject.SetActive (true);
		//_texture_AD.gameObject.SetActive(true);

        _texture_Screen.color = Static_ColorConfigs._Color_Background;
		_texture_Screen.alpha = 0;

		_label_Title.text = Static_TextConfigs._GameOver_Title;
		_label_IsNewRecord.text = Static_TextConfigs._NewRecord;
		_label_ShareScore.text = Static_TextConfigs._ShareScore;
		_label_Leaderboard.text = Static_TextConfigs.Btn_Ranking;
        //		_label_Restart.text = TextConfigs._BackToTitle;
        _label_RestartSkip.text = string.Format(Static_TextConfigs._PlaySkip, (UserData._Score_Best / Shooter.SkipGameTurn) * Shooter.SkipGameTurn);

		_texture_Backup.color = Static_ColorConfigs._Color_TextOrange;
		_label_Backup.color = Static_ColorConfigs._Color_TextWhite;
		_label_Backup_UpdateTime.color = Static_ColorConfigs._Color_TextWhite;

		_button_ShareScore.onClick.Clear ();
		_button_ShareScore.onClick.Add (new EventDelegate (ButtonResponse_ShareScore));

		_button_Leaderboard.onClick.Clear();
		_button_Leaderboard.onClick.Add(new EventDelegate(ButtonResponse_Leaderboard));

		_button_BackToHome.onClick.Clear ();
		_button_BackToHome.onClick.Add (new EventDelegate (ButtonResponse_BackToHome));

		_button_Restart.onClick.Clear ();
		_button_Restart.onClick.Add (new EventDelegate (ButtonResponse_Restart));

        _button_RestartSkip.onClick.Clear();
        _button_RestartSkip.onClick.Add(new EventDelegate(ButtonResponse_RestartSkip));

		_button_Backup.onClick.Clear();
		_button_Backup.onClick.Add(new EventDelegate(ButtonResponse_Backup));
    }

	void ButtonResponse_ShareScore()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
		}
		else
		{
			Debug.Log("=====ShareScore");
			NativeShareController.Instance.ShareImage();
			//_strBuilder.Remove(0, _strBuilder.Length);
			//_strBuilder.AppendFormat("{0} {1} {2}", TextConfigs._ShareScore_Message_Front, _finalScore, TextConfigs._ShareScore_Message_Rear);
			//_strBuilder.AppendFormat("{0}{1}", "\n", FN._Market_URL);
			//Share.ShareMessage(_strBuilder.ToString());

			//FirebaseLogController.LogEvent(FirebaseLogType.GameOverSceneBtn, "ShareScore");
		}
	}
	
	void ButtonResponse_Leaderboard()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			PopUpController.AddAlert(AlertPopUpType.Not_Connection_Network);
			return;
		}

		Debug.Log("=====Ranking Web View");
		// WebViewController.instance.ShowRankingView();
	}

	void ButtonResponse_BackToHome()
	{
		Main.BackToHome ();
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.GameOverSceneBtn, "BackToHome");
#endif
	}
	
	void ButtonResponse_Restart()
	{
		Main.Restart ();
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.GameOverSceneBtn, "Restart");
#endif
	}

    void ButtonResponse_RestartSkip()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
			if (StaticMethod.IsPurchase_SkipGame_Total_SmartShopReward())
				Main._main.DelayRestartSkip();
#if UNITY_ANDROID
            else
			{
				AdController.Show_Reward_SkipGame(true);
			}
#endif
        }
        else
            Main._main.DelayRestartSkip();
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.GameOverSceneBtn, "RestartSkip");
#endif
	}

	void ButtonResponse_Backup()
	{
#if UNITY_ANDROID
		if (FireBaseController.isLogin)
            PopUpController.Open_Backup();
		else
#endif
            PopUpController.Open_Login();
#if UNITY_ANDROID
		FirebaseLogController.LogEvent(FirebaseLogType.GameOverSceneBtn, "BackUp");
#endif
	}

    public void Direct(int finalScore, bool isNewRecord, System.Action endCallback)
	{
		_finalScore = finalScore;

		_collider_ShareScore.enabled = false;

		_collider_Leaderboard.enabled = false;
		_collider_BackToHome.enabled = false;
		_collider_Rastart.enabled = false;
        _collider_RestartSkip.enabled = false;

        _strBuilder.Remove (0, _strBuilder.Length);
		_strBuilder.AppendFormat ("{0}{1}", Static_TextConfigs._FinalScore, " : ");
		_strBuilder.Append (finalScore);
		_label_FinalScore.text = _strBuilder.ToString ();
		var startScore = PlayerPrefs.GetInt(PlayerPrefs_Config.StartRankingScore);
		var endScore = PlayerPrefs.GetInt(PlayerPrefs_Config.EndRankingScore);
		var score = endScore - startScore;
		_label_RankingScore.text = string.Format($"{Static_TextConfigs._RankingScore} : {score}");

        _label_RestartSkip.text = string.Format(Static_TextConfigs._PlaySkip, (UserData._Score_Best / Shooter.SkipGameTurn) * Shooter.SkipGameTurn);
        _texture_Screen.gameObject.SetActive (false);
		_label_Title.gameObject.SetActive (false);

		_label_FinalScore.gameObject.SetActive (false);
		_label_IsNewRecord.gameObject.SetActive (false);
		_label_RankingScore.gameObject.SetActive(false);
		_button_ShareScore.gameObject.SetActive (false);
		_button_Leaderboard.gameObject.SetActive (false);
		_button_BackToHome.gameObject.SetActive (false);
		_button_Restart.gameObject.SetActive (false);
        _button_RestartSkip.gameObject.SetActive(false);
		_button_Backup.gameObject.SetActive(false);
#if UNITY_ANDROID
		if(FireBaseController.isLogin && isNewRecord && AppServerController.Instance.IsNewDBInit)
		{
			AutoBackup();
		}
#endif
		_thread = Direct_GameOver (finalScore, isNewRecord, endCallback);
	}
	
	IEnumerator Direct_GameOver (float score, bool isNewRecord, System.Action endCallback)
	{
		// Direct Title

		float elapsedTime = 0;
		float duration = 0.5f;
		AnimationCurve curve = CurveHolder._AcceleratedRising;
		_label_Title.gameObject.SetActive (true);
		_label_Title.alpha = 0;
		_texture_Screen.gameObject.SetActive (true);
		_texture_Screen.alpha = 0;
		float endAlpha = 0.9f;
		while(elapsedTime < duration)
		{
			float lerpRatio = elapsedTime / (duration * 0.5f);
			_label_Title.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_texture_Screen.alpha = Mathf.Lerp(0, endAlpha, curve.Evaluate(lerpRatio));
			
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		_label_Title.alpha = 1;
		_texture_Screen.alpha = endAlpha;		
		
		elapsedTime = 0;
		duration = 0.2f;
		while(elapsedTime < duration)
		{
			yield return 0;
			elapsedTime += RealTime.deltaTime;
		}

		elapsedTime = 0;
		duration = 0.5f;
		_label_FinalScore.gameObject.SetActive (true);
		_label_FinalScore.alpha = 0;
		_label_RankingScore.gameObject.SetActive(true);
		_label_RankingScore.alpha = 0;

		while (elapsedTime < duration)
		{
			float lerpRatio = elapsedTime / (duration * 0.5f);
			_label_FinalScore.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_label_RankingScore.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));

			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		_label_FinalScore.alpha = 1;
		_label_RankingScore.alpha = 1;
		
		if(isNewRecord)
		{
			elapsedTime = 0;
			duration = 0.5f;
			
			_label_IsNewRecord.gameObject.SetActive(true);
			_tweener_IsNewRecord.ResetToBeginning();
			_tweener_IsNewRecord.PlayForward();
			while(elapsedTime < duration)
			{
				float lerpRatio = elapsedTime / (duration * 0.5f);
				_label_IsNewRecord.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
				
				yield return 0;
				elapsedTime += Time.deltaTime;
			}
			_label_IsNewRecord.alpha = 1;
			
			elapsedTime = 0;
			duration = 0.2f;
			while(elapsedTime < duration)
			{
				yield return 0;
				elapsedTime += RealTime.deltaTime;
			}
		}

		if(endCallback != null)
			endCallback();

		elapsedTime = 0;
		duration = 0.2f;
		while(elapsedTime < duration)
		{
			yield return 0;
			elapsedTime += RealTime.deltaTime;
		}

		elapsedTime = 0;
		duration = 0.5f;

		_button_ShareScore.gameObject.SetActive(true);
		_collider_ShareScore.enabled = true;
		_label_ShareScore.alpha = 0;
		_texture_ShareScore_BG.alpha = 0;

		_button_Leaderboard.gameObject.SetActive(true);
		_collider_Leaderboard.enabled = true;
		_label_Leaderboard.alpha = 0;
		_texture_Leaderboard.alpha = 0;
		_texture_Backup.alpha = 0;

		while(elapsedTime < duration)
		{
			float lerpRatio = elapsedTime / (duration * 0.5f);
			_label_Leaderboard.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_texture_Leaderboard.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_texture_Backup.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_label_ShareScore.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_texture_ShareScore_BG.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));

			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		_label_Leaderboard.alpha = 1;
		_texture_Leaderboard.alpha = 1;
		_texture_Backup.alpha = 1;
		_label_ShareScore.alpha = 1;
		_texture_ShareScore_BG.alpha = 1;

		elapsedTime = 0;
		duration = 0.2f;
		while(elapsedTime < duration)
		{
			yield return 0;
			elapsedTime += Time.deltaTime;
		}
		
		elapsedTime = 0;
		duration = 0.5f;
		_button_BackToHome.gameObject.SetActive (true);
        _button_Restart.gameObject.SetActive(true);
		_button_Backup.gameObject.SetActive(true);

		// 이어하기 버튼 표시 체크
		if ((UserData._Score_Best / Shooter.SkipGameTurn) * Shooter.SkipGameTurn > 0)
		{
			_button_RestartSkip.gameObject.SetActive(true);
			var pos = _button_RestartSkip.transform.localPosition;
			var pos2 = _button_Restart.transform.localPosition;
			var y = pos.y - (Mathf.Abs(pos.y) - Mathf.Abs(pos2.y));
			var localPos = new Vector3(0f, y, 0f);

			_button_Backup.transform.localPosition = localPos;
		}
		else
		{
			_button_RestartSkip.gameObject.SetActive(false);
			_button_Backup.transform.localPosition = _button_RestartSkip.transform.localPosition;
		}

		// 이어하기 아이콘 표시 체크
		if(StaticMethod.IsPurchase_SkipGame_Total_SmartShopReward())
			_texture_AD.gameObject.SetActive(false);
		else
			_texture_AD.gameObject.SetActive(true);

		_collider_BackToHome.enabled = true;
		_collider_Rastart.enabled = true;
        _collider_RestartSkip.enabled = true;

        while (elapsedTime < duration)
		{
			float lerpRatio = elapsedTime / (duration * 0.5f);
            _label_RestartSkip.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
            _texture_Icon_Home.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_texture_Icon_Restart.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_texture_AD.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
            _texture_BackToHome.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
			_texture_Restart.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));
            _texture_RestartSkip.alpha = Mathf.Lerp(0, 1, curve.Evaluate(lerpRatio));

            yield return 0;
			elapsedTime += Time.deltaTime;
		}

        _label_RestartSkip.alpha = 1;
        _texture_Icon_Home.alpha = 1;
		_texture_Icon_Restart.alpha = 1;
		_texture_AD.alpha = 1;
        _texture_BackToHome.alpha = 1;
		_texture_Restart.alpha = 1;
        _texture_RestartSkip.alpha = 1;
    }
	
	private void AutoBackup()
	{
		AppServerController.Instance.CheckNewDBUser(v =>
		{
			// new DB user
			if (v.result && v.data)
			{
				AppServerController.Instance.GetUserBackUpData(vv =>
				{
					if (vv.result)
					{
						NewBackUpData backupData = null;
						backupData = vv.data;

						if (backupData == null)
							backupData = new NewBackUpData();

						if (UserData._Score_Best <= backupData.BestScore)
							UserData._Score_Best = backupData.BestScore;
						else
							backupData.BestScore = UserData._Score_Best;

						backupData.LastBackupDate = DateTime.Now;

						AppServerController.Instance.SetUserBackUpData(backupData, vvv =>
						{
							if (vvv.result)
							{
								PlayerPrefs.SetString(PlayerPrefs_Config.BackupUpdateDateTime, vv.data.LastBackupDate.ToString());

								AppServerController.Instance.ValidationUserReceipt(vvvv =>
								{
									if(vvvv.result && vvvv.data != null)
									{
										AppServerController.Instance.SetIAPItem(vvvv.data);
									}
								});
							}
						});
					}
				});
			}
		});
	}

	public void MoveNext()
	{		
		if(_thread != null)
		{
			if(_thread.MoveNext() == false)
				_thread = null;
		}
#if UNITY_ANDROID
		if(_label_Backup_UpdateTime)
		{
			_label_Backup_UpdateTime.text = FireBaseController.isLogin ?
				PlayerPrefs.GetString(PlayerPrefs_Config.BackupUpdateDateTime, string.Empty) : string.Empty;
		}

		if (FireBaseController.ConfigData != null && FireBaseController.ConfigData.IsBackUpFeature && AppServerController.Instance.IsNewDBInit)
		{
			_button_Backup.gameObject.SetActive(true);
		}
		else
#endif
		{
			_button_Backup.gameObject.SetActive(false);
		}
	}
}
