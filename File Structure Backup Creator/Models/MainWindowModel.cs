using System.ComponentModel.DataAnnotations;
using System.IO;
using File_Structure_Backup_Creator.ValidationAttributes;

namespace File_Structure_Backup_Creator.Models
{
    public class MainWindowModel : ModelBase
    {
        private string _sInput;

        [Required, DiretorioExistente, DiscoLocalExistente]
        public string sInput
        {
            get { return _sInput; }
            set { _sInput = value; OnPropertyChanged(); }
        }

        private string _sOutput;

        [Required, DiscoLocalExistente]
        public string sOutput
        {
            get { return _sOutput; }
            set { _sOutput = value; OnPropertyChanged(); }
        }
        public override bool ValidarObjeto()
        {
            return base.ValidarObjeto();
        }
    }
}
