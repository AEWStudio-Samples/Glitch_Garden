using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDLink : MonoBehaviour
{
    // HUD elements that link to GUIControll //
    [Header("GUIControll Elements")]
    public TextMeshProUGUI ninjaCounter = null;
    public TextMeshProUGUI zombieCounter = null;
    public TextMeshProUGUI roundCounter = null;
    public TextMeshProUGUI coinCounter = null;

    // HUD elements that link to PriceManager //
    [Header("PriceManager Elements")]
    public TextMeshProUGUI ninjaPrice = null;
    public TextMeshProUGUI wallPrice = null;
    public TextMeshProUGUI pitPrice = null;
    public TextMeshProUGUI minePrice = null;
}
