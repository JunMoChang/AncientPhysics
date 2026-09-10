using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Script.NPC
{
    public class NpcDialogueManager : MonoBehaviour
    {
        private string apiUrl = "https://api.deepseek.com/v1/chat/completions";

        // 从 StreamingAssets/config.json 读取，不要硬编码进仓库
        // 文件格式：{ "deepSeekApiKey": "sk-xxxxxxxx" }
        private string apiKey;

        [System.Serializable]
        private class LocalConfig
        {
            public string deepSeekApiKey;
        }

        private void Awake()
        {
            apiKey = LoadApiKey();
        }

        private static string LoadApiKey()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "config.json");
            if (!File.Exists(path))
            {
                Debug.LogWarning("未找到 " + path + "，NPC 对话功能不可用。");
                return string.Empty;
            }

            try
            {
                LocalConfig config = JsonConvert.DeserializeObject<LocalConfig>(File.ReadAllText(path));
                return config?.deepSeekApiKey ?? string.Empty;
            }
            catch (System.Exception e)
            {
                Debug.LogError("读取 config.json 失败: " + e.Message);
                return string.Empty;
            }
        }

        // NPC角色设定（核心！）
        private string systemPrompt = 
            "你扮演明朝科学家宋应星，精通《天工开物》。\n" +
            "若问题超纲则答：“天道幽微，非人力尽窥也”";

        public IEnumerator SendChatRequest(string playerMessage, System.Action<string> callback)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("未配置 DeepSeek API Key（StreamingAssets/config.json），跳过请求。");
                yield break;
            }

            // 构建请求数据
            var requestData = new
            {
                model = "deepseek-chat",
                messages = new[] {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = playerMessage }
                }
            };
            string jsonBody = JsonConvert.SerializeObject(requestData);

            // 发送请求
            using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + apiKey);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    // 解析返回的JSON
                    var response = JsonConvert.DeserializeObject<DeepSeekResponse>(request.downloadHandler.text);
                    string npcReply = response.choices[0].message.content;
                    callback(npcReply);
                }
                else
                {
                    Debug.LogError("API请求失败: " + request.error);
                }
            }
        }

        // DeepSeek API返回结构
        private class DeepSeekResponse
        {
            public Choice[] choices;
        }
        private class Choice
        {
            public Message message;
        }
        private class Message
        {
            public string content;
        }
    }
}
