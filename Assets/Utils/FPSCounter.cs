using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour {
    [SerializeField] private bool display;
    [SerializeField] private int samples;
    [SerializeField] private TMP_Text text;
    private int[] buffer;
    private int bufferIndex;

    private void Update() {
        if (!display) {
            text.gameObject.SetActive(false);
            return;
        }
        if (buffer == null || samples != buffer.Length) {
            InitBuffer();
        }
        text.gameObject.SetActive(true);
        UpdateBuffer();
        UpdateText(CalculateFramerate());
    }

    private void InitBuffer() {
        if (samples <= 0) { samples = 1; }
        buffer = new int[samples];
        bufferIndex = 0;
    }

    private void UpdateBuffer() {
        buffer[bufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        bufferIndex %= samples;
    }

    private (int min, int avg, int max) CalculateFramerate() {
        int sum = 0;
        int min = int.MaxValue;
        int max = 0;
        for (int i = 0; i < samples; i++) {
            int fps = buffer[i];
            sum += fps;
            if (fps > max) {
                max = fps;
            }
            if (fps < min) {
                min = fps;
            }
        }
        return (min, sum / samples, max);
    }

    private void UpdateText((int min, int avg, int max) fps) {
        text.SetText(string.Format("Avg:{0}\nMin:{1}\nMax:{2}", fps.avg, fps.min, fps.max));
    }
}
