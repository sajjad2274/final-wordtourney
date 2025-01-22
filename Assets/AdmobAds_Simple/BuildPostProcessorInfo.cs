#if UNITY_EDITOR && UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using System.IO;


    public class BuildPostProcessorInfo
    {
        [PostProcessBuild]
        public static void OnPostBuildProcessInfo(BuildTarget target, string pathXcode)
        {
            if (target == BuildTarget.iOS)
            {
                var infoPlistPath = pathXcode + "/Info.plist";

                var document = new PlistDocument();
                document.ReadFromString(File.ReadAllText(infoPlistPath));
                var elementDict = document.root;
                elementDict.SetString("NSUserTrackingUsageDescription", "Your data will be used to deliver personalized ads to you.");
                File.WriteAllText(infoPlistPath, document.WriteToString());
            }
        }
}
#endif

