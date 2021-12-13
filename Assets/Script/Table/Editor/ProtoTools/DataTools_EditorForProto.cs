using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.IO;
using System.Diagnostics;
using System.Text;
using System;
using ExportExcelByNPOI;
using System.Data;
using Debug = UnityEngine.Debug;

public class DataTools_EditorForProto : EditorWindow
{
    private enum CurrentWindowsState
    {
        ExportTableClass,
        GenerateExcelFromProtoFile,
        CreatingBytesFromExcel
    }

    private static string defaultDirectory = Directory.GetCurrentDirectory();
    /// <summary>
    /// 默认目录
    /// </summary>
    public static string DefaultDirectory { get { return defaultDirectory; } }

    private static string defaultTableFilePath;//cs文件默认保存位置
    private static string defaultByteFilesPath;//二进制文件默认保存位置

    private static Rect windowRect = new Rect(0, 0, 1000, 600);
    private static Rect rectTemp;
    private static CurrentWindowsState currentState = CurrentWindowsState.ExportTableClass;
    private static UnityEngine.Object[] globalSelectedObjs;

    private static GUIStyle redTextStyle;
    protected static GUIStyle RedTextStyle
    {
        get
        {
            if (redTextStyle == null)
            {
                redTextStyle = new GUIStyle();
                redTextStyle.normal.textColor = Color.red;
            }
            return redTextStyle;
        }
    }

    [MenuItem("DataTools/TABLE配置")]
    static void OpenProtoEditorWindows()
    {
        GetWindowWithRect(typeof(DataTools_EditorForProto), windowRect, true, "TABLE配置");
        globalSelectedObjs = Selection.objects;
    }

    #region Editor窗口
    private static bool isAnyObjectBeingDragged = false;
    private static bool isDraggingEnded = false;
    private static Vector2 mousePosDuringDragged;
    private void OnGUI()
    {
        isAnyObjectBeingDragged = (Event.current.type == EventType.DragUpdated);
        isDraggingEnded = (Event.current.type == EventType.DragExited);
        if (isAnyObjectBeingDragged)
        {
            mousePosDuringDragged = Event.current.mousePosition;
        }
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = (currentState == CurrentWindowsState.ExportTableClass) ? Color.cyan : Color.white;
        if (GUILayout.Button("从proto文件生成C#脚本", GUILayout.Width(200)))
        {
            currentState = CurrentWindowsState.ExportTableClass;
        }
        GUI.backgroundColor = (currentState == CurrentWindowsState.GenerateExcelFromProtoFile) ? Color.cyan : Color.white;
        if (GUILayout.Button("从proto脚本生成Excel", GUILayout.Width(200)))
        {
            currentState = CurrentWindowsState.GenerateExcelFromProtoFile;
        }
        GUI.backgroundColor = (currentState == CurrentWindowsState.CreatingBytesFromExcel) ? Color.cyan : Color.white;
        if (GUILayout.Button("从Excel导出二进制文件", GUILayout.Width(200)))
        {
            currentState = CurrentWindowsState.CreatingBytesFromExcel;
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();
        switch (currentState)
        {
            case CurrentWindowsState.ExportTableClass:
                OnGUIForExportTableFromProto();
                break;
            case CurrentWindowsState.GenerateExcelFromProtoFile:
                OnGUIGenerateExcelFileFromProtoFile();
                break;
            case CurrentWindowsState.CreatingBytesFromExcel:
                OnGUICreateByteFromExcel();
                break;
            default:
                break;
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void OnSelectionChange()
    {
        globalSelectedObjs = Selection.objects;
    }
    #endregion

    #region 创建proto对应的C#脚本
    private static string tableFilePathTemp;
    private static Vector2 _ScrollPos;
    private static void OnGUIForExportTableFromProto()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(".cs导出目录:", GUILayout.Width(100));
        rectTemp = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        if (isAnyObjectBeingDragged && rectTemp.Contains(mousePosDuringDragged) && DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 && DragAndDrop.paths[0].Length > 7)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            tableFilePathTemp = DragAndDrop.paths[0].Substring(7, DragAndDrop.paths[0].Length - 7);
            tableFilePathTemp = EditorGUI.TextField(rectTemp, tableFilePathTemp);
        }
        else if (isDraggingEnded)
        {
            if (rectTemp.Contains(mousePosDuringDragged))
            {
                DefaultTableFilePath = tableFilePathTemp;
            }
        }
        else
        {
            DefaultTableFilePath = EditorGUI.TextField(rectTemp, DefaultTableFilePath);
        }
        if (string.IsNullOrEmpty(DefaultTableFilePath))
        {
            GUILayout.Label("文件夹路径不可为空", RedTextStyle);
        }
        GUILayout.EndHorizontal();
        List<string> protoFileFullPathList = GetFullPathesForSelectedObjects(".proto");
        GUI.enabled = protoFileFullPathList.Count > 0 && !string.IsNullOrEmpty(DefaultTableFilePath);
        if (GUILayout.Button("创建cs文件", GUILayout.Width(400)))
        {
            string outputDirectory = GetFullPath(DefaultTableFilePath);
            for (int i = 0; i < protoFileFullPathList.Count; i++)
            {
                EditorUtility.DisplayProgressBar("创建cs文件", string.Format("正在创建 {0}", protoFileFullPathList[i]), ((float)i) / protoFileFullPathList.Count);
                GenerateTableFromProto(protoFileFullPathList[i], outputDirectory);
            }
            EditorUtility.ClearProgressBar();
        }
        GUI.enabled = true;
        GUILayout.Label(string.Format("已选择{0}个proto文件", protoFileFullPathList.Count));
        if (protoFileFullPathList.Count > 0)
        {
            _ScrollPos = GUILayout.BeginScrollView(_ScrollPos);
            for (int i = 0; i < protoFileFullPathList.Count; i++)
            {
                GUILayout.Label(string.Format("{0}.  {1}", i + 1, protoFileFullPathList[i]));
            }
            GUILayout.EndScrollView();
        }
    }

    private static string protoDirectory = string.Empty;
    private static string protoName = string.Empty;
    private static string protoPureName = string.Empty;
    private static string outputFullPath = string.Empty;
    private static string parameter = string.Empty;
    /// <summary>
    /// 导出proto对应的C#脚本至文件夹中
    /// </summary>
    /// <param name="protoFullPath">proto文件的完整路径</param>
    /// <param name="outputDirectory">生成的数据类的输出文件夹完整路径</param>
    /// <param name="classFileName">生成的数据类文件名(为空时默认与proto文件名保持一致)</param>
    private static void GenerateTableFromProto(string protoFullPath, string outputDirectory, string classFileName = null)
    {
        //路径检查
        if (string.IsNullOrEmpty(protoFullPath) || string.IsNullOrEmpty(outputDirectory))
        {
            return;
        }
        protoFullPath = protoFullPath.Replace('\\', '/');
        outputDirectory = outputDirectory.Replace('\\', '/');
        if (protoFullPath.Length < 6 || protoFullPath.Substring(protoFullPath.Length - 6, 6) != ".proto")
        {
            return;
        }
        if (!string.IsNullOrEmpty(classFileName) && classFileName.Length > 3 && classFileName.Substring(classFileName.Length - 3, 3) == ".cs")
        {
            classFileName = classFileName.Substring(0, classFileName.Length - 3);
        }
        try
        {
            protoDirectory = Path.GetDirectoryName(protoFullPath);
            protoName = Path.GetFileName(protoFullPath);
            protoPureName = Path.GetFileNameWithoutExtension(protoFullPath);
            outputFullPath = string.Format("{0}/{1}.cs", outputDirectory, string.IsNullOrEmpty(classFileName) ? protoPureName : classFileName);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            if (File.Exists(outputFullPath))
            {
                File.Delete(outputFullPath);
            }
            parameter = string.Format("-i:{0} -o:{1} -p:detectMissing", protoName, outputFullPath);
            if (CallProgress("protogen", parameter, protoDirectory))
            {
                if (File.Exists(outputFullPath))
                {
                    UnityEngine.Debug.Log(string.Format("{0} 导出成功", outputFullPath));
                }
                else
                {
                    UnityEngine.Debug.Log(string.Format("{0} 导出失败", outputFullPath));
                }
            }
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("从 {0} 导出 {1} 出错  {2}", protoFullPath, outputFullPath, ex.Message));
        }
        finally
        {
            Directory.SetCurrentDirectory(DefaultDirectory);
            AssetDatabase.Refresh();
        }
    }
    #endregion

