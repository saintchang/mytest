using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BpmEngine.Core
{
    // --- 基礎定義區 (與 JSON 結構對應) ---

    public enum ActivityType { Start, UserTask, End }

    public class Activity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ActivityType Type { get; set; }
        
        [JsonPropertyName("assigneeRole")]
        public string AssigneeRole { get; set; } 
    }

    public class Transition
    {
        [JsonPropertyName("from")]
        public string FromNodeId { get; set; }
        
        [JsonPropertyName("to")]
        public string ToNodeId { get; set; }
        
        [JsonPropertyName("condition")]
        public string Condition { get; set; } 
    }

    public class WorkflowDefinition
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("activities")]
        public List<Activity> Activities { get; set; } = new List<Activity>();
        
        [JsonPropertyName("transitions")]
        public List<Transition> Transitions { get; set; } = new List<Transition>();

        // 靜態方法：從 JSON 轉換為物件
        public static WorkflowDefinition FromJson(string json)
        {
            return JsonSerializer.Deserialize<WorkflowDefinition>(json);
        }
    }

    // --- 執行期物件 ---

    public class WorkflowInstance
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DefinitionId { get; set; }
        public string CurrentNodeId { get; set; }
        public string Status { get; set; } = "Running";
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
    }

    public class WorkflowTask
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InstanceId { get; set; }
        public string NodeId { get; set; }
        public string Assignee { get; set; }
        public bool IsCompleted { get; set; }
    }

    // --- 引擎核心 ---

    public class SimpleWorkflowEngine
    {
        private readonly List<WorkflowDefinition> _definitions = new List<WorkflowDefinition>();
        private readonly List<WorkflowInstance> _instances = new List<WorkflowInstance>();
        private readonly List<WorkflowTask> _tasks = new List<WorkflowTask>();

        /// <summary>
        /// 透過 JSON 字串部署流程
        /// </summary>
        public void Deploy(string jsonDefinition)
        {
            var def = WorkflowDefinition.FromJson(jsonDefinition);
            _definitions.Add(def);
            Console.WriteLine($"[部署] 成功部署流程: {def.Name} (ID: {def.Id})");
        }

        public WorkflowInstance StartProcess(string definitionId, Dictionary<string, object> initialVariables = null)
        {
            var def = _definitions.FirstOrDefault(d => d.Id == definitionId) 
                      ?? throw new Exception("找不到流程定義");

            var startNode = def.Activities.First(a => a.Type == ActivityType.Start);
            
            var instance = new WorkflowInstance 
            { 
                DefinitionId = definitionId, 
                CurrentNodeId = startNode.Id,
                Variables = initialVariables ?? new Dictionary<string, object>()
            };
            
            _instances.Add(instance);
            MoveNext(instance, "Default");
            return instance;
        }

        public void CompleteTask(Guid taskId, string user, string action, Dictionary<string, object> inputVariables = null)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskId && !t.IsCompleted)
                       ?? throw new Exception("任務不存在或已完成");

            var instance = _instances.First(i => i.Id == task.InstanceId);
            
            if (inputVariables != null)
            {
                foreach (var kvp in inputVariables)
                    instance.Variables[kvp.Key] = kvp.Value;
            }

            task.IsCompleted = true;
            Console.WriteLine($"[簽核] {user} 在 [{task.NodeId}] 執行了 [{action}]");

            MoveNext(instance, action);
        }

        private void MoveNext(WorkflowInstance instance, string action)
        {
            var def = _definitions.First(d => d.Id == instance.DefinitionId);
            var transitions = def.Transitions.Where(t => t.FromNodeId == instance.CurrentNodeId).ToList();
            
            Transition selectedTransition = null;
            foreach (var t in transitions)
            {
                if (t.Condition == "Default" || t.Condition == action || EvaluateCondition(t.Condition, instance.Variables))
                {
                    selectedTransition = t;
                    break;
                }
            }

            if (selectedTransition == null) return;

            var nextNode = def.Activities.First(a => a.Id == selectedTransition.ToNodeId);
            instance.CurrentNodeId = nextNode.Id;

            if (nextNode.Type == ActivityType.End)
            {
                instance.Status = "Completed";
                Console.WriteLine($"[系統] 流程結束。最終狀態: {instance.Status}");
            }
            else if (nextNode.Type == ActivityType.UserTask)
            {
                CreateTask(instance, nextNode);
            }
        }

        private bool EvaluateCondition(string condition, Dictionary<string, object> vars)
        {
            if (string.IsNullOrEmpty(condition) || !vars.ContainsKey("Amount")) return false;
            double amount = Convert.ToDouble(vars["Amount"]);

            // 這裡可以整合更強大的 Expression Parser，此處僅為簡易模擬
            return condition switch
            {
                "Amount < 5000" => amount < 5000,
                "Amount >= 5000 && Amount < 10000" => amount >= 5000 && amount < 10000,
                "Amount >= 10000" => amount >= 10000,
                _ => false
            };
        }

        private void CreateTask(WorkflowInstance instance, Activity node)
        {
            var task = new WorkflowTask { InstanceId = instance.Id, NodeId = node.Id, Assignee = node.AssigneeRole };
            _tasks.Add(task);
            Console.WriteLine($"[任務] 分派至: {node.Name} (處理人角色: {task.Assignee})");
        }

        public List<WorkflowTask> GetActiveTasks() => _tasks.Where(t => !t.IsCompleted).ToList();
    }

    // --- 實作：從 JSON 載入流程 ---

    class Program
    {
        static void Main(string[] args)
        {
            var engine = new SimpleWorkflowEngine();

            // 模擬從外部檔案讀取的 JSON 字串
            string feeWaiverJson = @"
            {
                ""id"": ""CreditCardFeeWaiver"",
                ""name"": ""信用卡費用減免申請流程 (JSON 版)"",
                ""activities"": [
                    { ""id"": ""START"", ""name"": ""流程開始"", ""type"": ""Start"" },
                    { ""id"": ""FILL_FORM"", ""name"": ""專員填寫"", ""type"": ""UserTask"", ""assigneeRole"": ""Specialist"" },
                    { ""id"": ""SUP1"", ""name"": ""主管1覆核"", ""type"": ""UserTask"", ""assigneeRole"": ""Supervisor1"" },
                    { ""id"": ""SUP2"", ""name"": ""主管2覆核"", ""type"": ""UserTask"", ""assigneeRole"": ""Supervisor2"" },
                    { ""id"": ""SUP3"", ""name"": ""主管3覆核"", ""type"": ""UserTask"", ""assigneeRole"": ""Supervisor3"" },
                    { ""id"": ""END"", ""name"": ""流程結束"", ""type"": ""End"" }
                ],
                ""transitions"": [
                    { ""from"": ""START"", ""to"": ""FILL_FORM"", ""condition"": ""Default"" },
                    { ""from"": ""FILL_FORM"", ""to"": ""SUP1"", ""condition"": ""Amount < 5000"" },
                    { ""from"": ""FILL_FORM"", ""to"": ""SUP2"", ""condition"": ""Amount >= 5000 && Amount < 10000"" },
                    { ""from"": ""FILL_FORM"", ""to"": ""SUP3"", ""condition"": ""Amount >= 10000"" },
                    { ""from"": ""SUP1"", ""to"": ""END"", ""condition"": ""Approve"" },
                    { ""from"": ""SUP2"", ""to"": ""END"", ""condition"": ""Approve"" },
                    { ""from"": ""SUP3"", ""to"": ""END"", ""condition"": ""Approve"" }
                ]
            }";

            // 1. 部署流程 (從 JSON 載入)
            engine.Deploy(feeWaiverJson);

            // 2. 啟動流程
            Console.WriteLine("\n--- 模擬專員申請減免 $7,500 ---");
            var instance = engine.StartProcess("CreditCardFeeWaiver");

            // 3. 專員填單送出
            var task1 = engine.GetActiveTasks().First();
            engine.CompleteTask(task1.Id, "專員A", "Submit", new Dictionary<string, object> { { "Amount", 7500 } });

            // 4. 主管2審核
            var task2 = engine.GetActiveTasks().First();
            if (task2.NodeId == "SUP2")
            {
                engine.CompleteTask(task2.Id, "主管2", "Approve");
            }

            Console.ReadLine();
        }
    }
}
