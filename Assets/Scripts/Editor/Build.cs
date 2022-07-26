using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

public class Build
{
    [MenuItem("Assets/Build Application")]
    public static void BuildTest()
    {
        //�v���b�g�t�H�[���A�I�v�V����
        bool isDevelopment = true;
        BuildTarget platform = BuildTarget.StandaloneWindows;

        // �o�͖��Ƃ�
        var exeName = PlayerSettings.productName;
        var ext = ".exe";
        var outpath = "";

        // �r���h�ΏۃV�[�����X�g
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        //�R�}���h���C���������p�[�X
        //NOTE: iOS�̓v���W�F�N�g����������邾���Ȃ̂ŁA�ʓr��������
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-projectPath":
                    outpath = args[i + 1] + "\\Build";
                    break;
                case "-devmode":
                    isDevelopment = args[i + 1] == "true";
                    break;
                case "-platform":
                    switch (args[i + 1])
                    {
                        case "Android":
                            platform = BuildTarget.Android;
                            ext = ".apk";
                            break;

                        case "Windows":
                            platform = BuildTarget.StandaloneWindows;
                            ext = ".exe";
                            break;

                        case "WebGL":
                            platform = BuildTarget.WebGL;
                            PlayerSettings.WebGL.decompressionFallback = true; //�����Ȃ���������̂Ń`�F�b�N�����
                            ext = "";
                            break;

                        case "Switch":
                            platform = BuildTarget.Switch;
                            ext = "";
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        //�r���h�I�v�V�����̐��^
        var option = new BuildPlayerOptions();
        option.scenes = scenes;
        option.locationPathName = outpath + "\\" + exeName + ext;
        if (isDevelopment)
        {
            //options�̓r�b�g�t���O�Ȃ̂ŁA|�Œǉ����Ă������Ƃ��ł���
            option.options = BuildOptions.Development | BuildOptions.AllowDebugging;
        }
        option.target = platform; //�r���h�^�[�Q�b�g��ݒ�. �����Win64

        // ���s
        var report = BuildPipeline.BuildPlayer(option);

        // ���ʏo��
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("BUILD SUCCESS");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError("BUILD FAILED");

            foreach (var step in report.steps)
            {
                Debug.Log(step.ToString());
            }

            Debug.LogError("Erro Count: " + report.summary.totalErrors);
            EditorApplication.Exit(1);
        }
    }
}