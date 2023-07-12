/****************************************************
*	文件：HttpWebRequestCreateEditorWindow.GenerateRequest.cs
*	作者：何明飞
*	邮箱：hemingfei@outlook.com
*	日期：2023/02/08 12:07:27
*	功能：暂无
*****************************************************/

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace CustomGameFramework.Editor
{
    /// <summary>
    ///     Description of HttpWebRequestCreateEditorWindow.GenerateRequest
    /// </summary>
    public partial class HttpWebRequestCreateEditorWindow
    {
        private void GenerateRequestData(string webRequestPath, string webRequestName, HTTPType webRequestType,
            bool modifyHeaders, bool modifyBody, bool modifyQuery,
            List<StringArgs> headers, List<MemberArgs> body, List<StringArgs> query, string urlId = "",
            string upTime = "")
        {
            if (!Directory.Exists(GenerateRootFolder + webRequestPath))
                Directory.CreateDirectory(GenerateRootFolder + webRequestPath);

            GenerateRequestData_cs(webRequestPath, webRequestName, webRequestType, modifyHeaders, modifyBody,
                modifyQuery, headers, body, query, urlId, upTime);

            GenerateRequestData_cs_send(webRequestPath, webRequestName, webRequestType, modifyHeaders, modifyBody,
                modifyQuery, headers, body, query, urlId, upTime);

            //GenerateRequestData_cs_create(webRequestPath, webRequestName, webRequestType, modifyHeaders, modifyBody,
            //    modifyQuery, headers, body, query);
        }

        private void GenerateRequestData_cs(string webRequestPath, string webRequestName, HTTPType webRequestType,
            bool modifyHeaders, bool modifyBody, bool modifyQuery,
            List<StringArgs> headers, List<MemberArgs> body, List<StringArgs> query, string urlId = "",
            string upTime = "")
        {
            var folderNames = webRequestPath.Split('/');
            var folderName = folderNames[^1];
            var dataName = string.Empty;
            for (var i = 0; i < folderNames.Length; i++)
            {
                if (string.IsNullOrEmpty(folderNames[i])) continue;

                var n = folderNames[i][0].ToString().ToUpper() + folderNames[i].Substring(1);
                dataName += n;
            }

            var exportPath = GenerateRootFolder + webRequestPath + "/" + folderName + ".cs";

            var requestType = webRequestType.ToString();

            #region 计算方法的参数字符

            var publicParams = "";
            var privateParams = "";

            {
                if (modifyHeaders)
                {
                    publicParams += "Dictionary<string, string> headers";
                    privateParams += "headers";
                }
                else
                {
                    privateParams += "null";
                }

                if (modifyBody)
                {
                    if (modifyHeaders) publicParams += ", ";

                    publicParams += "string bodyJson";
                    privateParams += ", bodyJson";
                }
                else
                {
                    privateParams += ", string.Empty";
                }

                if (modifyQuery)
                {
                    if (modifyHeaders || modifyBody) publicParams += ", ";

                    publicParams += "SortedDictionary<string, string> query";
                    privateParams += ", query";
                }
                else
                {
                    privateParams += ", null";
                }
            }

            #endregion

            #region 计算方法的详细参数

            var publicHeaderParam = "";
            var publicBodyParam = "";
            var publicQueryParam = "";

            var privateHeaderParam = "";
            var privateBodyParam = "";
            var privateQueryParam = "";

            var finalPublicParam = "";

            {
                if (modifyHeaders)
                {
                    var tmpIndex = 0;
                    var tmpLength = headers.Count;
                    foreach (var header in headers)
                    {
                        publicHeaderParam += $"string header_{header.Name}";
                        privateHeaderParam += $"header_{header.Name}";
                        if (tmpLength > 1 && tmpIndex < tmpLength - 1)
                        {
                            publicHeaderParam += ", ";
                            privateHeaderParam += ", ";
                        }

                        tmpIndex++;
                    }
                }

                if (modifyBody)
                {
                    var tmpIndex = 0;
                    var tmpLength = body.Count;
                    foreach (var b in body)
                    {
                        var typeString = b.Type;
                        if (string.IsNullOrEmpty(typeString)) typeString = "string";

                        if (typeString == "array") typeString = "List<string>";
                        publicBodyParam += $"{typeString} body_{b.Name}";
                        privateBodyParam += $"body_{b.Name}";
                        if (tmpLength > 1 && tmpIndex < tmpLength - 1)
                        {
                            publicBodyParam += ", ";
                            privateBodyParam += ", ";
                        }

                        tmpIndex++;
                    }
                }

                if (modifyQuery)
                {
                    var tmpIndex = 0;
                    var tmpLength = query.Count;
                    foreach (var q in query)
                    {
                        publicQueryParam += $"string query_{q.Name}";
                        privateQueryParam += $"query_{q.Name}";
                        if (tmpLength > 1 && tmpIndex < tmpLength - 1)
                        {
                            publicQueryParam += ", ";
                            privateQueryParam += ", ";
                        }

                        tmpIndex++;
                    }
                }
            }
            {
                if (modifyHeaders) finalPublicParam += publicHeaderParam;

                if (modifyBody)
                {
                    if (modifyHeaders) finalPublicParam += ", ";

                    finalPublicParam += publicBodyParam;
                }

                if (modifyQuery)
                {
                    if (modifyHeaders || modifyBody) finalPublicParam += ", ";

                    finalPublicParam += publicQueryParam;
                }
            }

            #endregion

            using (var txt = new FileStream(exportPath, FileMode.Create))
            using (var sw = new StreamWriter(txt))
            {
                var sb = new StringBuilder();
                sb.Append("using System.Collections.Generic;\n");
                sb.Append("using CustomGameFramework.Runtime;\n\n");
                sb.Append("namespace GameMain.WebRequest\n");
                sb.Append("{\n");
                sb.Append("\t/// <summary>\n");
                sb.Append($"\t/// {dataName}\n");
                sb.Append($"\t/// {webRequestName}\n");
                sb.Append("\t/// </summary>\n");
                sb.Append("\tpublic partial class HttpWebRequestMgr\n");
                sb.Append("\t{\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append($"\t\t/// {webRequestName}\n");
                sb.Append($"\t\t/// 接口路径：{webRequestPath}\n");
                sb.Append("\t\t/// </summary>\n");
                if (modifyHeaders)
                    foreach (var header in headers)
                        sb.Append($"\t\t/// <param name=\"header_{header.Name}\">{header.Des}</param>\n");

                if (modifyBody)
                    foreach (var b in body)
                        sb.Append($"\t\t/// <param name=\"body_{b.Name}\">{b.Des}</param>\n");

                if (modifyQuery)
                    foreach (var q in query)
                        sb.Append($"\t\t/// <param name=\"query_{q.Name}\">{q.Des}</param>\n");

                sb.Append("\t\t/// <returns>Web 请求任务的序列编号。</returns>\n");
                sb.Append($"\t\tpublic static int {requestType}_{dataName}({finalPublicParam})\n");
                sb.Append("\t\t{\n");
                if (modifyHeaders)
                    sb.Append(
                        $"\t\t\tDictionary<string, string> headers = {dataName}.GetHeaders({privateHeaderParam});\n");

                if (modifyBody) sb.Append($"\t\t\tstring bodyJson = {dataName}.GetBodyJson({privateBodyParam});\n");

                if (modifyQuery)
                    sb.Append(
                        $"\t\t\tSortedDictionary<string, string> query = {dataName}.GetQuery({privateQueryParam});\n");

                sb.Append($"\t\t\treturn Request_{dataName}({privateParams});\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append($"\t\t/// {webRequestName}\n");
                sb.Append($"\t\t/// 接口路径：{webRequestPath}\n");
                //sb.Append($"\t\t/// 接口更新时间：{upTime}\n");
                sb.Append("\t\t/// 需要手动输入headers、body等\n");
                sb.Append("\t\t/// </summary>\n");
                if (modifyHeaders)
                    sb.Append($"\t\t/// <param name=\"headers\">可以通过 {dataName}.GetHeaders 获取</param>\n");

                if (modifyBody) sb.Append($"\t\t/// <param name=\"bodyJson\">可以通过 {dataName}.GetBodyJson 获取</param>\n");

                if (modifyQuery) sb.Append($"\t\t/// <param name=\"query\">可以通过 {dataName}.GetQuery 获取</param>\n");

                sb.Append("\t\t/// <returns>Web 请求任务的序列编号。</returns>\n");
                sb.Append($"\t\tpublic static int {requestType}_{dataName}_Handle({publicParams})\n");
                sb.Append("\t\t{\n");
                sb.Append($"\t\t\treturn Request_{dataName}({privateParams});\n");
                sb.Append("\t\t}\n\n");
                
                sb.Append($"\t\tprivate static int Request_{dataName}(Dictionary<string, string> headers, string bodyJson, SortedDictionary<string, string> query)\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\theaders = AddCommonHeaders(headers);\n");
                sb.Append("\t\t\tquery = AddCommonQuery(query);\n");
                sb.Append($"\t\t\treturn WebRequest.AddWebRequest(UrlRoot + {dataName}.GetUrl(), HttpWebRequestInfo.Create({dataName}.GetUID(), {dataName}.GetEventId_Start(), {dataName}.GetEventId_Success(), {dataName}.GetEventId_Failure(), HttpWebRequestInfo.HTTPType.{requestType}, headers, bodyJson, query));\n");
                sb.Append("\t\t}\n");

                sb.Append("\t}\n");
                sb.Append("}\n");
                sw.Write(sb.ToString());
            }
        }

        private void GenerateRequestData_cs_send(string webRequestPath, string webRequestName, HTTPType webRequestType,
            bool modifyHeaders, bool modifyBody, bool modifyQuery,
            List<StringArgs> headers, List<MemberArgs> body, List<StringArgs> query, string urlId = "",
            string upTime = "")
        {
            var folderNames = webRequestPath.Split('/');
            var folderName = folderNames[^1];
            var dataName = string.Empty;
            for (var i = 0; i < folderNames.Length; i++)
            {
                if (string.IsNullOrEmpty(folderNames[i])) continue;

                var n = folderNames[i][0].ToString().ToUpper() + folderNames[i].Substring(1);
                dataName += n;
            }

            var exportPath = GenerateRootFolder + webRequestPath + "/" + folderName + ".send.cs";

            var requestType = webRequestType.ToString();

            using (var txt = new FileStream(exportPath, FileMode.Create))
            using (var sw = new StreamWriter(txt))
            {
                var sb = new StringBuilder();
                sb.Append("using System.Collections.Generic;\n");
                sb.Append("using GameFramework;\n");
                sb.Append("using LitJson;\n\n");
                sb.Append("namespace GameMain.WebRequest\n");
                sb.Append("{\n");
                sb.Append("\t/// <summary>\n");
                sb.Append($"\t/// {webRequestName}\n");
                sb.Append($"\t/// 接口路径：{webRequestPath}\n");
                //sb.Append($"\t/// 接口更新时间：{upTime}\n");
                sb.Append("\t/// </summary>\n");
                sb.Append($"\tpublic class {dataName}: IReference\n");
                sb.Append("\t{\n");

                sb.Append($"\t\tprivate const string URL = \"{webRequestPath}\";\n\n");

                var nodeString = webRequestPath.Replace('/', '.');
                nodeString = nodeString.Substring(1);
                sb.Append($"\t\tprivate const string NODE = \"{nodeString}\";\n\n");

                sb.Append(
                    $"\t\tprivate static readonly int EventId_Start = typeof({dataName}_Event_Start).GetHashCode();\n");
                sb.Append(
                    $"\t\tprivate static readonly int EventId_Success = typeof({dataName}_Event_Success).GetHashCode();\n");
                sb.Append(
                    $"\t\tprivate static readonly int EventId_Failure = typeof({dataName}_Event_Failure).GetHashCode();\n\n");

                sb.Append($"\t\tprivate class {dataName}_Event_Start{{}}\n");
                sb.Append($"\t\tprivate class {dataName}_Event_Success{{}}\n");
                sb.Append($"\t\tprivate class {dataName}_Event_Failure{{}}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 获取接口路径\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append("\t\tpublic static string GetUrl()\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\treturn URL;\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 获取唯一标识\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append("\t\tpublic static string GetUID()\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\treturn NODE;\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 获取事件Id Start\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append("\t\tpublic static int GetEventId_Start()\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\treturn EventId_Start;\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 获取事件Id Success\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append("\t\tpublic static int GetEventId_Success()\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\treturn EventId_Success;\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 获取事件Id Failure\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append("\t\tpublic static int GetEventId_Failure()\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\treturn EventId_Failure;\n");
                sb.Append("\t\t}\n\n");

                if (modifyHeaders)
                {
                    sb.Append("\t\t/// <summary>\n");
                    sb.Append("\t\t/// 获取Headers\n");
                    sb.Append("\t\t/// </summary>\n");
                    foreach (var header in headers)
                        sb.Append($"\t\t/// <param name=\"{header.Name}\">{header.Des}</param>\n");

                    sb.Append("\t\tpublic static Dictionary<string, string> GetHeaders(");
                    var headerIndex = 0;
                    var headerLength = headers.Count;
                    foreach (var header in headers)
                    {
                        sb.Append($"string {header.Name}");
                        if (headerLength > 1 && headerIndex < headerLength - 1) sb.Append(", ");

                        headerIndex++;
                    }

                    sb.Append(")\n");
                    sb.Append("\t\t{\n");
                    sb.Append("\t\t\tvar dict = new Dictionary<string, string>();\n");
                    foreach (var header in headers) sb.Append($"\t\t\tdict.Add(\"{header.Name}\", {header.Name});\n");

                    sb.Append("\t\t\treturn dict;\n");
                    sb.Append("\t\t}\n\n");
                }

                if (modifyBody)
                {
                    sb.Append("\t\t/// <summary>\n");
                    sb.Append("\t\t/// 获取BodyJson\n");
                    sb.Append("\t\t/// </summary>\n");
                    foreach (var b in body) sb.Append($"\t\t/// <param name=\"{b.Name}\">{b.Des}</param>\n");

                    sb.Append("\t\tpublic static string GetBodyJson(");
                    var bodyIndex = 0;
                    var bodyLength = body.Count;
                    foreach (var b in body)
                    {
                        var typeString = b.Type;
                        if (string.IsNullOrEmpty(typeString)) typeString = "string";

                        if (typeString == "array") typeString = "List<string>";

                        sb.Append($"{typeString} {b.Name}");
                        if (bodyLength > 1 && bodyIndex < bodyLength - 1) sb.Append(", ");

                        bodyIndex++;
                    }

                    sb.Append(")\n");
                    sb.Append("\t\t{\n");
                    sb.Append($"\t\t\tvar bodyClass = ReferencePool.Acquire<{dataName}>();\n");
                    foreach (var b in body) sb.Append($"\t\t\tbodyClass.{b.Name} = {b.Name};\n");

                    sb.Append("\t\t\tvar json = JsonMapper.ToJson(bodyClass);\n");
                    sb.Append("\t\t\tReferencePool.Release(bodyClass);\n");
                    sb.Append("\t\t\treturn json;\n");
                    sb.Append("\t\t}\n\n");
                }

                if (modifyQuery)
                {
                    sb.Append("\t\t/// <summary>\n");
                    sb.Append("\t\t/// 获取Query\n");
                    sb.Append("\t\t/// </summary>\n");
                    foreach (var q in query) sb.Append($"\t\t/// <param name=\"{q.Name}\">{q.Des}</param>\n");

                    sb.Append("\t\tpublic static SortedDictionary<string, string> GetQuery(");
                    var queryIndex = 0;
                    var queryLength = query.Count;
                    foreach (var q in query)
                    {
                        sb.Append($"string {q.Name}");
                        if (queryLength > 1 && queryIndex < queryLength - 1) sb.Append(", ");

                        queryIndex++;
                    }

                    sb.Append(")\n");
                    sb.Append("\t\t{\n");
                    sb.Append("\t\t\tvar dict = new SortedDictionary<string, string>();\n");
                    foreach (var q in query) sb.Append($"\t\t\tdict.Add(\"{q.Name}\", {q.Name});\n");

                    sb.Append("\t\t\treturn dict;\n");
                    sb.Append("\t\t}\n\n");
                }

                foreach (var b in body)
                {
                    sb.Append("\t\t/// <summary>\n");
                    sb.Append($"\t\t/// {b.Des}\n");
                    sb.Append("\t\t/// </summary>\n");
                    var typeString = b.Type;
                    if (string.IsNullOrEmpty(typeString)) typeString = "string";
                    if (typeString == "array") typeString = "List<string>";

                    sb.Append($"\t\tpublic {typeString} {b.Name};\n\n");
                }

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 初始化值\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append($"\t\tpublic {dataName}()\n");
                sb.Append("\t\t{\n");
                foreach (var b in body) sb.Append($"\t\t\t{b.Name} = default;\n");

                sb.Append("\t\t}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 清理重置\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append("\t\tpublic void Clear()\n");
                sb.Append("\t\t{\n");
                foreach (var b in body) sb.Append($"\t\t\t{b.Name} = default;\n");

                sb.Append("\t\t}\n");

                sb.Append("\t}\n");
                sb.Append("}\n");
                sw.Write(sb.ToString());
            }
        }

        private void GenerateRequestData_cs_create(string webRequestPath, string webRequestName,
            HTTPType webRequestType,
            bool modifyHeaders, bool modifyBody, bool modifyQuery,
            List<StringArgs> headers, List<MemberArgs> body, List<StringArgs> query,
            string resultDataType, string urlId = "", string upTime = "")
        {
            var folderNames = webRequestPath.Split('/');
            var folderName = folderNames[^1];
            var dataName = string.Empty;
            for (var i = 0; i < folderNames.Length; i++)
            {
                if (string.IsNullOrEmpty(folderNames[i])) continue;

                var n = folderNames[i][0].ToString().ToUpper() + folderNames[i].Substring(1);
                dataName += n;
            }

            var exportPath = GenerateRootFolder + webRequestPath + "/" + folderName + ".create.cs";

            var requestType = webRequestType.ToString();

            #region 计算方法的参数字符

            var publicParams = "";
            var privateParams = "";

            var publicInsideParam = "";

            {
                if (modifyHeaders)
                {
                    publicParams += "Dictionary<string, string> headers";
                    privateParams += "headers";
                    publicInsideParam += "headers";
                }
                else
                {
                    privateParams += "null";
                }

                if (modifyBody)
                {
                    if (modifyHeaders)
                    {
                        publicParams += ", ";
                        publicInsideParam += ", ";
                    }

                    publicParams += "string bodyJson";
                    privateParams += ", bodyJson";
                    publicInsideParam += "bodyJson";
                }
                else
                {
                    privateParams += ", string.Empty";
                }

                if (modifyQuery)
                {
                    if (modifyHeaders || modifyBody)
                    {
                        publicParams += ", ";
                        publicInsideParam += ", ";
                    }

                    publicParams += "SortedDictionary<string, string> query";
                    privateParams += ", query";
                    publicInsideParam += "query";
                }
                else
                {
                    privateParams += ", null";
                }
            }

            #endregion

            #region 计算方法的详细参数

            var publicHeaderParam = "";
            var publicBodyParam = "";
            var publicQueryParam = "";

            var privateHeaderParam = "";
            var privateBodyParam = "";
            var privateQueryParam = "";

            var finalPublicParam = "";
            var finalPublicInsideParam = "";

            {
                if (modifyHeaders)
                {
                    var tmpIndex = 0;
                    var tmpLength = headers.Count;
                    foreach (var header in headers)
                    {
                        publicHeaderParam += $"string header_{header.Name}";
                        privateHeaderParam += $"header_{header.Name}";
                        if (tmpLength > 1 && tmpIndex < tmpLength - 1)
                        {
                            publicHeaderParam += ", ";
                            privateHeaderParam += ", ";
                        }

                        tmpIndex++;
                    }
                }

                if (modifyBody)
                {
                    var tmpIndex = 0;
                    var tmpLength = body.Count;
                    foreach (var b in body)
                    {
                        var typeString = b.Type;
                        if (string.IsNullOrEmpty(typeString)) typeString = "string";

                        if (typeString == "array") typeString = "List<string>";
                        publicBodyParam += $"{typeString} body_{b.Name}";
                        privateBodyParam += $"body_{b.Name}";
                        if (tmpLength > 1 && tmpIndex < tmpLength - 1)
                        {
                            publicBodyParam += ", ";
                            privateBodyParam += ", ";
                        }

                        tmpIndex++;
                    }
                }

                if (modifyQuery)
                {
                    var tmpIndex = 0;
                    var tmpLength = query.Count;
                    foreach (var q in query)
                    {
                        publicQueryParam += $"string query_{q.Name}";
                        privateQueryParam += $"query_{q.Name}";
                        if (tmpLength > 1 && tmpIndex < tmpLength - 1)
                        {
                            publicQueryParam += ", ";
                            privateQueryParam += ", ";
                        }

                        tmpIndex++;
                    }
                }
            }
            {
                if (modifyHeaders)
                {
                    finalPublicParam += publicHeaderParam;
                    finalPublicInsideParam += privateHeaderParam;
                }

                if (modifyBody)
                {
                    if (modifyHeaders)
                    {
                        finalPublicParam += ", ";
                        finalPublicInsideParam += ", ";
                    }

                    finalPublicParam += publicBodyParam;
                    finalPublicInsideParam += privateBodyParam;
                }

                if (modifyQuery)
                {
                    if (modifyHeaders || modifyBody)
                    {
                        finalPublicParam += ", ";
                        finalPublicInsideParam += ", ";
                    }

                    finalPublicParam += publicQueryParam;
                    finalPublicInsideParam += privateQueryParam;
                }
            }

            #endregion

            using (var txt = new FileStream(exportPath, FileMode.Create))
            using (var sw = new StreamWriter(txt))
            {
                var sb = new StringBuilder();
                sb.Append("using System;\n");
                sb.Append("using System.Collections.Generic;\n");
                sb.Append("using CustomGameFramework.Runtime;\n");
                sb.Append("using GameFramework;\n");
                sb.Append("using GameFramework.Event;\n\n");
                sb.Append("namespace GameMain.WebRequest\n");
                sb.Append("{\n");
                sb.Append("\t/// <summary>\n");
                sb.Append($"\t/// 快速创建 {dataName}\n");
                sb.Append($"\t/// {webRequestName}\n");
                sb.Append($"\t/// 接口路径：{webRequestPath}\n");
                //sb.Append($"\t/// 接口更新时间：{upTime}\n");
                sb.Append("\t/// </summary>\n");
                sb.Append($"\tpublic class Create_{requestType}_{dataName} : IReference\n");
                sb.Append("\t{\n");

                var actionType = resultDataType;
                if (resultDataType is "object")
                    actionType = $"{dataName}ResultData";
                else if (resultDataType is "array") actionType = $"List<{dataName}ResultData>";

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 成功回调, resultData\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append($"\t\tprivate Action<{actionType}> _onSuccess_RESULT_{requestType}_{dataName};\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 失败回调，errorCode 和 errorMsg， errorCode -1为接口发送失败  -2为接口Json解析失败\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append($"\t\tprivate Action<string, string> _onFailure_RESULT_{requestType}_{dataName};\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 清除\n");
                sb.Append("\t\t/// </summary>\n");
                sb.Append("\t\tpublic void Clear()\n");
                sb.Append("\t\t{\n");
                sb.Append($"\t\t\t_onSuccess_RESULT_{requestType}_{dataName} = null;\n");
                sb.Append($"\t\t\t_onFailure_RESULT_{requestType}_{dataName} = null;\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 创建快速发送。\n");
                sb.Append($"\t\t/// {webRequestName}。\n");
                sb.Append($"\t\t/// 接口路径：{webRequestPath}\n");
                //sb.Append($"\t\t/// 接口更新时间：{upTime}\n");
                sb.Append("\t\t/// </summary>\n");
                if (modifyHeaders)
                    foreach (var header in headers)
                        sb.Append($"\t\t/// <param name=\"header_{header.Name}\">{header.Des}</param>\n");

                if (modifyBody)
                    foreach (var b in body)
                        sb.Append($"\t\t/// <param name=\"body_{b.Name}\">{b.Des}</param>\n");

                if (modifyQuery)
                    foreach (var q in query)
                        sb.Append($"\t\t/// <param name=\"query_{q.Name}\">{q.Des}</param>\n");

                sb.Append("\t\t/// <param name=\"onSuccess\">成功回调，resultData</param>\n");
                sb.Append(
                    "\t\t/// <param name=\"onFailure\">失败回调，errorCode 和 errorMsg， errorCode -1为接口发送失败  -2为接口Json解析失败</param>\n");
                sb.Append($"\t\tpublic static void Send({finalPublicParam}");
                if (!string.IsNullOrEmpty(finalPublicParam)) sb.Append(", ");

                sb.Append($"Action<{actionType}> onSuccess, Action<string, string> onFailure)\n");
                sb.Append("\t\t{\n");
                sb.Append(
                    $"\t\t\tReferencePool.Acquire<Create_{requestType}_{dataName}>().Create({finalPublicInsideParam}");
                if (!string.IsNullOrEmpty(finalPublicInsideParam)) sb.Append(", ");

                sb.Append("onSuccess, onFailure);\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t/// <summary>\n");
                sb.Append("\t\t/// 创建快速发送。\n");
                sb.Append($"\t\t/// {webRequestName}\n");
                sb.Append($"\t\t/// 接口路径：{webRequestPath}\n");
                //sb.Append($"\t\t/// 接口更新时间：{upTime}\n");
                sb.Append("\t\t/// 需要手动输入headers、body等\n");
                sb.Append("\t\t/// </summary>\n");
                if (modifyHeaders)
                    sb.Append($"\t\t/// <param name=\"headers\">可以通过 {dataName}.GetHeaders 获取</param>\n");

                if (modifyBody) sb.Append($"\t\t/// <param name=\"bodyJson\">可以通过 {dataName}.GetBodyJson 获取</param>\n");

                if (modifyQuery) sb.Append($"\t\t/// <param name=\"query\">可以通过 {dataName}.GetQuery 获取</param>\n");

                sb.Append("\t\t/// <param name=\"onSuccess\">成功回调，resultData</param>\n");
                sb.Append(
                    "\t\t/// <param name=\"onFailure\">失败回调，errorCode 和 errorMsg， errorCode -1为接口发送失败  -2为接口Json解析失败</param>\n");
                sb.Append($"\t\tpublic static void Send_Handle({publicParams}");
                if (!string.IsNullOrEmpty(publicParams)) sb.Append(", ");


                sb.Append($"Action<{actionType}> onSuccess, Action<string, string> onFailure)\n");
                sb.Append("\t\t{\n");
                sb.Append(
                    $"\t\t\tReferencePool.Acquire<Create_{requestType}_{dataName}>().Create_Handle({publicInsideParam}");
                if (!string.IsNullOrEmpty(publicInsideParam)) sb.Append(", ");

                sb.Append("onSuccess, onFailure);\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t// 创建发送，注册事件等。\n");
                sb.Append($"\t\tprivate void Create({finalPublicParam}");
                if (!string.IsNullOrEmpty(finalPublicParam)) sb.Append(", ");

                sb.Append($"Action<{actionType}> onSuccess, Action<string, string> onFailure)\n");
                sb.Append("\t\t{\n");
                sb.Append($"\t\t\t_onSuccess_RESULT_{requestType}_{dataName} = onSuccess;\n");
                sb.Append($"\t\t\t_onFailure_RESULT_{requestType}_{dataName} = onFailure;\n");
                sb.Append(
                    $"\t\t\tHttpWebRequestMgr.Event.Subscribe({dataName}.GetEventId_Success(), OnHttpWebRequest_{dataName}_Success);\n");
                sb.Append(
                    $"\t\t\tHttpWebRequestMgr.Event.Subscribe({dataName}.GetEventId_Failure(), OnHttpWebRequest_{dataName}_Failure);\n");
                sb.Append($"\t\t\tHttpWebRequestMgr.{requestType}_{dataName}({finalPublicInsideParam});\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t// 创建发送，注册事件等。\n");
                sb.Append($"\t\tprivate void Create_Handle({publicParams}");
                if (!string.IsNullOrEmpty(publicParams)) sb.Append(", ");

                sb.Append($"Action<{actionType}> onSuccess, Action<string, string> onFailure)\n");
                sb.Append("\t\t{\n");
                sb.Append($"\t\t\t_onSuccess_RESULT_{requestType}_{dataName} = onSuccess;\n");
                sb.Append($"\t\t\t_onFailure_RESULT_{requestType}_{dataName} = onFailure;\n");
                sb.Append(
                    $"\t\t\tHttpWebRequestMgr.Event.Subscribe({dataName}.GetEventId_Success(), OnHttpWebRequest_{dataName}_Success);\n");
                sb.Append(
                    $"\t\t\tHttpWebRequestMgr.Event.Subscribe({dataName}.GetEventId_Failure(), OnHttpWebRequest_{dataName}_Failure);\n");
                sb.Append($"\t\t\tHttpWebRequestMgr.{requestType}_{dataName}_Handle({publicInsideParam});\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t// 网络请求成功事件\n");
                sb.Append($"\t\tprivate void OnHttpWebRequest_{dataName}_Success(object sender, GameEventArgs e)\n");
                sb.Append("\t\t{\n");
                sb.Append($"\t\t\tvar returnData = HttpWebRequestMgr.HttpWebRequest.GetReturnData<HttpMsgReturnEncryptData>({dataName}.GetUID());\n");
                sb.Append("\t\t\tif (returnData != null && returnData.IsSuccess())\n");
                sb.Append("\t\t\t{\n");
                sb.Append($"\t\t\t\tstring encryptResultData = returnData.resultData;\n");
                sb.Append($"\t\t\t\tstring resultJsonData = HttpWebRequestMgr.DecryptResultData(encryptResultData);\n");

                sb.Append($"\t\t\t\tvar resultData = Utility.Json.ToObject<{actionType}>(resultJsonData);\n");
                sb.Append("\t\t\t\tOnSuccess(resultData);\n");
                sb.Append("\t\t\t}\n");
                sb.Append("\t\t\telse\n");
                sb.Append("\t\t\t{\n");
                sb.Append("\t\t\t\tif (returnData == null)\n");
                sb.Append("\t\t\t\t{\n");
                sb.Append("\t\t\t\t\tOnFailure(\"-2\", \"接口映射错误\");\n");
                sb.Append("\t\t\t\t}\n");
                sb.Append("\t\t\t\telse\n");
                sb.Append("\t\t\t\t{\n");
                sb.Append("\t\t\t\t\tOnFailure(returnData.returnCode, returnData.message);\n");
                sb.Append("\t\t\t\t}\n");
                sb.Append("\t\t\t}\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t// 网络请求失败事件\n");
                sb.Append($"\t\tprivate void OnHttpWebRequest_{dataName}_Failure(object sender, GameEventArgs e)\n");
                sb.Append("\t\t{\n");
                sb.Append("\t\t\tvar args = (HttpWebRequestFailureEventArgs)e;\n");
                sb.Append("\t\t\tOnFailure(\"-1\", args.ErrorMessage);\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t// 成功获取 ResultData\n");
                sb.Append($"\t\tprivate void OnSuccess({actionType} resultData)\n");
                sb.Append("\t\t{\n");
                sb.Append(
                    $"\t\t\tHttpWebRequestMgr.Event.Unsubscribe({dataName}.GetEventId_Success(), OnHttpWebRequest_{dataName}_Success);\n");
                sb.Append(
                    $"\t\t\tHttpWebRequestMgr.Event.Unsubscribe({dataName}.GetEventId_Failure(), OnHttpWebRequest_{dataName}_Failure);\n");
                sb.Append($"\t\t\t_onSuccess_RESULT_{requestType}_{dataName}?.Invoke(resultData);\n");
                sb.Append("\t\t\tReferencePool.Release(this);\n");
                sb.Append("\t\t}\n\n");

                sb.Append("\t\t// 未获取 ResultData\n");
                sb.Append("\t\tprivate void OnFailure(string errorCode, string errorMessage)\n");
                sb.Append("\t\t{\n");
                sb.Append(
                    $"\t\t\tHttpWebRequestMgr.Event.Unsubscribe({dataName}.GetEventId_Success(), OnHttpWebRequest_{dataName}_Success);\n");
                sb.Append(
                    $"\t\t\tHttpWebRequestMgr.Event.Unsubscribe({dataName}.GetEventId_Failure(), OnHttpWebRequest_{dataName}_Failure);\n");
                sb.Append($"\t\t\t_onFailure_RESULT_{requestType}_{dataName}?.Invoke(errorCode, errorMessage);\n");
                sb.Append("\t\t\tReferencePool.Release(this);\n");
                sb.Append("\t\t}\n");

                sb.Append("\t}\n");
                sb.Append("}\n");
                sw.Write(sb.ToString());
            }
        }

        public static void GenerateRequestData_data(string webRequestPath, string webRequestName,
            HTTPType webRequestType,
            bool modifyHeaders, bool modifyBody, bool modifyQuery,
            List<StringArgs> headers, List<MemberArgs> body, List<StringArgs> query,
            string urlId = "", string upTime = "")
        {
            var data = new RequestData_data();
            data.webRequestPath = webRequestPath;
            data.webRequestName = webRequestName;
            data.webRequestType = webRequestType;
            data.modifyHeaders = modifyHeaders;
            data.modifyBody = modifyBody;
            data.modifyQuery = modifyQuery;
            data.headers = headers;
            data.body = body;
            data.query = query;
            data.urlId = urlId;
            data.upTime = upTime;
            var dataJson = JsonUtility.ToJson(data);
            var folderNames = webRequestPath.Split('/');
            var folderName = folderNames[^1];
            if (!Directory.Exists(GenerateRootFolder + webRequestPath))
                Directory.CreateDirectory(GenerateRootFolder + webRequestPath);

            var exportPath = GenerateRootFolder + webRequestPath + "/" + folderName + ".data.send.data";
            using (var txt = new FileStream(exportPath, FileMode.Create))
            using (var sw = new StreamWriter(txt))
            {
                var sb = new StringBuilder();
                sb.Append(dataJson);
                sw.Write(sb.ToString());
            }
        }

        public sealed class RequestData_data
        {
            public List<MemberArgs> body;
            public List<StringArgs> headers;
            public bool modifyBody;
            public bool modifyHeaders;
            public bool modifyQuery;
            public List<StringArgs> query;
            public string upTime;
            public string urlId;
            public string webRequestName;
            public string webRequestPath;
            public HTTPType webRequestType;
        }
    }
}