using UnityEngine;
using System.Collections;

public class PopUp_Pause : PopUp
{
	public UIBasicSprite _texture_Wall;
	public UIBasicSprite _texture_Floor;
	public UILabel _label_BackToTitle;
	public UILabel _label_Resume;
    public UILabel _label_QuitGame;
    public UIButton _button_BackToTitle;
    public UIButton _button_Resume;
    public UIButton _button_QuitGame;

    protected override void Initialize_PopUp()
	{
		_texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
		_label_BackToTitle.color = Static_ColorConfigs._Color_ButtonFrame;
		_label_Resume.color = Static_ColorConfigs._Color_ButtonFrame;
        //_label_QuitGame.color = ColorConfigs._Color_ButtonFrame;

        _label_BackToTitle.text = Static_TextConfigs._BackToTitle;
		_label_Resume.text = Static_TextConfigs._Resume;
        //_label_QuitGame.text = TextConfigs._Quit;

        _button_BackToTitle.onClick.Clear ();
		_button_BackToTitle.onClick.Add (new EventDelegate (ButtonResponse_BackToTitle));

		_button_Resume.onClick.Clear ();
		_button_Resume.onClick.Add (new EventDelegate (ButtonResponse_Resume));

        //_button_QuitGame.onClick.Clear();
        //_button_QuitGame.onClick.Add(new EventDelegate(ButtonResponse_QuitGame));
    }

	void ButtonResponse_BackToTitle()
	{
		if (Main._main._game._Score_Current > UserData._Score_Best)
		{
			UserData._Score_Best = Main._main._game._Score_Current;
			//GameCenterController.Post_HighScore(Main._main._game._Score_Current);
		}

		Main.BackToTitle();
	}

	void ButtonResponse_Resume()
	{
		Close ();
	}

    //void ButtonResponse_QuitGame()
    //{
    //    Application.Quit();
    //}

    public override void Refresh ()
	{
		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
	}

    public override void SetUI()
    {

    }
}
