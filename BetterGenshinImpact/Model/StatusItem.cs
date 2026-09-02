using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;

namespace BetterGenshinImpact.Model
{
    public partial class StatusItem : ObservableObject
    {
        public string Name { get; }
        public string Icon { get; }
        public string DisplayName => Service.I18n.I18nService.Instance.Translate(_displayName);
        private readonly string _displayName;
        private INotifyPropertyChanged _sourceObject { get; set; }
        private string _propertyName { get; set; }

        [ObservableProperty] private bool _isEnabled;

        public StatusItem(string name, INotifyPropertyChanged sourceObject, string propertyName = "Enabled")
        {
            Name = name;
            var separatorIndex = name.IndexOf(' ');
            if (separatorIndex > 0)
            {
                Icon = name[..separatorIndex];
                _displayName = name[(separatorIndex + 1)..];
            }
            else
            {
                Icon = string.Empty;
                _displayName = name;
            }
            _sourceObject = sourceObject;
            _propertyName = propertyName;

            _sourceObject.PropertyChanged += OnSourcePropertyChanged;
            Service.I18n.I18nService.Instance.PropertyChanged += OnI18nPropertyChanged;
            IsEnabled = GetSourceValue();
        }

        private void OnI18nPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Service.I18n.I18nService.Revision))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private bool GetSourceValue()
        {
            var property = _sourceObject.GetType().GetProperty(_propertyName);
            ArgumentNullException.ThrowIfNull(property);
            var value = property.GetValue(_sourceObject);
            ArgumentNullException.ThrowIfNull(value);
            return (bool)value;
        }


        private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _propertyName)
            {
                this.IsEnabled = GetSourceValue();
            }
        }
    }
}
