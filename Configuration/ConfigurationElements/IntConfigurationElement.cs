using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OutPathOptionsMod.Configuration.ConfigurationElements
{
    public class IntConfigurationElement : ConfigurationElement
    {
        private string _title;

        private int _default;

        private int _min;

        private int _max;

        public int Value
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
                _configEntry.Value = value;
                OnChangeValue(value);
            }
        }

        public event Action<int> OnChangeValue;

        private bool _isFocused = false;

        private string _tempValue;

        private bool _slider;

        private ConfigEntry<int> _configEntry;

        private IntConfigurationElement(
            List<ConfigurationElement> configurations,
            string id,
            string title,
            int defvalue,
            int min,
            int max,
            bool slider) : base(id)
        {
            configurations.Add(this);
            _title = title;
            _default = defvalue;
            _min = min;
            _max = max;
            _configEntry = _config.Bind<int>("Config", id, defvalue);
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

        public static IntConfigurationElement Create(
            List<ConfigurationElement> configurations,
            string id,
            string title,
            int defvalue,
            int min,
            int max,
            bool slider = false)
        {
            IntConfigurationElement element = new IntConfigurationElement(configurations, id, title, defvalue, min, max, slider);
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

                    if (int.TryParse(_tempValue, out int res))
                    {
                        Value = res;
                    }
                    else
                    {
                        Value = _default;
                    }

                    if (Value > _max)
                    {
                        Value = _max;
                    }
                    else if (Value < _min)
                    {
                        Value = _min;
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
                    Value = (int)value;
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
