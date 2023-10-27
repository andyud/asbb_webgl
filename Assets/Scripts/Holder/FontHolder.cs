using UnityEngine;
using System.Collections;

public class FontHolder : MonoBehaviour
{
	static FontHolder _instance;

	public Font _font_English;
	public Font _font_Korean;

	static Font _currentFont;

	public static void Initialize(SystemLanguage language)
	{
		_instance = FindObjectOfType<FontHolder>();

		if (language == SystemLanguage.Korean)
			_currentFont = _instance._font_Korean;
		else
			_currentFont = _instance._font_English;
	}

	public static Font GetFont()
	{
		return _currentFont;
	}
}
