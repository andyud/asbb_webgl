using UnityEngine;
using System.Collections;

public class PopUp_Setting : PopUp
{
    public UIBasicSprite _texture_Wall;
    public UIBasicSprite _texture_Floor;

    public UILabel _label_Title;

    public UILabel _label_Sound;
    public UILabel _label_Sound_On;
    public UILabel _label_Sound_Off;
    public UIButton _button_Sound_On;
    public UIButton _button_Sound_Off;

    public UILabel _label_ContactPoint;
    public UILabel _label_ContactPoint_On;
    public UILabel _label_ContactPoint_Off;
    public UIButton _button_ContactPoint_On;
    public UIButton _button_ContactPoint_Off;

    public UILabel _label_Close;
    public UIButton _button_Close;

	public UILabel DarkModeTitle;
	public UILabel DarkModeOnLabel;
	public UILabel DarkModeOffLabel;
	public UIButton DarkModeOnBtn;
	public UIButton DarkModeOffBtn;


    public UILabel PolicyTitle;
    public UIButton PolicyBtn;

    protected override void Initialize_PopUp()
    {
        _texture_Wall.color = Static_ColorConfigs._Color_ButtonFrame;
        _texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;

		_label_Title.color = Static_ColorConfigs._Color_ButtonFrame;

        _label_Sound.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_Sound_On.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_Sound_Off.color = Static_ColorConfigs._Color_ButtonFrame;

        _label_ContactPoint.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_ContactPoint_On.color = Static_ColorConfigs._Color_ButtonFrame;
        _label_ContactPoint_Off.color = Static_ColorConfigs._Color_ButtonFrame;

        _label_Close.color = Static_ColorConfigs._Color_ButtonFrame;

        _label_Title.text = Static_TextConfigs._Setting;

        _label_Sound.text = Static_TextConfigs._Sound + " : ";
        _label_Sound_On.text = Static_TextConfigs._On;
        _label_Sound_Off.text = Static_TextConfigs._Off;

        _label_ContactPoint.text = Static_TextConfigs._ContactPoint + " : ";
        _label_ContactPoint_On.text = Static_TextConfigs._On;
        _label_ContactPoint_Off.text = Static_TextConfigs._Off;

        _label_Close.text = Static_TextConfigs._Close;

        _button_Sound_On.onClick.Clear();
        _button_Sound_On.onClick.Add(new EventDelegate(ButtonResponse_SoundOn));

        _button_Sound_Off.onClick.Clear();
        _button_Sound_Off.onClick.Add(new EventDelegate(ButtonResponse_SoundOff));

        _button_ContactPoint_On.onClick.Clear();
        _button_ContactPoint_On.onClick.Add(new EventDelegate(ButtonResponse_ContactPointOn));

        _button_ContactPoint_Off.onClick.Clear();
        _button_ContactPoint_Off.onClick.Add(new EventDelegate(ButtonResponse_ContactPointOff));

        _button_Close.onClick.Clear();
        _button_Close.onClick.Add(new EventDelegate(ButtonResponse_Close));

		DarkModeOffLabel.color = DarkModeOnLabel.color = DarkModeTitle.color = Static_ColorConfigs._Color_ButtonFrame;
		DarkModeOffLabel.text = Static_TextConfigs._Off;
		DarkModeOnLabel.text = Static_TextConfigs._On;
		DarkModeTitle.text = Static_TextConfigs.DarkModeTitle + " : ";

        if(PolicyTitle != null)
            PolicyTitle.text = Static_TextConfigs.PolicyTitle;

        DarkModeOnBtn.onClick.Clear();
		DarkModeOnBtn.onClick.Add(new EventDelegate(() => ButtonResponse_DarkMode(true)));

		DarkModeOffBtn.onClick.Clear();
		DarkModeOffBtn.onClick.Add(new EventDelegate(() => ButtonResponse_DarkMode(false)));


        if (PolicyBtn != null)
        {
            PolicyBtn.onClick.Clear();
            PolicyBtn.onClick.Add(new EventDelegate(() => ButtonResponse_Policy()));
        }
    }

    void ButtonResponse_Policy()
    {
        Application.OpenURL(Static_APP_Config._Policy_URL);
    }


    void ButtonResponse_DarkMode(bool isDark)
	{
		UserData._OnDarkMode = isDark;
		Static_ColorConfigs.SetDarkMode(isDark);
		MaterialHolder.Initialize();
        Main._main._gui.SetDarkMode();
		InGameController._instance.SetDarkMode();
		BrickGenerator._instance.SetDarkMode();

		Refresh();
	}

    void ButtonResponse_SoundOn()
    {
        UserData._OnSound = true;
        SoundController.SetSoundOn(UserData._OnSound);

        Refresh();
    }

    void ButtonResponse_SoundOff()
    {
        UserData._OnSound = false;
        SoundController.SetSoundOn(UserData._OnSound);

        Refresh();
    }

    void ButtonResponse_ContactPointOn()
    {
        UserData._ShowContactPoint = true;

        Refresh();
    }

    void ButtonResponse_ContactPointOff()
    {
        UserData._ShowContactPoint = false;

        Refresh();
    }

    void ButtonResponse_Close()
    {
        UserData._ShowContactPoint_Selected = true;
        Close();
    }

    public override void Refresh()
    {
        if (UserData._OnSound)
        {
            _label_Sound_On.color = Static_ColorConfigs._Color_TextStrongGreen;
            _label_Sound_Off.color = Static_ColorConfigs._Color_TextBlack;
        }
        else
        {
            _label_Sound_On.color = Static_ColorConfigs._Color_TextBlack;
            _label_Sound_Off.color = Static_ColorConfigs._Color_TextStrongGreen;
        }

        if (UserData._ShowContactPoint)
        {
            _label_ContactPoint_On.color = Static_ColorConfigs._Color_TextStrongGreen;
            _label_ContactPoint_Off.color = Static_ColorConfigs._Color_TextBlack;
        }
        else
        {
            _label_ContactPoint_On.color = Static_ColorConfigs._Color_TextBlack;
            _label_ContactPoint_Off.color = Static_ColorConfigs._Color_TextStrongGreen;
        }

		if(UserData._OnDarkMode)
		{
			DarkModeOnLabel.color = Static_ColorConfigs._Color_TextStrongGreen;
			DarkModeOffLabel.color = Static_ColorConfigs._Color_TextBlack;
		}
		else
		{
			DarkModeOnLabel.color = Static_ColorConfigs._Color_TextBlack;
			DarkModeOffLabel.color = Static_ColorConfigs._Color_TextStrongGreen;
		}

		_texture_Floor.color = Static_ColorConfigs._Color_PopupBackGround;
	}

    public override void SetUI()
    {

    }
}
