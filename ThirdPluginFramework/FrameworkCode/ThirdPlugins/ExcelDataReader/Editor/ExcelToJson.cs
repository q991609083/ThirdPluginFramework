#if UNITY_EDITOR 
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class ExcelToJson : Editor
{
    public static string globalJsonString = "{";

    private static int _totalData = 0;

    private static int _currentData = 0;
    /// <summary>
    /// Excel转Jsoun并保存Json
    /// </summary>
    [MenuItem("Assets/JQ Tools/ExcelToJson")]
    static void DoXlsxToJson()
    {
        _totalData = 0;
        _currentData = 0;
        List<string> names = new List<string>();
        var o = Selection.activeObject;
        if (o == null)
        {
            EditorUtility.DisplayDialog("错误","未选择文件", "OK");
            return;
        }
        string filePath = AssetDatabase.GetAssetPath(o);
        if (filePath.Split('.').Length == 1)
        {
            EditorUtility.DisplayDialog("错误", "未选择文件", "OK");
            return;
        }
        if (filePath.Split('.')[1] != "xlsx")
        {
            EditorUtility.DisplayDialog("错误", "选择的文件非Excel表格文件", "OK");
            return;
        }
        ReadSingleExcel(filePath);
    }
    /// <summary>
    /// 读取Excel文件
    /// </summary>
    /// <param name="xlsxPath"></param>
    /// <param name="jsonPath"></param>
    public static void ReadSingleExcel(string xlsxPath, string jsonPath = null)
    {
        globalJsonString = "{";
        string dataPath = Application.dataPath;

        if (jsonPath == null)
        {
            jsonPath = dataPath + "/StreamingAssets/ConfigJsons";
            if (!Directory.Exists(jsonPath))
                Directory.CreateDirectory(jsonPath);
        }
        string xlsxName = FileTool.GetFileName(xlsxPath);

        if (!File.Exists(xlsxPath))
        {
            EditorUtility.DisplayDialog("错误","Excel文件不存在","OK");
            return;
        }

        using (Stream stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read))
        {
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();


            DataTableCollection tc = result.Tables;
            //获取总共元素个数,用来计算进度
            foreach (var table in result.Tables)
            {
                int rows = (table as DataTable).Rows.Count - 3;
                int Columns = (table as DataTable).Columns.Count;
                _totalData += rows * Columns;
            }
            //开始按照Sheet读取
            for (int i = 0; i < tc.Count; i++)
            {
                if (i != 0)
                {
                    globalJsonString += ",";
                }
                ReadSingleSheet(result.Tables[i], jsonPath);
            }
            globalJsonString += "}";
            FileTool.SaveFile(globalJsonString, jsonPath + "/game.json");


            var rootObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigData>(globalJsonString);
            var fs = new FileStream(jsonPath + "/game.bin", FileMode.OpenOrCreate);

            var formatter = new BinaryFormatter();

            formatter.Serialize(fs, rootObject);
            fs.Close();
            EditorUtility.ClearProgressBar();
            FileTool.DeleteFile(jsonPath + "/game.json");
            EditorUtility.DisplayDialog("提示", "配置表转换成功", "OK");
        }
    }
    /// <summary>
    /// 读取一个工作表的数据
    /// </summary>
    /// <param name="type">要转换的struct或class类型</param>
    /// <param name="dataTable">读取的工作表数据</param>
    /// <param name="jsonPath">存储路径</param>
    private static void ReadSingleSheet(System.Data.DataTable dataTable, string jsonPath)
    {
        Assembly assembly = Assembly.LoadFile(Environment.CurrentDirectory + @"\Library\ScriptAssemblies\Assembly-CSharp.dll"); // 获取当前程序集 
        Type type = assembly.GetType(dataTable.TableName); // 创建类的实例，返回为 object 类型，需要强制类型转换

        int rows = dataTable.Rows.Count;
        int Columns = dataTable.Columns.Count;
        // 工作表的行数据
        DataRowCollection collect = dataTable.Rows;
        // xlsx对应的数据字段，规定是第二行
        string[] jsonFileds = new string[Columns];
        // 要保存成Json的obj
        List<object> objsToSave = new List<object>();
        for (int i = 0; i < Columns; i++)
        {
            jsonFileds[i] = collect[1][i].ToString();
        }
        // 从第三行开始
        for (int i = 3; i < rows; i++)
        {
            // 生成一个实例
            object objIns = Activator.CreateInstance(type);

            for (int j = 0; j < Columns; j++)
            {
                _currentData += 1;
                string displayInfo = "SheetName : " + dataTable.TableName + " 第" + (i + 1) + "行" + "Title 为" + collect[1][j].ToString();
                EditorUtility.DisplayProgressBar("配置表转换", displayInfo, (float)_currentData / (float)_totalData);
                if (collect[1][j].ToString() == "")
                {
                    continue;
                }
                try
                {
                    // 获取字段
                    FieldInfo field = type.GetField(jsonFileds[j]);
                    if (field != null)
                    {
                        object value = null;
                        try // 赋值
                        {
                            if (field.FieldType.IsEnum)
                            {
                                value = Convert.ChangeType(collect[i][j], typeof(int));
                            }
                            else
                            {
                                value = Convert.ChangeType(collect[i][j], field.FieldType);
                            }
                        }
                        catch (InvalidCastException e) // 一般到这都是Int数组，当然还可以更细致的处理不同类型的数组
                        {
                            string str = collect[i][j].ToString();
                            if (str.IndexOf('|') != -1)
                            {
                                string[] strs = str.Split('|');
                                string[] arr = new string[strs.Length - 1];
                                for (int k = 0; k < strs.Length - 1; k++)
                                {
                                    arr[k] = strs[k];
                                }
                                value = arr;

                            }
                            else
                            {
                                //有小数点，则为float类型
                                if (str.IndexOf('.') != -1)
                                {
                                    string[] strs = str.Split(',');
                                    float[] floats = new float[strs.Length];
                                    for (int k = 0; k < strs.Length; k++)
                                    {
                                        floats[k] = Convert.ToSingle(strs[k]);
                                    }
                                    value = floats;
                                }
                                //无小数点，则为int类型
                                else
                                {
                                    string[] strs = str.Split(',');
                                    int[] ints = new int[strs.Length];
                                    for (int k = 0; k < strs.Length; k++)
                                    {
                                        ints[k] = Convert.ToInt32(strs[k]);
                                    }
                                    value = ints;
                                }

                            }
                        }

                        field.SetValue(objIns, value);
                    }
                    else
                    {
                        UnityEngine.Debug.LogFormat("有无法识别的字符串：{0}", displayInfo);
                    }
                }catch(InvalidCastException e)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("错误", displayInfo + "出错", "OK");
                }
            }
            objsToSave.Add(objIns);
        }
        string content = Newtonsoft.Json.JsonConvert.SerializeObject(objsToSave);

        globalJsonString += "\"" + dataTable.TableName + "\":" + content; 
    }

}
/// <summary>
/// 文件工具类
/// </summary>
public class FileTool
{
    /// <summary>
    /// 根据路径获取文件名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileName(string path)
    {
        string[] fs = path.Split('/');
        string file = fs[fs.Length - 1];
        string name = file.Split('.')[0];
        return name;
    }
    /// <summary>
    /// 根据路径获取后缀名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileSuffix(string path)
    {
        return path.Split('.')[1];
    }
    /// <summary>
    /// 将内容保存到指定路径
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="Path">路径</param>
    public static void SaveFile(string content, string Path)
    {
        if (File.Exists(Path))
        {
            File.Delete(Path);
        }
        File.WriteAllBytes(Path, Encoding.UTF8.GetBytes(content));
    }

    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

#endif