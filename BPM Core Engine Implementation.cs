using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BpmSystem.Engine
{
    #region 領域模型 (Domain Models - 映射自 JSON)

    public enum ActivityType { Start, UserTask, BusinessRuleTask, End }

    public class Activity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ActivityType Type { get; set; }
        public string AssigneeRole { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    public class Transition
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Condition { get; set; }
    }

    public class WorkflowDefinition
    {
        public string ProcessId { get; set; }
        public string ProcessName { get; set; }
        public int Version { get; set; }
        public List<Activity> Activities { get; set; } = new();
        public List<Transition> Transitions { get; set; } = new();
    }

    public class WorkflowInstance
    {
        public Guid InstanceId { get; set; }
        public string ProcessId { get; set; }
        public string CurrentNodeId { get; set; }
        public string Status { get; set; } // Running, Completed, Suspended
        public Dictionary<string, object> Variables { get; set; } = new();
    }

    #endregion

    #region 抽象介面 (Abstractions)

    /// <summary>
    /// 規則引擎介面，負責執行 BusinessRuleTask 與 Transition Condition 判斷
    /// </summary>
    public interface IRuleEngine
    {
        // 執行決策表並回傳結果 (例如回傳 "SUP1")
        Task<object> ExecuteRuleAsync(string ruleId, Dictionary<string, object> variables);
        
        // 執行布林表達式判斷 (例如 "Result == 'SUP1'")
        bool EvaluateCondition(string condition, Dictionary<string, object> variables);
    }

    #endregion

    #region 核心引擎 (Core Engine)

    public class BpmWorkflowEngine
    {
        private readonly IRuleEngine _ruleEngine;

        public BpmWorkflowEngine(IRuleEngine ruleEngine)
        {
            _ruleEngine = ruleEngine;
        }

        /// <summary>
        /// 驅動流程前進 (Signal)
        /// </summary>
        /// <param name="instance">當前流程實例</param>
        /// <param name="definition">流程定義</param>
        /// <param name="action">觸發動作 (如 submit, approve)</param>
        public async Task MoveNextAsync(WorkflowInstance instance, WorkflowDefinition definition, string action)
        {
            if (instance.Status == "Completed") return;

            // 1. 獲取當前節點定義
            var currentActivity = definition.Activities.FirstOrDefault(a => a.Id == instance.CurrentNodeId);
            if (currentActivity == null) throw new Exception($"Node {instance.CurrentNodeId} not found.");

            // 2. 尋找符合條件的出口 (Transition Selection)
            var nextTransition = FindEligibleTransition(instance, definition, action);
            if (nextTransition == null)
            {
                // 如果找不到出口，且當前是 UserTask，則等待人工輸入
                return;
            }

            // 3. 移動到目標節點
            instance.CurrentNodeId = nextTransition.To;
            var nextActivity = definition.Activities.First(a => a.Id == instance.CurrentNodeId);

            // 4. 根據節點類型執行對應邏輯 (Node Execution Strategy)
            await ExecuteActivityAsync(instance, nextActivity, definition);
        }

        private async Task ExecuteActivityAsync(WorkflowInstance instance, Activity activity, WorkflowDefinition definition)
        {
            switch (activity.Type)
            {
                case ActivityType.BusinessRuleTask:
                    // 對應 ACT_DECISION_GATE：執行 DMN/規則 JSON
                    string ruleId = activity.Properties["ruleId"]?.ToString();
                    var result = await _ruleEngine.ExecuteRuleAsync(ruleId, instance.Variables);
                    
                    // 將結果存入變數，供後續 Transition Condition 使用 (Result == 'SUP1')
                    instance.Variables["Result"] = result;
                    
                    // 自動流轉到下一站 (Recursive call for automatic nodes)
                    await MoveNextAsync(instance, definition, "auto");
                    break;

                case ActivityType.End:
                    instance.Status = "Completed";
                    break;

                case ActivityType.UserTask:
                    // 停在此處，等待 Task API 觸發下一次 MoveNext
                    break;

                case ActivityType.Start:
                    await MoveNextAsync(instance, definition, "default");
                    break;
            }
        }

        private Transition FindEligibleTransition(WorkflowInstance instance, WorkflowDefinition definition, string action)
        {
            var outboundTransitions = definition.Transitions.Where(t => t.From == instance.CurrentNodeId);

            foreach (var t in outboundTransitions)
            {
                // 優先處理固定 Action (如 submit, approve)
                if (t.Condition.Equals(action, StringComparison.OrdinalIgnoreCase))
                    return t;

                if (action == "default" && t.Condition == "default")
                    return t;

                // 處理邏輯表達式 (例如 "Result == 'SUP1'")
                // 這是連接 Workflow Engine 與 Rule Engine 的關鍵
                if (_ruleEngine.EvaluateCondition(t.Condition, instance.Variables))
                    return t;
            }

            return null;
        }
    }

    #endregion
}
