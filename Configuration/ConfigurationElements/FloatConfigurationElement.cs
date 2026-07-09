using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OutPathOptionsMod.Configuration.ConfigurationElements
{
    public class FloatConfigurationElement : ConfigurationElement
    {
        private string _title;

        private float _default;

        private float _min;

        private float _max;

        public float Value
        {
            get
            {
                if (_configEntry != null)
                {
                    return _configEntry.Value;
                }
                else
                {
                    return _default;
                }
            }

            private set
            {
                float _value = value;
                if (_value > _max)
                    _value = _max;
                else if (_value < _min)
                    _value = _min;
                _configEntry.Value = _value;
                OnChangeValue?.Invoke(_value);
            }
        }

        private bool _isFocused = false;

        private string _tempValue;

        private bool _slider;

        public event Action<float> OnChangeValue;

        private ConfigEntry<float> _configEntry;

        private FloatConfigurationElement(
            List<ConfigurationElement> configurations,
            string id,
            string title,
            float defvalue,
            float min,
            float max,
            bool slider) : base(id)
        {
            configurations.Add(this);
            _title = title;
            _default = defvalue;
            _min = min;
            _max = max;
            _configEntry = _config.Bind<float>("Config", id, defvalue);
            _slider = slider;

            // test after cfg load
            if (Value > _max)
            {
                Value = _max;
            }
            else if (Value < _min)
            {
                Value = _min;
            }
        }

        public static FloatConfigurationElement Create(
            List<ConfigurationElement> configurations,
            string id,
            string title,
            float defvalue,
            float min,
            float max,
            bool slider = false)
        {
            FloatConfigurationElement element = new FloatConfigurationElement(configurations, id, title, defvalue, min, max, slider);
            return element;
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{_title}");

            if (!_isFocused) _tempValue = Value.ToString();
            string tvalue = GUILayout.TextArea(_tempValue);
            if (!_tempValue.Equals(tvalue))
            {
                _isFocused = true;
                _tempValue = tvalue;

                // enter -> unfocused
                if (_tempValue.Contains("\n"))
                {
                    _tempValue = _tempValue.Replace("\n", "");
                    _isFocused = false;
                    GUIUtility.keyboardControl = 0;

                    if (!_tempValue.Contains(",")) _tempValue += ",0";

                    if (float.TryParse(_tempValue, out float res))
                    {
                        Value = res;
                    }
                    else
                    {
                        Value = _default;
                    }
                }
            }

            if (_slider)
            {
                float value = GUILayout.HorizontalSlider(Value, _min, _max,
                GUILayout.MinWidth(SLIDER_MIN_WIDTH),
                GUILayout.MaxWidth(SLIDER_MAX_WIDTH));
                if (value != Value)
                {
                    Value = value;
                }
            }

            if (GUILayout.Button("Default", GUILayout.MinWidth(DEFAULT_BUTTON_SIZE), GUILayout.MaxWidth(DEFAULT_BUTTON_SIZE)))
            {
                Value = _default;
            }
            GUILayout.EndHorizontal();
        }
    }
}