    #region 从proto文件生成Excel
    private static string selectedProtoFilePathTemp;

    private static void OnGUIGenerateExcelFileFromProtoFile()
    {
        List<string> protoFileFullPathList = GetFullPathesForSelectedObjects(".proto");
        GUI.enabled = protoFileFullPathList.Count > 0;
        if (GUILayout.Button("生成xls文件", GUILayout.Width(400)))
        {
            for (int i = 0; i < protoFileFullPathList.Count; i++)
            {
                EditorUtility.DisplayProgressBar("生成xls文件", string.Format("正在生成 {0}", protoFileFullPathList[i]), ((float)i) / protoFileFullPathList.Count);
                GenerateExcelFromProto(protoFileFullPathList[i]);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
        GUI.enabled = true;
        GUILayout.Label(string.Format("已选中 {0} 个proto文件:", protoFileFullPathList.Count), GUILayout.Width(300));
        if (protoFileFullPathList.Count > 0)
        {
            _ScrollPos = GUILayout.BeginScrollView(_ScrollPos);
            for (int i = 0; i < protoFileFullPathList.Count; i++)
            {
                GUILayout.Label(string.Format("{0},  {1}", i + 1, protoFileFullPathList[i]));
            }
            GUILayout.EndScrollView();
        }
    }

    private static void GenerateExcelFromProto(string protoFullPath)
    {
        if (string.IsNullOrEmpty(protoFullPath))
        {
            UnityEngine.Debug.Log("路径错误");
            return;
        }
        if (!File.Exists(protoFullPath))
        {
            UnityEngine.Debug.Log("路径不存在");
            return;
        }
        Proto protoTemp = new Proto(protoFullPath);
        bool isSuccess;
        Exception result = protoTemp.Deal(out isSuccess);
        if (result != null)
        {
            if (isSuccess)
            {
                Debug.Log(result.Message);
            }
            else
            {
                Debug.LogError(result.Message);
            }
        }
    }
    #endregion

    #region 从Excel导出二进制文件
    private static string thisCSDirectoryPath = string.Empty;
    protected static string ThisCSDirectoryPath
    {
        get
        {
            if (string.IsNullOrEmpty(thisCSDirectoryPath))
            {
                thisCSDirectoryPath = GetCurrentCSFileDirectory();
            }
            return thisCSDirectoryPath;
        }
    }

    private static string protocFullPath = string.Empty;
    protected static string ProtocFullPath
    {
        get
        {
            if (string.IsNullOrEmpty(protocFullPath))
            {
                protocFullPath = ThisCSDirectoryPath + "/protoc.exe";
            }
            return protocFullPath;
        }
    }

    private static string tableWriterFullPath = string.Empty;
    protected static string TableWriterFullPath
    {
        get
        {
            if (string.IsNullOrEmpty(tableWriterFullPath))
            {
                tableWriterFullPath = ThisCSDirectoryPath + "/table_writer.py";
            }
            return tableWriterFullPath;
        }
    }

    private static List<BytePair> pairList;
    protected static List<BytePair> PairList { get { pairList = pairList ?? ReadPairInfo(); return pairList; } }

    private static bool canGenerateByte = false;
    private static int needComponentMissingInfoCounting = 0;

    private static string[] draggedPathes;
    private static string[] pathesToBeAdded;
    private static string protoFilePathBuff = string.Empty;
    protected static string ProtoFilePathBuff
    {
        get
        {
            if (string.IsNullOrEmpty(protoFilePathBuff))
            {
                protoFilePathBuff = Application.dataPath;
            }
            return protoFilePathBuff;
        }
        set
        {
            protoFilePathBuff = value;
        }
    }

    private static string excelFilePathBuff = string.Empty;
    protected static string ExcelFilePathBuff
    {
        get
        {
            if (string.IsNullOrEmpty(excelFilePathBuff))
            {
                excelFilePathBuff = Application.dataPath;
            }
            return excelFilePathBuff;
        }
        set
        {
            excelFilePathBuff = value;
        }
    }

    private static string protoFilePathTemp;
    private static string excelFilePathTemp;

    private static void OnGUICreateByteFromExcel()
    {
        #region 导出目录
        GUILayout.BeginHorizontal();
        GUILayout.Label(".byte导出目录:", GUILayout.Width(100));
        rectTemp = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        if (isAnyObjectBeingDragged && rectTemp.Contains(mousePosDuringDragged) && DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 && DragAndDrop.paths[0].Length > 7)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            tableFilePathTemp = DragAndDrop.paths[0].Substring(7, DragAndDrop.paths[0].Length - 7);
            tableFilePathTemp = EditorGUI.TextField(rectTemp, tableFilePathTemp);
        }
        else if (isDraggingEnded)
        {
            if (rectTemp.Contains(mousePosDuringDragged))
            {
                DefaultByteFilePath = tableFilePathTemp;
            }
        }
        else
        {
            DefaultByteFilePath = EditorGUI.TextField(rectTemp, DefaultByteFilePath);
        }
        GUILayout.EndHorizontal();
        #endregion

        #region 创建二进制文件按钮
        GUI.enabled = PairList != null && PairList.Count > 0;
        if (GUILayout.Button("创建二进制文件", GUILayout.Width(400)))
        {
            canGenerateByte = IsRequiredComponentExist();
            if (canGenerateByte)
            {
                needComponentMissingInfoCounting = 0;
                GenerateBytes();
            }
            else
            {
                needComponentMissingInfoCounting = 100;
            }
        }
        GUI.enabled = true;
        //必要组件不存在提示
        if (needComponentMissingInfoCounting > 0)
        {
            needComponentMissingInfoCounting--;
            GUILayout.BeginHorizontal();
            GUILayout.Label("请检查protoc.exe和table_writer.py是否存在", RedTextStyle);
            GUILayout.EndHorizontal();
        }
        #endregion

        #region 添加/清空 proto-Excel 对
        bool isChanged = false;
        GUILayout.BeginHorizontal();
        GUILayout.Label(string.Format("当前有 {0} 个proto-Excel对", PairList.Count), GUILayout.Width(200));
        if (GUILayout.Button("添加 proto-excel 对", GUILayout.Width(200)))
        {
            isChanged = true;
            PairList.Add(new BytePair());
        }
        GUILayout.EndHorizontal();
        #endregion

        #region 展示/移除 proto-Excel 对
        if (PairList.Count > 0)
        {
            _ScrollPos = GUILayout.BeginScrollView(_ScrollPos);
            for (int i = PairList.Count - 1; i >= 0; i--)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label((PairList.Count - i).ToString(), GUILayout.Width(32));
                if (GUILayout.Button("选择proto", GUILayout.Width(120)))
                {
                    protoFilePathTemp = EditorUtility.OpenFilePanelWithFilters("选择proto文件", ProtoFilePathBuff, new string[2] { "Proto文件", "proto" });
                    if (!string.IsNullOrEmpty(protoFilePathTemp) && File.Exists(protoFilePathTemp))
                    {
                        ProtoFilePathBuff = Path.GetDirectoryName(protoFilePathTemp);
                        PairList[i].ProtoPath = protoFilePathTemp;
                        isChanged = true;
                    }
                }
                PairList[i].ProtoPath = GUILayout.TextField(PairList[i].ProtoPath, GUILayout.Width(320));
                if (GUILayout.Button("选择excel", GUILayout.Width(120)))
                {
                    excelFilePathTemp = EditorUtility.OpenFilePanelWithFilters("选择excel文件", ExcelFilePathBuff, new string[2] { "Excel文件", "xls" });
                    if (!string.IsNullOrEmpty(excelFilePathTemp) && File.Exists(excelFilePathTemp))
                    {
                        ExcelFilePathBuff = Path.GetDirectoryName(excelFilePathTemp);
                        PairList[i].ExcelPath = excelFilePathTemp;
                        isChanged = true;
                    }
                }
                PairList[i].ExcelPath = GUILayout.TextField(PairList[i].ExcelPath, GUILayout.Width(320));
                if (GUILayout.Button("移除"))
                {
                    PairList.RemoveAt(i);
                    isChanged = true;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        if (isChanged)
        {
            SavePairInfo();
        }
        #endregion
    }

    private static List<BytePair> ReadPairInfo()
    {
        string str;
        List<BytePair> pair = new List<BytePair>();
        if (GetConfig("Pair", out str))
        {
            string[] strs = str.Split('|');
            for (int i = 0; i < strs.Length; i++)
            {
                if (string.IsNullOrEmpty(strs[i]))
                {
                    continue;
                }
                BytePair p = new BytePair();
                p.RefreshPath(strs[i]);
                pair.Add(p);
            }
        }
        return pair;
    }

    private static void SavePairInfo()
    {
        if (PairList != null)
        {
            StringBuilder sbtemp = new StringBuilder();
            for (int i = 0; i < PairList.Count; i++)
            {
                sbtemp.Append(PairList[i].ToString());
                sbtemp.Append("|");
            }
            SetConfig("Pair", sbtemp.ToString());
        }
    }

    private static bool IsRequiredComponentExist()
    {
        return File.Exists(ProtocFullPath) && File.Exists(TableWriterFullPath);
    }

    private static void GenerateBytes()
    {
        string outputDirectory = GetFullPath(defaultByteFilesPath, false);
        string tempDirectory = string.Empty;
        try
        {
            tempDirectory = GetTempDirectory();
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
            //复制 protoc.exe 和 table_writer.py 到临时文件夹
            string targetCopiedProtoFilePath = tempDirectory + "/protoc.exe";
            File.Copy(ProtocFullPath, targetCopiedProtoFilePath);
            string targetCopiedTableWriterFilePath = tempDirectory + "/table_writer.py";
            File.Copy(TableWriterFullPath, targetCopiedTableWriterFilePath);
            int successedCounting = 0;
            for (int i = 0; i < PairList.Count; i++)
            {
                EditorUtility.DisplayProgressBar("创建二进制文件", string.Format("正在解析 {0}  {1}", Path.GetFileName(PairList[i].ProtoPath), Path.GetFileName(PairList[i].ExcelPath)), ((float)i) / PairList.Count);
                if (PairList[i].GenerateByte(outputDirectory, tempDirectory))
                {
                    successedCounting++;
                }
            }
            UnityEngine.Debug.Log(string.Format("生成了{0}个二进制文件", successedCounting));
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }
    }
    #endregion

    #region 路径配置buffer
    private static string configPath;
    protected static string ConfigPath
    {
        get
        {
            if (configPath == null)
            {
                configPath = DefaultDirectory + "/ProjectSettings/Mini_ConfigForProto.txt";
            }
            return configPath;
        }
    }

    private static Dictionary<string, string> buffDic;
    protected static Dictionary<string, string> BuffDic
    {
        get
        {
            if (buffDic == null)
            {
                ReadConfigBuff();
            }
            return buffDic;
        }
    }

    protected static string DefaultTableFilePath
    {
        get
        {
            if (defaultTableFilePath == null && !GetConfig("Table", out defaultTableFilePath))
            {
                defaultTableFilePath = "MiniTable";
                SetConfig("Table", defaultTableFilePath);
            }
            return defaultTableFilePath;
        }
        set
        {
            if (defaultTableFilePath != value)
            {
                defaultTableFilePath = value;
                SetConfig("Table", defaultTableFilePath);
            }
        }
    }

    protected static string DefaultByteFilePath
    {
        get
        {
            if (defaultByteFilesPath == null && !GetConfig("Byte", out defaultByteFilesPath))
            {
                defaultByteFilesPath = "Resources/Bytes";
                SetConfig("Byte", defaultByteFilesPath);
            }
            return defaultByteFilesPath;
        }
        set
        {
            if (defaultByteFilesPath != value)
            {
                defaultByteFilesPath = value;
                SetConfig("Byte", defaultByteFilesPath);
            }
        }
    }

    private static bool GetConfig(string configKey, out string config)
    {
        return BuffDic.TryGetValue(configKey, out config);
    }

    private static void SetConfig(string configKey, string config)
    {
        if (BuffDic.ContainsKey(configKey))
        {
            BuffDic[configKey] = config;
        }
        else
        {
            BuffDic.Add(configKey, config);
        }
        SaveConfigBuff();
    }

    private static void ReadConfigBuff()
    {
        buffDic = new Dictionary<string, string>();
        if (File.Exists(ConfigPath))
        {
            string[] lines = File.ReadAllLines(ConfigPath);
            for (int i = 0; i < lines.Length; i++)
            {
                string singleLine = lines[i];
                string[] splitedLine = singleLine.Split(':');
                if (splitedLine != null && splitedLine.Length >= 2)
                {
                    if (!BuffDic.ContainsKey(splitedLine[0]))
                    {
                        BuffDic.Add(splitedLine[0], singleLine.Substring(splitedLine[0].Length + 1, singleLine.Length - splitedLine[0].Length - 1));
                    }
                }
            }
        }
    }

    private static void SaveConfigBuff()
    {
        StringBuilder sbtemp = new StringBuilder();
        foreach (var item in BuffDic)
        {
            sbtemp.Append(item.Key);
            sbtemp.Append(':');
            sbtemp.Append(item.Value);
            sbtemp.Append("\r\n");
        }
        if (File.Exists(ConfigPath))
        {
            File.Delete(ConfigPath);
        }
        File.WriteAllText(ConfigPath, sbtemp.ToString());
    }
    #endregion

    #region Tool
    private static string GetFullPath(string relativePath, bool removeAssetPrefix = false)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            return Application.dataPath;
        }
        if (relativePath.Length > 7 && removeAssetPrefix)
        {
            return Application.dataPath + "/" + relativePath.Substring(7, relativePath.Length - 7);
        }
        else
        {
            return Application.dataPath + "/" + relativePath;
        }
    }

    private static string GetTempDirectory()
    {
        string tempRes = ThisCSDirectoryPath + "/temp";
        string temp = tempRes;
        int index = 0;
        while (Directory.Exists(temp))
        {
            temp = tempRes + index.ToString();
            index++;
        }
        return temp;
    }

    private static string GetCurrentCSFileDirectory()
    {
        string classFilePath = GetSelfClassFilePath();
        string className = "\\" + typeof(DataTools_EditorForProto).ToString() + ".cs";
        string fileDirectory = classFilePath.Replace(className, string.Empty).Replace('\\', '/');
        return fileDirectory;
    }

    private static string GetSelfClassFilePath()
    {
        StackTrace st = new StackTrace(1, true);
        return st.GetFrame(0).GetFileName();
    }

    private static List<string> GetFullPathesForSelectedObjects(string postfix)
    {
        List<string> fullPathList = new List<string>();
        if (globalSelectedObjs != null && globalSelectedObjs.Length > 0)
        {
            for (int i = 0; i < globalSelectedObjs.Length; i++)
            {
                string relativePath = AssetDatabase.GetAssetPath(globalSelectedObjs[i]);
                if (!string.IsNullOrEmpty(relativePath) && relativePath.Length > postfix.Length + 7 && relativePath.Substring(relativePath.Length - postfix.Length, postfix.Length) == postfix)
                {
                    fullPathList.Add(GetFullPath(relativePath, true));
                }
            }
        }
        fullPathList.Sort((l, r) =>
        {
            if (l.Length == r.Length)
            {
                return l.CompareTo(r);
            }
            else
            {
                return l.Length > r.Length ? 1 : -1;
            }
        });
        return fullPathList;
    }

    private static bool CallProgress(string processName, string parameter, string runningDirectory)
    {
        bool isRunningInAnotherDirectory = !string.IsNullOrEmpty(runningDirectory);
        bool result = false;
        try
        {
            if (isRunningInAnotherDirectory)
            {
                Directory.SetCurrentDirectory(runningDirectory);
            }
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = processName,
                Arguments = parameter
            };
            Process process = Process.Start(startInfo);
            process.WaitForExit();
            result = true;
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError(ex.Message);
        }
        finally
        {
            if (isRunningInAnotherDirectory)
            {
                Directory.SetCurrentDirectory(DefaultDirectory);
            }
        }
        return result;
    }
    #endregion

