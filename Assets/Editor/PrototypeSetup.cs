using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace StinyPopela.Editor
{
    public static class PrototypeSetup
    {
        [MenuItem("Stiny Popela/Create Prototype Scene")]
        public static void Create()
        {
            if (!System.IO.Directory.Exists("Assets/Scenes")) AssetDatabase.CreateFolder("Assets","Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var pipeline = UniversalRenderPipeline.asset;
            if (pipeline == null) throw new System.Exception("URP must be active");
            QualitySettings.SetQualityLevel(0,true);
            pipeline = UniversalRenderPipeline.asset;
            pipeline.supportsHDR = true;
            EditorUtility.SetDirty(pipeline);
            var camera=Camera.main;
            camera.backgroundColor=new Color(.08f,.11f,.14f);
            camera.clearFlags=CameraClearFlags.SolidColor;
            RenderSettings.ambientMode=AmbientMode.Flat;
            RenderSettings.ambientLight=new Color(.55f,.6f,.65f);
            camera.GetUniversalAdditionalCameraData().renderPostProcessing=true;
            camera.GetUniversalAdditionalCameraData().volumeLayerMask=1;
            var profile=ScriptableObject.CreateInstance<VolumeProfile>();
            profile.Add<Tonemapping>(true).mode.value=TonemappingMode.ACES;
            var bloom=profile.Add<Bloom>(true);bloom.intensity.value=.5f;bloom.threshold.value=.9f;
            profile.Add<Vignette>(true).intensity.value=.22f;
            const string path="Assets/Settings/PrototypeVolume.asset";
            var existing=AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
            if(existing!=null) { EditorUtility.CopySerialized(profile,existing);Object.DestroyImmediate(profile);profile=existing; }
            else AssetDatabase.CreateAsset(profile,path);
            var volume=new GameObject("Global Volume").AddComponent<Volume>();volume.isGlobal=true;volume.sharedProfile=profile;
            new GameObject("Prototype").AddComponent<PrototypeWorld>();
            PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
            PlayerSettings.companyName="StinyPopela";PlayerSettings.productName="Stíny popela";
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.stinypopela.prototype");
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/Prototype.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Prototype.unity",true)};
            AssetDatabase.SaveAssets();
        }

        public static void BuildAndroid()
        {
            System.IO.Directory.CreateDirectory("Builds/Android");
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes=new[]{"Assets/Scenes/Prototype.unity"}, locationPathName="Builds/Android/StinyPopela.apk",
                target=BuildTarget.Android,options=BuildOptions.Development
            });
            if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new System.Exception("Android build failed: "+report.summary.result);
        }
    }
}
