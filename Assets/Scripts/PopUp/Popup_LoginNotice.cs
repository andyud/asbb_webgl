using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_LoginNotice : PopUp
{
    [SerializeField] private UIButton _closeBtn;

    [SerializeField] private UILabel _titleLabel;
    [SerializeField] private UISprite BG;

	protected override void Initialize_PopUp()
	{
		BG.color = Static_ColorConfigs._Color_PopupBackGround;

		if (Application.systemLanguage == SystemLanguage.Korean || Application.systemLanguage == SystemLanguage.English)
			_titleLabel.text = Static_TextConfigs.LoginNotice_Popup_Comment;
		else
        {
			//영어 한국어 이란어 우즈벡어 독일어 네덜란드어
			if (Application.systemLanguage == SystemLanguage.Turkish)
				_titleLabel.text = "Yeni bir oyuna giriş yaptığınızda puanınızı ve satın alma geçmişinizi alabilirsiniz. Giriş yapmak için ana ekrandaki Giriş düğmesine tıklayın.\n\nLütfen herhangi bir sorunuz varsa bizimle iletişime geçmekten çekinmeyin. (hailey0hailey0@gmail.com)";
			else if (Application.systemLanguage == SystemLanguage.Spanish)
				_titleLabel.text = "Cuando inicias sesión en un juego nuevo, puedes recuperar tu puntaje y el historial de compras. Para iniciar sesión, haga clic en el botón Iniciar sesión en la pantalla principal.\n\nNo dude en contactarnos si tiene alguna pregunta. (hailey0hailey0@gmail.com)";
			else if (Application.systemLanguage == SystemLanguage.Portuguese)
				_titleLabel.text = "Ao fazer login em um novo jogo, você pode recuperar sua pontuação e histórico de compras. Para fazer login, clique no botão Login na tela principal.\n\nSinta-se à vontade para nos contatar em caso de dúvidas. (hailey0hailey0@gmail.com)";
			else if (Application.systemLanguage == SystemLanguage.Arabic)
				_titleLabel.text = "عند تسجيل الدخول إلى لعبة جديدة ، يمكنك استرداد نتيجتك وسجل الشراء. لتسجيل الدخول ، انقر فوق الزر تسجيل الدخول على الشاشة الرئيسية. \n\n لا تتردد في الاتصال بنا لطرح أي أسئلة. (hailey0hailey0@gmail.com)";
			else if (Application.systemLanguage == SystemLanguage.German)
				_titleLabel.text = "Wenn Sie sich bei einem neuen Spiel anmelden, können Sie Ihren Punktestand und Ihre Kaufhistorie abrufen. Um sich anzumelden, klicken Sie auf dem Startbildschirm auf die Schaltfläche „Anmelden“. \n\n Fühlen Sie sich frei, Ihre Fragen zu stellen. (hailey0hailey0@gmail.com)";
			else
				_titleLabel.text = Static_TextConfigs.LoginNotice_Popup_Comment;
		}

		//_closeBtn.onClick.Clear();
		//_closeBtn.onClick.Add(new EventDelegate(() =>
		//{
		//	Debug.Log("Popup_LoginNotice._closeBtn onClick");
		//	Close();
		//}));
	}

	public override void SetUI() { }

	public override void Refresh()
	{
		BG.color = Static_ColorConfigs._Color_PopupBackGround;
	}

	public void ClosePopup()
    {
		Close();
	}

}