    public class BytePair
    {
        private string protoPath = string.Empty;
        public string ProtoPath { get { return protoPath; } set { protoPath = value; } }

        private string excelPath = string.Empty;
        public string ExcelPath { get { return excelPath; } set { excelPath = value; } }

        public bool IsAbleToGenerateByte(bool logoutError = false)
        {
            if (logoutError)
            {
                StringBuilder sbtemp = new StringBuilder();
                bool protoState = string.IsNullOrEmpty(ProtoPath);
                if (protoState)
                {
                    sbtemp.Append(string.Format("Error:\r\n proto路径错误\r\n"));
                }
                bool excelState = string.IsNullOrEmpty(ExcelPath);
                if (excelState)
                {
                    sbtemp.Append(string.Format("Error:\r\n excel路径错误\r\n"));
                }
                if (protoState || excelState)
                {
                    UnityEngine.Debug.Log(sbtemp.ToString());
                    return false;
                }
                return true;
            }
            return !(string.IsNullOrEmpty(ProtoPath) || string.IsNullOrEmpty(ExcelPath));
        }

        /// <summary>
        /// 导出二进制文件
        /// </summary>
        /// <param name="outputDirectory">输出文件夹</param>
        /// <param name="tempDirectory">临时文件夹(用于临时存放.py文件,.pyc文件,导出的.byte文件等过程文件)</param>
        /// <returns></returns>
        public bool GenerateByte(string outputDirectory, string tempDirectory)
        {
            if (!IsAbleToGenerateByte(true))
            {
                return false;
            }
            if (!File.Exists(ProtoPath))
            {
                UnityEngine.Debug.LogError(string.Format("请检查proto文件 {0} 是否存在", ProtoPath));
                return false;
            }
            if (!File.Exists(ExcelPath))
            {
                UnityEngine.Debug.LogError(string.Format("请检查Excel文件 {0} 是否存在", ExcelPath));
                return false;
            }
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                if (!Directory.Exists(outputDirectory))
                {
                    UnityEngine.Debug.LogError(string.Format("无法创建目录: {0}", outputDirectory));
                    return false;
                }
            }
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
            string protoFileName = Path.GetFileName(ProtoPath);
            string excelPureName = Path.GetFileNameWithoutExtension(ExcelPath);
            string excelFileName = Path.GetFileName(ExcelPath);
            //将proto文件和excel复制到临时文件夹
            string tempProtoFilePath = tempDirectory + "/" + protoFileName;
            string tempExcelFilePath = tempDirectory + "/" + excelFileName;
            if (File.Exists(tempProtoFilePath))
            {
                File.Delete(tempProtoFilePath);
            }
            if (File.Exists(tempExcelFilePath))
            {
                File.Delete(tempExcelFilePath);
            }
            File.Copy(ProtoPath, tempProtoFilePath);
            File.Copy(ExcelPath, tempExcelFilePath);
            //运行protoc.exe,导出py文件
            if (!CallProgress("protoc", string.Format("--python_out=./ {0}", protoFileName), tempDirectory))
            {
                UnityEngine.Debug.Log("protoc.exe运行失败");
            }
            string pb2FilePath = tempDirectory + "/" + excelPureName + "_pb2.py";
            string originpb2FilePath = tempDirectory + "/" + Path.GetFileNameWithoutExtension(protoFileName) + "_pb2.py";
            if (File.Exists(originpb2FilePath) && !string.Equals(originpb2FilePath, pb2FilePath))
            {
                File.Copy(originpb2FilePath, pb2FilePath, true);
                File.Delete(originpb2FilePath);
            }
            //运行table_writer.py,导出bytes文件
            if (!CallProgress("python", string.Format(@"table_writer.py -c ""{0}""", excelPureName), tempDirectory))
            {
                UnityEngine.Debug.Log("table_writer.py执行失败");
            }
            //将运行得到的byte文件复制到目标文件夹
            string byteFileName = excelPureName.ToLower() + ".bytes";
            string tempByteFilePath = tempDirectory + "/" + byteFileName;
            string targetByteFilePath = outputDirectory + "/" + byteFileName;
            if (!File.Exists(tempByteFilePath))
            {
                UnityEngine.Debug.Log(string.Format("未生成 {0}", tempByteFilePath));
                return false;
            }
            File.Copy(tempByteFilePath, targetByteFilePath, true);
            UnityEngine.Debug.Log(string.Format("成功生成 {0}", targetByteFilePath));
            if (File.Exists(tempByteFilePath))
            {
                File.Delete(tempByteFilePath);
            }
            return true;
        }

