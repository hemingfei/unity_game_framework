//
//  UIEditorConstant.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

namespace CustomGameFramework.Editor
{
    public class UIEditorConstant
    {
        public const string S_ResFolderModePath = "Res/AssetBundle";

        public const string S_UIAssemblyName = "Assembly-CSharp";
        public const string S_AppNamespace = "GameMain";
        public const string S_UiTemplatePath = "Template/UITemplate";
        public const string S_UiWindowClassPath = "Scripts/UI";

        public const string S_HotfixUIAssemblyName = "GameMain.Hotfix";
        public const string S_HotfixNamespace = "GameMain.Hotfix";
        public const string S_HotfixUiTemplatePath = "Template/UITemplate";
        public const string S_HotfixUiWindowClassPath = "ScriptsHotfix/UI";

        public static string[] S_AutoBindRuleHelperAssemblyNames =
        {
            //"Assembly-CSharp",
            "CustomGameFramework.Runtime",

        };
    }
}
