using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
	static SoundController _instance;

	static List<AudioSource> _audioSource_List = new List<AudioSource>();

	public AudioClip _sfx_Success;
	public AudioClip _sfx_Impact_02;
	public AudioClip _sfx_Impact_03;
	public AudioClip _sfx_GameOver;

	static int _ignoreFrame_BrickImpact;
	static int _ignoreFrame_PickUpImpact;

	public static void Initialize()
	{
		_instance = FindObjectOfType<SoundController>();
		_ignoreFrame_BrickImpact = 0;
		_ignoreFrame_PickUpImpact = 0;

		for (int i = 0; i < 10; i++)
			_audioSource_List.Add(_instance.gameObject.AddComponent<AudioSource>());

		SetSoundOn(UserData._OnSound);
	}

	public static void SetSoundOn(bool b)
	{
		if (b)
		{
			AudioListener.volume = 1;
			NGUITools.soundVolume = 1;
		}
		else
		{
			AudioListener.volume = 0;
			NGUITools.soundVolume = 0;
		}
	}

	protected static void PlaySound(AudioClip clip, float volume, float pitch, float playDelay = 0)
	{
		if (UserData._OnSound == false)
			return;

		AudioSource audio = GetAudioSourceOnStandBy();

		audio.playOnAwake = false;
		audio.loop = false;
		audio.volume = volume;
		audio.pitch = pitch;
		audio.clip = clip;

		if (playDelay == 0)
			audio.Play();
		else
			audio.PlayDelayed(playDelay);
	}

	static AudioSource GetAudioSourceOnStandBy()
	{
		for (int i = 0; i < _audioSource_List.Count; i++)
		{
			if (!_audioSource_List[i].isPlaying)
				return _audioSource_List[i];
		}

		AudioSource audio = _instance.gameObject.AddComponent<AudioSource>();
		_audioSource_List.Add(audio);
		return audio;
	}

	public static void Play_Success(float volume = 1)
	{
		PlaySound(_instance._sfx_Success, volume, 1);
	}

	public static void Play_GameOver()
	{
		PlaySound(_instance._sfx_GameOver, 1, 1);
	}

	public static void Play_Impact_Brick()
	{
		if (_ignoreFrame_BrickImpact <= 0)
		{
			float pitch = 0.95f + 0.1f * Random.value;
			float volume = 0.6f;
			PlaySound(_instance._sfx_Impact_02, volume, pitch);
			_ignoreFrame_BrickImpact = 2;
		}
	}

	public static void Play_Impact_PickUp()
	{
		if (_ignoreFrame_PickUpImpact <= 0)
		{
			PlaySound(_instance._sfx_Impact_03, 1, 1.25f);
			_instance.StartCoroutine(Thread_Play_Impack_PickUp_Delay());
			_ignoreFrame_PickUpImpact = 1;
		}
	}

	static IEnumerator Thread_Play_Impack_PickUp_Delay()
	{
		float elapsedTime = 0;
		float duration = 0.1f;
		while (elapsedTime < duration)
		{
			yield return 0;
			elapsedTime += Time.deltaTime;
		}

		PlaySound(_instance._sfx_Impact_03, 1, 1.75f);
	}

	void FixedUpdate()
	{
		if (_ignoreFrame_BrickImpact > 0)
			_ignoreFrame_BrickImpact--;

		if (_ignoreFrame_PickUpImpact > 0)
			_ignoreFrame_PickUpImpact--;
	}
}