        public override string ToString()
        {
            return string.Format("{0}---{1}", ProtoPath, ExcelPath);
        }

        public void RefreshPath(string pathinfo)
        {
            if (string.IsNullOrEmpty(pathinfo))
            {
                ProtoPath = string.Empty;
                ExcelPath = string.Empty;
                return;
            }
            string[] strs = pathinfo.Split(new string[1] { "---" }, 2, System.StringSplitOptions.None);
            if (strs.Length == 2)
            {
                ProtoPath = strs[0];
                ExcelPath = strs[1];
            }
        }
    }

    public class Proto
    {
        private static string pattern_Package = @"^package TABLE;";

        public MessageInProto lineMessage;
        public MessageInProto arrayMessage;

        private string protoContent;
        public string ProtoContent { get { return protoContent; } }

        private string protoFullPath;
        public string ProtoFullPath { get { return protoFullPath; } }

        public Proto(string protoFullPath)
        {
            this.protoFullPath = protoFullPath;
        }

        public Exception Deal(out bool isSuccess)
        {
            isSuccess = false;
            protoContent = File.ReadAllText(protoFullPath);
            if (string.IsNullOrEmpty(protoContent))
            {
                return new Exception(string.Format("{0}  proto文件内容为空", protoFullPath));
            }
            if (!Regex.IsMatch(protoContent, pattern_Package))
            {
                return new Exception(string.Format("{0}  proto的package设置错误,应设置为 package TABLE;", protoFullPath));
            }
            MatchCollection messageMatchCollection = Regex.Matches(protoContent, MessageInProto.pattern);
            IEnumerator messageMatchIEnumerator = messageMatchCollection.GetEnumerator();
            Match lineMatch = null;
            Match arrayMatch = null;
            int matchCount = 0;
            while (messageMatchIEnumerator.MoveNext())
            {
                matchCount++;
                if (matchCount == 1)
                {
                    lineMatch = messageMatchIEnumerator.Current as Match;
                }
                else if (matchCount == 2)
                {
                    arrayMatch = messageMatchIEnumerator.Current as Match;
                }
            }
            if (matchCount > 2)
            {
                return new Exception(string.Format("匹配上了 {0} 个message,请检查proto文件 {1}", matchCount, protoFullPath));
            }
            if (lineMatch == null || arrayMatch == null)
            {
                return new Exception(string.Format("proto文件消息缺失,请检查"));
            }
            lineMessage = new MessageInProto(lineMatch.Value);
            arrayMessage = new MessageInProto(arrayMatch.Value);
            if (!lineMessage.Deal())
            {
                return new Exception(string.Format("行消息定义有误!"));
            }
            if (!arrayMessage.Deal())
            {
                return new Exception(string.Format("数组消息定义有误!"));
            }
            //检查消息内容
            if (lineMessage.Variables.Length == 0)
            {
                return new Exception(string.Format("行消息变量数为0"));
            }
            if (string.IsNullOrEmpty(lineMessage.MessageName))
            {
                return new Exception(string.Format("行消息消息名为空"));
            }
            if (string.IsNullOrEmpty(arrayMessage.MessageName))
            {
                return new Exception(string.Format("数组消息消息名为空"));
            }
            if (!arrayMessage.MessageName.EndsWith("ARRAY"))
            {
                return new Exception(string.Format("数组消息命名应当以ARRAY结尾"));
            }
            if (arrayMessage.Variables.Length == 0)
            {
                return new Exception(string.Format("数组消息变量数为0"));
            }
            if (!(arrayMessage.Variables.Length == 1 //数组消息变量数为1
                && arrayMessage.Variables[0].Modifier == VariableInMessage.ModifierType.Repeated //数组消息变量应为数组格式
                && arrayMessage.Variables[0].Variable == VariableInMessage.VariableType.Others
                && arrayMessage.Variables[0].VariableTypeString == lineMessage.MessageName //数组消息变量应为行消息类型名
                && arrayMessage.Variables[0].VariableName == "rows" //数组消息变量名应为rows
                && arrayMessage.Variables[0].VariableIndex == 1)) //数组消息变量的编号应为1
            {
                return new Exception("数组消息变量应当满足 \"repeated + 行消息类型名 + rows = 1;\" 的格式!");
            }
            string errorMessage;
            if (!GenerateExcelFile(lineMessage, lineMessage.MessageName, out errorMessage))
            {
                return new Exception(string.Format("{0} proto文件处理失败 {1}", protoFullPath, errorMessage));
            }
            isSuccess = true;
            return new Exception(string.Format("{0} proto文件处理完毕", protoFullPath));
        }

        private bool GenerateExcelFile(MessageInProto protoLineMessage, string xlsFileName, out string errorMessage)
        {
            errorMessage = string.Empty;
            string xlsFilePath = Path.GetDirectoryName(protoFullPath) + '/' + xlsFileName + ".xls";
            if (File.Exists(xlsFilePath))
            {
                if (!EditorUtility.DisplayDialog("xls文件已存在", string.Format("是否替换 {0} 文件", xlsFilePath), "替换", "不替换"))
                {
                    errorMessage = string.Format("{0} 文件已存在", xlsFilePath);
                    return false;
                }
            }
            string messageName = protoLineMessage.MessageName;
            ExcelHelper excelHelper = new ExcelHelper(xlsFilePath);
            DataTable dt = new DataTable();
            List<ExcelHelper.Comment> comments = new List<ExcelHelper.Comment>();
            for (int i = 0; i < protoLineMessage.Variables.Length; i++)
            {
                if (protoLineMessage.Variables[i].Modifier != VariableInMessage.ModifierType.NULL && protoLineMessage.Variables[i].Modifier != VariableInMessage.ModifierType.Repeated)
                {
                    if (protoLineMessage.Variables[i].Variable == VariableInMessage.VariableType.Others)
                    {
                        EnumTypeInMessage enumType = protoLineMessage.GetEnumTypeByName(protoLineMessage.Variables[i].VariableTypeString);
                        if (enumType == null)
                        {
                            continue;
                        }
                        comments.Add(new ExcelHelper.Comment(0, i, enumType.ToString()));
                    }
                    dt.Columns.Add(protoLineMessage.Variables[i].VariableName);
                }
            }
            bool isFreeze = lineMessage.Enums.Length == 0;
            excelHelper.DataTableToExcel(dt, messageName, true, isFreeze, comments.ToArray());
            excelHelper.Dispose();
            return true;
        }
    }

    public class MessageInProto
    {
        public static string pattern = @"message((([^}]+)enum([^}]+)})*)([^}]+)\n}";

        private static string messageNamePattern = @"message\s+\w+";

        private string content;
        public string Content { get { return content; } }

        private string messageName;
        public string MessageName { get { return messageName; } }

        private EnumTypeInMessage[] enums;
        public EnumTypeInMessage[] Enums { get { return enums; } }

        private VariableInMessage[] variables;
        public VariableInMessage[] Variables { get { return variables; } }

        public MessageInProto(string messageContent)
        {
            content = messageContent;
        }

        public bool Deal()
        {
            return GetMessageName() && GetAllEnums() && GetAllVariables();
        }

        private bool GetMessageName()
        {
            Match messageNameMatch = Regex.Match(content, messageNamePattern);
            if (messageNameMatch.Success)
            {
                messageName = messageNameMatch.Value.Substring(7, messageNameMatch.Value.Length - 7).Trim();
                return true;
            }
            return false;
        }

        private bool GetAllEnums()
        {
            MatchCollection enumTypeCollection = Regex.Matches(content, EnumTypeInMessage.pattern);
            IEnumerator enumIEnumerator = enumTypeCollection.GetEnumerator();
            List<EnumTypeInMessage> enumList = new List<EnumTypeInMessage>();
            while (enumIEnumerator.MoveNext())
            {
                EnumTypeInMessage enumTemp = new EnumTypeInMessage((enumIEnumerator.Current as Match).Value);
                if (enumTemp.Deal())
                {
                    enumList.Add(enumTemp);
                }
            }
            enums = enumList.ToArray();
            return true;
        }

        private bool GetAllVariables()
        {
            MatchCollection variableCollection = Regex.Matches(content, VariableInMessage.pattern);
            IEnumerator variableIEnumerator = variableCollection.GetEnumerator();
            List<VariableInMessage> variableList = new List<VariableInMessage>();
            while (variableIEnumerator.MoveNext())
            {
                VariableInMessage variableTemp = new VariableInMessage((variableIEnumerator.Current as Match).Value);
                if (variableTemp.Deal())
                {
                    variableList.Add(variableTemp);
                }
            }
            variables = variableList.ToArray();
            return true;
        }

        public EnumTypeInMessage GetEnumTypeByName(string enumName)
        {
            if (enums == null)
            {
                return null;
            }
            for (int i = 0; i < enums.Length; i++)
            {
                if (enums[i].EnumName == enumName)
                {
                    return enums[i];
                }
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sbTemp = new StringBuilder();
            sbTemp.Append("Message ");
            sbTemp.Append(messageName);
            sbTemp.Append(":\n");
            if (enums.Length > 0)
            {
                sbTemp.Append("Enums:\n");
                for (int i = 0; i < enums.Length; i++)
                {
                    sbTemp.Append(enums[i].ToString());
                    sbTemp.Append("\n");
                }
            }
            if (variables.Length > 0)
            {
                sbTemp.Append("Variables:\n");
                for (int i = 0; i < variables.Length; i++)
                {
                    sbTemp.Append(variables[i].ToString());
                    sbTemp.Append("\n");
                }
            }
            return sbTemp.ToString();
        }
    }

    public class VariableInMessage
    {
        public static string pattern = @"(required|repeated|optional)\s+\w+\s+\w+\s*=\s*\d+\s*;";

        private static string indexPattern = @"=\s*\d+\s*";

        public enum ModifierType
        {
            Required,
            Optional,
            Repeated,
            NULL
        }

        public enum VariableType
        {
            Bool,
            Double,
            Float,
            Int32,
            UInt32,
            Int64,
            UInt64,
            SInt32,
            SInt64,
            Sing64,
            Fixed32,
            Fixed64,
            SFixed32,
            SFixed64,
            String,
            Bytes,
            Others
        }

        private ModifierType modifier;
        public ModifierType Modifier { get { return modifier; } }

        private VariableType variableType;
        public VariableType Variable { get { return variableType; } }

        private string variableTypeString;
        public string VariableTypeString { get { return variableTypeString; } }

        private string variableName;
        public string VariableName { get { return variableName; } }

        private int variableIndex;
        public int VariableIndex { get { return variableIndex; } }

        private string content;
        public string Content { get { return content; } }

        public VariableInMessage(string variableContent)
        {
            content = variableContent;
        }

        public bool Deal()
        {
            string modifierString = content.Substring(0, 8);
            modifier = GetModifierTypeByString(modifierString);
            string variableAndNameString = content.Substring(8, content.IndexOf('=') - 8).Trim();
            string[] strs = variableAndNameString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs != null && strs.Length == 2)
            {
                variableTypeString = strs[0];
                variableType = GetVariableTypeByString(variableTypeString);
                variableName = strs[1];
                Match indexMatch = Regex.Match(content, indexPattern);
                if (indexMatch.Success)
                {
                    return modifier != ModifierType.NULL && int.TryParse(indexMatch.Value.Substring(1, indexMatch.Value.Length - 1).Trim(), out variableIndex);
                }
            }
            return false;
        }

        private ModifierType GetModifierTypeByString(string modifierStr)
        {
            switch (modifierStr)
            {
                case "required": return ModifierType.Required;
                case "optional": return ModifierType.Optional;
                case "repeated": return ModifierType.Repeated;
                default: return ModifierType.NULL;
            }
        }

        private VariableType GetVariableTypeByString(string variableTypeStr)
        {
            switch (variableTypeStr)
            {
                case "bool": return VariableType.Bool;
                case "double": return VariableType.Double;
                case "float": return VariableType.Float;
                case "int32": return VariableType.Int32;
                case "uint32": return VariableType.UInt32;
                case "int64": return VariableType.Int64;
                case "uint64": return VariableType.UInt64;
                case "sint32": return VariableType.SInt32;
                case "sint64": return VariableType.SInt64;
                case "fixed32": return VariableType.Fixed32;
                case "fixed64": return VariableType.Fixed64;
                case "sfixed32": return VariableType.SFixed32;
                case "sfixed64": return VariableType.SFixed64;
                case "string": return VariableType.String;
                case "bytes": return VariableType.Bytes;
                case "enum":
                case "message":
                default: return VariableType.Others;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} = {3}", modifier.ToString(), variableType.ToString(), variableName, variableIndex.ToString());
        }
    }

    public class EnumTypeInMessage
    {
        public static string pattern = @"enum([^}]+)}";

        private static string singleEnumOptionPattern = @"\b\w+\s*=\s*\d+\s*;";
        private static string singleEnumNamePattern = @"enum\s*\w+";
        private static string singleEnumOption_GetOptionNamePattern = @"\w+";
        private static string singleEnumOption_GetOptionIndexPattern = @"=\s*\d+\s*";

        private string enumName;
        public string EnumName { get { return enumName; } }

        private EnumOption[] options;
        public EnumOption[] Options { get { return options; } }

        private string content;
        public string Content { get { return content; } }

        public EnumTypeInMessage(string enumContent)
        {
            content = enumContent;
        }

        public bool Deal()
        {
            MatchCollection enumNameMatchCollection = Regex.Matches(content, singleEnumNamePattern);
            IEnumerator enumNameMatchIEnum = enumNameMatchCollection.GetEnumerator();
            while (enumNameMatchIEnum.MoveNext())
            {
                Match matchTemp = enumNameMatchIEnum.Current as Match;
                string str = matchTemp.Value.Substring(4, matchTemp.Value.Length - 4);
                enumName = str.Trim();
            }
            MatchCollection optionMatchCollection = Regex.Matches(content, singleEnumOptionPattern);
            IEnumerator optionMatchIEnum = optionMatchCollection.GetEnumerator();
            List<EnumOption> optionList = new List<EnumOption>();
            while (optionMatchIEnum.MoveNext())
            {
                Match matchTemp = optionMatchIEnum.Current as Match;
                Match optionNameMatch = Regex.Match(matchTemp.Value, singleEnumOption_GetOptionNamePattern);
                Match indexMatch = Regex.Match(matchTemp.Value, singleEnumOption_GetOptionIndexPattern);
                if (optionNameMatch.Success && indexMatch.Success)
                {
                    EnumOption optionTemp = new EnumOption();
                    optionTemp.optionName = optionNameMatch.Value;
                    if (int.TryParse(indexMatch.Value.Substring(1, indexMatch.Value.Length - 1).Trim(), out optionTemp.optionIndex))
                    {
                        optionList.Add(optionTemp);
                    }
                }
            }
            options = optionList.ToArray();
            if (string.IsNullOrEmpty(enumName) || optionList.Count == 0)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder sbTemp = new StringBuilder();
            sbTemp.Append(enumName);
            sbTemp.Append("\n");
            sbTemp.Append("{");
            for (int i = 0; i < options.Length; i++)
            {
                sbTemp.Append("\n");
                sbTemp.Append("\t");
                sbTemp.Append(options[i].ToString());
            }
            sbTemp.Append("\n}");
            return sbTemp.ToString();
        }

        public struct EnumOption
        {
            public string optionName;
            public int optionIndex;

            public override string ToString()
            {
                return string.Format("{0} = {1};", optionName, optionIndex);
            }
        }
    }
}
