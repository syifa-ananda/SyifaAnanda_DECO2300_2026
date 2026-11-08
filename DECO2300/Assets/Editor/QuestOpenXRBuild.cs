using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

internal static class QuestOpenXRBuild
{
    private const string RequestFileName = ".codex-quest-build-request";
    private const string OutputPath = "Builds/DECO2300-Quest.apk";
    private const string ScenePath = "Assets/Scenes/EventVenue_Final.unity";

    [InitializeOnLoadMethod]
    private static void QueueRequestedBuild()
    {
        if (File.Exists(GetRequestPath()))
            EditorApplication.delayCall += ConfigureAndBuildWhenReady;
    }

    private static void ConfigureAndBuildWhenReady()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;

            EditorApplication.delayCall += ConfigureAndBuildWhenReady;
            return;
        }

        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += ConfigureAndBuildWhenReady;
            return;
        }

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
        {
            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
                throw new InvalidOperationException("Could not switch the active build target to Android.");

            EditorApplication.delayCall += ConfigureAndBuildWhenReady;
            return;
        }

        File.Delete(GetRequestPath());
        ConfigureOpenXR();
        ConfigureAndroidPlayer();

        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath) ?? "Builds");
        AssetDatabase.SaveAssets();

        var buildOptions = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None
        };

        Debug.Log($"[QuestOpenXRBuild] Building immersive Quest APK at {OutputPath}");
        var report = BuildPipeline.BuildPlayer(buildOptions);
        if (report.summary.result != BuildResult.Succeeded)
            throw new BuildFailedException($"Quest build failed: {report.summary.result}");

        Debug.Log($"[QuestOpenXRBuild] BUILD SUCCEEDED: {report.summary.outputPath} ({report.summary.totalSize} bytes)");
    }

    private static void ConfigureOpenXR()
    {
        const BuildTargetGroup targetGroup = BuildTargetGroup.Android;

        if (!EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey,
                out XRGeneralSettingsPerBuildTarget perTargetSettings) || perTargetSettings == null)
        {
            EnsureAssetFolder("Assets/XR/Settings");
            perTargetSettings = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
            AssetDatabase.CreateAsset(perTargetSettings, "Assets/XR/Settings/XRGeneralSettingsPerBuildTarget.asset");
            EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, perTargetSettings, true);
        }

        if (!perTargetSettings.HasManagerSettingsForBuildTarget(targetGroup))
            perTargetSettings.CreateDefaultManagerSettingsForBuildTarget(targetGroup);

        var generalSettings = perTargetSettings.SettingsForBuildTarget(targetGroup);
        generalSettings.InitManagerOnStart = true;
        EditorUtility.SetDirty(generalSettings);

        var managerSettings = perTargetSettings.ManagerSettingsForBuildTarget(targetGroup);
        if (!XRPackageMetadataStore.AssignLoader(managerSettings, typeof(OpenXRLoader).FullName, targetGroup))
            throw new InvalidOperationException("Could not assign the OpenXR loader to Android.");

        FeatureHelpers.RefreshFeatures(targetGroup);
        EnableFeature(targetGroup, MetaQuestFeature.featureId);
        EnableFeature(targetGroup, OculusTouchControllerProfile.featureId);
        EnableFeature(targetGroup, MetaQuestTouchPlusControllerProfile.featureId);
        EnableFeature(targetGroup, MetaQuestTouchProControllerProfile.featureId);

        var openXRSettings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
        openXRSettings.renderMode = OpenXRSettings.RenderMode.SinglePassInstanced;
        EditorUtility.SetDirty(openXRSettings);
    }

    private static void EnableFeature(BuildTargetGroup targetGroup, string featureId)
    {
        var feature = FeatureHelpers.GetFeatureWithIdForBuildTarget(targetGroup, featureId);
        if (feature == null)
            throw new InvalidOperationException($"OpenXR feature not found: {featureId}");

        feature.enabled = true;
        EditorUtility.SetDirty(feature);
    }

    private static void ConfigureAndroidPlayer()
    {
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.Vulkan });
        EditorUserBuildSettings.buildAppBundle = false;
    }

    private static string GetRequestPath()
    {
        return Path.Combine(Directory.GetParent(Application.dataPath)!.FullName, RequestFileName);
    }

    private static void EnsureAssetFolder(string folderPath)
    {
        var parts = folderPath.Split('/');
        var current = parts[0];
        for (var index = 1; index < parts.Length; index++)
        {
            var next = $"{current}/{parts[index]}";
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[index]);
            current = next;
        }
    }
}

internal sealed class QuestDeviceSceneProcessor : IProcessSceneWithReport
{
    public int callbackOrder => 0;

    public void OnProcessScene(Scene scene, BuildReport report)
    {
        if (report == null || report.summary.platform != BuildTarget.Android)
            return;

        foreach (var root in scene.GetRootGameObjects().Where(root => root.name == "XR Interaction Simulator"))
        {
            root.SetActive(false);
            Debug.Log($"[QuestOpenXRBuild] Disabled editor-only XR Interaction Simulator in build scene: {scene.path}");
        }
    }
}
