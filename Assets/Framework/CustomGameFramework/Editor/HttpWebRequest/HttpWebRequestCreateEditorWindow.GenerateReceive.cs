/****************************************************
*	文件：HttpWebRequestCreateEditorWindow.GenerateReceive.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/08 12:07:40
*	功能：暂无
*****************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    /// <summary>
    ///     Description of HttpWebRequestCreateEditorWindow.GenerateReceive
    /// </summary>
    public partial class HttpWebRequestCreateEditorWindow
    {
        private void GenerateReturnData(string webRequestPath, List<MemberArgs> resultData,
            List<ClassArgs> resultInnerData,
            string resultDataType, string urlId = "", string upTime = "")
        {
            if (!Directory.Exists(GenerateRootFolder + webRequestPath))
                Directory.CreateDirectory(GenerateRootFolder + webRequestPath);

            var folderNames = webRequestPath.Split('/');
            var folderName = folderNames[^1];
            var dataName = string.Empty;
            for (var i = 0; i < folderNames.Length; i++)
            {
                if (string.IsNullOrEmpty(folderNames[i])) continue;
                var n = folderNames[i][0].ToString().ToUpper() + folderNames[i].Substring(1);
                dataName += n;
            }

            var exportPath = GenerateRootFolder + webRequestPath + "/" + folderName + ".receive.cs";

            using (var txt = new FileStream(exportPath, FileMode.Create))
            using (var sw = new StreamWriter(txt))
            {
                var sb = new StringBuilder();
                sb.Append("using System.Collections.Generic;\n\n");
                sb.Append("namespace GameMain.WebRequest\n");
                sb.Append("{\n");

                sb.Append("\t/// <summary>\n");
                sb.Append("\t/// 接口返回的数据\n");
                sb.Append($"\t/// 接口路径：{webRequestPath}\n");
                //sb.Append($"\t/// 接口更新时间：{upTime}\n");
                sb.Append("\t/// </summary>\n");
                sb.Append($"\tpublic class {dataName}ReturnData : HttpMsgReturnBaseData\n");
                sb.Append("\t{\n");
                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 结果数据\n");
                sb.Append("\t\t/// </summary>\n");
                if (resultDataType == "object")
                    sb.Append($"\t\tpublic {dataName}ResultData resultData;\n");
                else if (resultDataType == "array")
                    sb.Append($"\t\tpublic List<{dataName}ResultData> resultData;\n");
                else
                    sb.Append($"\t\tpublic {resultDataType} resultData;\n");

                sb.Append("\t}\n\n");

                if (resultDataType is "object" or "array")
                {
                    sb.Append("\t/// <summary>\n");
                    sb.Append("\t/// 接口返回的 resultData 结果数据\n");
                    sb.Append($"\t/// 接口路径：{webRequestPath}\n");
                    //sb.Append($"\t/// 接口更新时间：{upTime}\n");
                    sb.Append("\t/// </summary>\n");
                    sb.Append($"\tpublic class {dataName}ResultData\n");
                    sb.Append("\t{\n");
                    foreach (var result in resultData)
                    {
                        sb.Append("\t\t/// <summary>\n");
                        sb.Append($"\t\t/// {result.Des}\n");
                        sb.Append("\t\t/// </summary>\n");
                        sb.Append($"\t\tpublic {result.Type} {result.Name};\n\n");
                    }

                    sb.Append("\t}\n\n");
                }


                foreach (var resultInner in resultInnerData)
                {
                    sb.Append("\t/// <summary>\n");
                    sb.Append("\t/// 接口返回的 resultData 结果数据里的自定义类型\n");
                    sb.Append($"\t/// 接口路径：{webRequestPath}\n");
                    sb.Append("\t/// </summary>\n");
                    sb.Append($"\tpublic class {resultInner.TypeName}\n");
                    sb.Append("\t{\n");
                    foreach (var member in resultInner.Members)
                    {
                        sb.Append("\t\t/// <summary>\n");
                        sb.Append($"\t\t/// {member.Des}\n");
                        sb.Append("\t\t/// </summary>\n");
                        sb.Append($"\t\tpublic {member.Type} {member.Name};\n\n");
                    }

                    sb.Append("\t}\n\n");
                }

                sb.Append("}\n");
                sw.Write(sb.ToString());
            }
        }

        public static void GenerateReturnData_data(string webRequestPath, List<MemberArgs> resultData,
            List<ClassArgs> resultInnerData,
            string resultType = "object", string urlId = "", string upTime = "")
        {
            var data = new ReturnData_data();
            data.webRequestPath = webRequestPath;
            data.resultData = resultData;
            data.resultInnerData = resultInnerData;
            data.resultType = resultType;
            data.urlId = urlId;
            data.upTime = upTime;
            var dataJson = JsonUtility.ToJson(data);
            var folderNames = webRequestPath.Split('/');
            var folderName = folderNames[^1];
            var exportPath = GenerateRootFolder + webRequestPath + "/" + folderName + ".data.receive.data";
            using (var txt = new FileStream(exportPath, FileMode.Create))
            using (var sw = new StreamWriter(txt))
            {
                var sb = new StringBuilder();
                sb.Append(dataJson);
                sw.Write(sb.ToString());
            }
        }

        public sealed class ReturnData_data
        {
            public List<MemberArgs> resultData;
            public List<ClassArgs> resultInnerData;
            public string resultType;
            public string upTime;
            public string urlId;
            public string webRequestPath;
        }
    }
}