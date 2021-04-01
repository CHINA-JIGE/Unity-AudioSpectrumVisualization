using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumVisualize : MonoBehaviour
{

    public GameObject m_BouncingRectPrefab;
    public GameObject m_BottomRectPrefab;
    public int m_RowCount;
    public int m_ColCount;
    public Vector2 m_GridSize;
    public float m_SmoothFactor;

    private GameObject[,] m_BouncingRects;
    private GameObject[,] m_BottomRects;

    // Start is called before the first frame update
    void Start()
    {
        // instantiate rects
        m_BouncingRects = new GameObject[m_RowCount, m_ColCount];
        m_BottomRects = new GameObject[m_RowCount, m_ColCount];
        Vector3 centerOffset = new Vector3(m_GridSize.x * m_RowCount / 2.0f, 0.0f, m_GridSize.y * m_ColCount / 2.0f);
        for (int row =0; row < m_RowCount; ++row)
        {
            for(int col = 0; col < m_ColCount; ++col)
            {
                Vector3 pos = new Vector3(m_GridSize.x * row, 0.0f, m_GridSize.y * col) - centerOffset;
                m_BouncingRects[row, col] = GameObject.Instantiate(m_BouncingRectPrefab, pos, Quaternion.identity);
                m_BouncingRects[row, col].transform.localScale = new Vector3(0.05f, 1.0f, 0.05f);

                m_BottomRects[row, col] = GameObject.Instantiate(m_BottomRectPrefab, pos, Quaternion.identity);
                m_BottomRects[row, col].transform.localScale = new Vector3(0.1f, 1.0f, 0.1f);
            }
        }

        // play audio
        AudioSource audioSrc = GetComponent<AudioSource>();
        audioSrc.Play();
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource audioSrc = GetComponent<AudioSource>();
        const int spectrumSize = 1024;
        float[] spectrum = new float[spectrumSize];
        audioSrc.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        float powerBase = Mathf.Pow(2.0f, 1.0f / (float)m_RowCount);

        for (int col = 0; col < m_ColCount; ++col)         
        {
            for (int row = 0; row < m_RowCount; ++row)
            {
                float ratio = (Mathf.Pow(powerBase, (float)row) - 1.0f);
                ratio *= 0.2f; // reduce the spectrum range
                int freqIndex = (int)(spectrumSize * ratio);
                if (freqIndex >= spectrumSize) freqIndex = spectrumSize-1;

                int shiftedRow = (13 * col + row) % m_RowCount; // 加点错位

                // smooth lerp and scale
                //float prevScaleY = m_CubeInstances[shiftedRow, col].transform.localScale.y;
                //float currentScaleY = Mathf.Lerp(prevScaleY, spectrum[freqIndex] * 100.0f, m_SmoothFactor);
                //m_CubeInstances[shiftedRow, col].transform.localScale = new Vector3(0.2f, currentScaleY, 0.2f);

                Vector3 prevPos = m_BouncingRects[shiftedRow, col].transform.position;
                float targetHeight = 100.0f * Mathf.Log10(spectrum[freqIndex] + 1.0f);
                float currentPosY = Mathf.Lerp(prevPos.y, targetHeight, m_SmoothFactor);
                m_BouncingRects[shiftedRow, col].transform.position = new Vector3(prevPos.x, currentPosY, prevPos.z);

                Material bottomRectMat = m_BottomRects[shiftedRow, col].GetComponent<MeshRenderer>().material;
                bottomRectMat.SetFloat("_Intensity", currentPosY * 0.2f);
            }
        }
    }
}
