#if UNITY_EDITOR
using Employee;
using Employee.StressMeter;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Employee.StressMeter
{
    [CustomEditor(typeof(StressMeterImpl))]
    [CanEditMultipleObjects]
    public class Inspector : Editor
    {
        private Rect stagesRect;

        private const float ABSOLUTE_STRESS_MIN = -10_000.0f;
        private const float ABSOLUTE_STRESS_MAX = 10_000.0f;
        private const float MIN_STRESS_STAGE_WIDTH = 0.1f;

        public override void OnInspectorGUI()
        {
            StressMeterImpl stress_meter = target as StressMeterImpl;

            _ = EditorGUILayout.BeginVertical();
            {
                bool add_stage = GUILayout.Button("+", GUILayout.Width(60f));

                if (add_stage)
                {
                    stress_meter.Stages.Insert(
                        0,
                        new StressStage() { Buff = null, StartsAt = ABSOLUTE_STRESS_MIN }
                    );
                }

                _ = EditorGUILayout.BeginHorizontal();
                {
                    DrawStageIndicator(stress_meter);

                    EditorGUILayout.Space(20);

                    DrawStages(stress_meter);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();

            _ = DrawDefaultInspector();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void DrawStageIndicator(StressMeterImpl stress_meter)
        {
            int step_count = (stress_meter.Stages.Count * 2) + 1;
            int current_step = step_count - (stress_meter.ComputeCurrentStage() * 2) - 2;

            bool prev_enabled = GUI.enabled;
            GUI.enabled = false;

            _ = EditorGUILayout.BeginVertical();
            GUILayout.Space(7.5f);
            _ = GUILayout.VerticalSlider(
                current_step,
                0,
                step_count - 1,
                GUILayout.Height(stagesRect.height - 10.0f)
            );
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();

            GUI.enabled = prev_enabled;
        }

        private void DrawStages(StressMeterImpl stress_meter)
        {
            _ = EditorGUILayout.BeginVertical(GUILayout.Height(80));
            stress_meter.MaxStress = EditorGUILayout.FloatField("max", stress_meter.MaxStress);
            stress_meter.MaxStress = Mathf.Clamp(
                stress_meter.MaxStress,
                ABSOLUTE_STRESS_MIN,
                ABSOLUTE_STRESS_MAX
            );

            for (int i = stress_meter.Stages.Count - 1; i >= 0; i--)
            {
                StressStage stage = stress_meter.Stages[i];

                _ = EditorGUILayout.BeginHorizontal();
                stage.Buff = (Buff)EditorGUILayout.ObjectField(stage.Buff, typeof(Buff), false);

                bool remove_stage = GUILayout.Button("-", GUILayout.Width(20f));
                EditorGUILayout.EndHorizontal();

                string label = i == 0 ? "min" : "threshold";
                stage.StartsAt = EditorGUILayout.FloatField(label, stage.StartsAt);

                float clamp_min = ABSOLUTE_STRESS_MIN;
                if (i != 0)
                {
                    clamp_min = stress_meter.Stages[i - 1].StartsAt + MIN_STRESS_STAGE_WIDTH;
                }

                float clamp_max;
                if (i != stress_meter.Stages.Count - 1)
                {
                    clamp_max = stress_meter.Stages[i + 1].StartsAt - MIN_STRESS_STAGE_WIDTH;
                }
                else
                {
                    clamp_max = stress_meter.MaxStress - MIN_STRESS_STAGE_WIDTH;
                }

                stage.StartsAt = Mathf.Clamp(stage.StartsAt, clamp_min, clamp_max);

                stress_meter.Stages[i] = stage;

                if (remove_stage)
                {
                    if (stress_meter.Stages.Count <= 1)
                    {
                        Debug.LogWarning("Cannot remove last element");
                    }
                    else
                    {
                        stress_meter.Stages.RemoveAt(i);
                    }
                }
            }
            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.Repaint)
            {
                stagesRect = GUILayoutUtility.GetLastRect();
            }
        }
    }
}
#endif
