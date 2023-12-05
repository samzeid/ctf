using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SizeAdjuster : MonoBehaviour
{
    private float defaultScale = 0.16f;

    private void Awake()
    {
        if (!GetComponent<TextMeshProUGUI>())
            transform.localScale = Vector3.one * defaultScale;
    }
}
