using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
public class DoozyPath
{ public const string UIANIMATIONS = "UIAnimations";
    private const string HIDE = "Hide";
    private const string LOOP = "Loop";
    private const string PUNCH = "Punch";
    private const string SHOW = "Show";
    private const string STATE = "State";
    public static string UIANIMATIONS_RESOURCES_PATH = Path.Combine("Resources", UIANIMATIONS);   // -- Resources/UIAnimations/
    public static string HIDE_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, HIDE);   // -- Resources/UIAnimations/Hide/
    public static string LOOP_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, LOOP);   // -- Resources/UIAnimations/Loop/
    public static string PUNCH_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, PUNCH); // -- Resources/UIAnimations/Punch/
    public static string SHOW_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, SHOW);   // -- Resources/UIAnimations/Show/
    public static string STATE_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, STATE); // -- Resources/UIAnimations/State/

}