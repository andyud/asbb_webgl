using UnityEngine;
using System.Collections;

public class HowToPlay : PhasePopUp
{
	public UILabel _label_Title;
	public UIBasicSprite _texture_TopLine;
	public UIBasicSprite _texture_BottomLine;
	public UIBasicSprite _texture_LeftLine;
	public UIBasicSprite _texture_RightLine;

	public HowToPlay_Initializer _initializer;



	protected override void Initialize_PhasePopUp ()
	{
		_texture_Body.gameObject.SetActive (false);
		_texture_Frame.gameObject.SetActive (false);

		//_label_Title.color = Static_ColorConfigs._Color_TextBlack;
		//_label_Title.text = Static_TextConfigs._HowToPlay_Title;

		_texture_TopLine.color = Static_ColorConfigs._Color_TextBlack;
		_texture_BottomLine.color = Static_ColorConfigs._Color_TextBlack;
		_texture_LeftLine.color = Static_ColorConfigs._Color_TextBlack;
		_texture_RightLine.color = Static_ColorConfigs._Color_TextBlack;

		_initializer.Initialize ();
	}

	protected override void StartPhase ()
	{

	}

	protected override void EndPhase ()
	{

	}

    public override void SetUI()
    {

    }
}
