using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace File_Structure_Backup_Creator.Models
{
    public class ModelBase : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        public ModelBase()
        {
            IsValidaErrosNoPropertyChanged = false;
        }

        #region INotifyDataErrorInfo

        public bool IsValidaErrosNoPropertyChanged { get; set; }

        private readonly IDictionary<string, IList<string>> _errors = new Dictionary<string, IList<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void OnErrorsChanged(string propertyName)
        {
            var handled = ErrorsChanged;
            if (handled != null)
                handled(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                IList<string> propertyErrors = _errors[propertyName];
                foreach (string propertyError in propertyErrors)
                {
                    yield return propertyError;
                }
            }
            yield break;
        }

        public bool HasErrors
        {
            get { return _errors.Count > 0; }
        }

        protected void ValidarPropriedade(string propertyName, object value)
        {
            ModelBase objectToValidate = this;
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateProperty(
                value,
                new ValidationContext(objectToValidate, null, null)
                {
                    MemberName = propertyName
                },
                results);

            if (isValid)
                RemoveErrorsForProperty(propertyName);
            else
                AddErrorsForProperty(propertyName, results);

            OnErrorsChanged(propertyName);
        }

        private void AddErrorsForProperty(string propertyName, IEnumerable<ValidationResult> validationResults)
        {
            RemoveErrorsForProperty(propertyName);
            _errors.Add(propertyName, validationResults.Select(vr => vr.ErrorMessage).ToList());
        }

        private void RemoveErrorsForProperty(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
                _errors.Remove(propertyName);
        }

        public virtual bool ValidarObjeto()
        {
            ModelBase objectToValidate = this;
            _errors.Clear();
            Type objectType = objectToValidate.GetType();
            PropertyInfo[] properties = objectType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(ValidationAttribute), true).Any())
                {
                    object value = property.GetValue(objectToValidate, null);
                    ValidarPropriedade(property.Name, value);
                }
            }

            return !HasErrors;
        }

        #endregion INotifyDataErrorInfo

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged Members
    }
}
