using System;
using UnityEngine;

public class Login : MonoBehaviour
{
	public UIBasicSprite Texture_Wall;
	public UIBasicSprite Texture_Floor;

	[SerializeField] private UIButton _appleLoginBtn;
	[SerializeField] private UIButton _googleLoginBtn;
	[SerializeField] private UIButton _closeBtn;
	[SerializeField] private UIButton _deleteUserBtn;
	[SerializeField] private UIButton _signOutBtn;

	public void Init(Action closeBtnClick, Action appleLoginBtnClick, Action googleLoginBtnClick)
	{
		Texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		Texture_Floor.color = Static_ColorConfigs._Color_ButtonBase;

		if (_closeBtn)
		{
			_closeBtn.onClick.Clear();
			_closeBtn.onClick.Add(new EventDelegate(() => closeBtnClick?.Invoke()));		}

		if (_appleLoginBtn)
		{
			_appleLoginBtn.onClick.Clear();
			_appleLoginBtn.onClick.Add(new EventDelegate(() => appleLoginBtnClick?.Invoke()));
		}

		if (_googleLoginBtn)
		{
			_googleLoginBtn.onClick.Clear();
			_googleLoginBtn.onClick.Add(new EventDelegate(() => googleLoginBtnClick?.Invoke()));
		}

		if(_deleteUserBtn)
		{
			_deleteUserBtn.onClick.Clear();
#if UNITY_ANDROID
			_deleteUserBtn.onClick.Add(new EventDelegate(FireBaseController.DeleteUser));
#endif
		}

		if(_signOutBtn)
		{
			_signOutBtn.onClick.Clear();
#if UNITY_ANDROID
			_deleteUserBtn.onClick.Add(new EventDelegate(FireBaseController.AuthSignOut));
#endif
		}
	}
}
