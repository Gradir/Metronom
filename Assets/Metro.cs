using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AudioSource))]
public class Metro : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI tickText;
	[SerializeField] private TMPro.TextMeshProUGUI metrumText;
	[SerializeField] private TMPro.TextMeshProUGUI bpmText;
	[SerializeField] private int minSignatureLo = 2;
	[SerializeField] private int maxSignatureLo = 16;
	public double bpm = 140.0F;
	public float gain = 0.5F;
	public int signatureHi = 4;
	public int signatureLo = 4;
	private double nextTick = 0.0F;
	private float amp = 0.0F;
	private float phase = 0.0F;
	private double sampleRate = 0.0F;
	private int accent;
	private bool running = false;
	private bool changeTempo = false;
	private bool changeTickText = false;
	private const string tickString = "Tick: ";
	private const string slashString = "/";
	private const string metrumString = "Metrum: ";
	private const string bpmString = "BPM: ";

	void Start()
	{
		accent = signatureHi;
		double startTick = AudioSettings.dspTime;
		sampleRate = AudioSettings.outputSampleRate;
		nextTick = startTick * sampleRate;
		running = true;

		bpmText.text = bpmString + bpm;
		SetupTexts();
	}

	private void SetupTexts()
	{
		tickText.text = tickString + accent + slashString + signatureHi;
		metrumText.text = metrumString + signatureLo + slashString + signatureHi;
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
		if (!running)
			return;

		double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
		double sample = AudioSettings.dspTime * sampleRate;
		int dataLen = data.Length / channels;
		int n = 0;

		while (n < dataLen)
		{
			float x = gain * amp * Mathf.Sin(phase);
			int i = 0;
			while (i < channels)
			{
				data[n * channels + i] += x;
				i++;
			}
			while (sample + n >= nextTick)
			{
				nextTick += samplesPerTick;
				amp = 1.0F;
				if (++accent > signatureHi)
				{
					accent = 1;
					amp *= 2.0F;
				}
				changeTickText = true;
			}
			phase += amp * 0.3F;
			amp *= 0.993F;
			n++;
			if (accent == signatureHi)
			{
				changeTempo = true;
			}
		}
	}

	private void Update()
	{
		if (changeTickText)
		{
			SetupTexts();
			changeTickText = false;
		}
		if (changeTempo)
		{
			changeTempo = false;
			RollSignature();
		}
	}

	private void RollSignature()
	{
		//signatureHi = UnityEngine.Random.Range(minSignatureHi, maxSignatureHi);

		if (UnityEngine.Random.Range(0,99) < 50)
		{
			signatureHi = signatureHi == 4 ? 8 : 4;
		}
		signatureLo = UnityEngine.Random.Range(minSignatureLo, maxSignatureLo);
	}
}